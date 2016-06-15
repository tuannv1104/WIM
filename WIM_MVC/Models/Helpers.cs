using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace WIM_MVC.Models
{
    public class XMLHelpers
    {
        private bool IsEncrypt = false;
        //private string ConfigXMLPath = "~/App_Data/ConfigData.xml";
        //private string KeyName = "XML_ENC_WIM_KEYNAME";
        //private string KeyContainerName = "XML_ENC_WIM_KEYCONTAINERNAME";

        public List<XMLConfigModel> ReadXMLConfigs()
        {
            string xmlPath = HttpContext.Current.Server.MapPath(@Resource.Message.ConfigXMLPath);
            if (IsEncrypt)
                if (!XMLDecrypt(xmlPath)) return null;

            XDocument xmlDoc = XDocument.Load(xmlPath);
            if (IsEncrypt) XMLEncrypt(xmlPath);

            List<XElement> nodeList = xmlDoc.Root.Elements("Config").ToList();
            var configs = new List<XMLConfigModel>();
            foreach (XElement node in nodeList)
            {
                XMLConfigModel config = new XMLConfigModel();
                config.ConfigId = node.Element("ConfigId").Value;
                config.ConfigName = node.Element("ConfigName").Value;
                config.ConfigNote = node.Element("ConfigNote").Value;
                config.ConfigValue = node.Element("ConfigValue").Value;
                configs.Add(config);
            }

            return configs;
        }

        public bool SaveXMLConfig(List<XMLConfigModel> model, ref string message)
        {
            string xmlPath = HttpContext.Current.Server.MapPath(@Resource.Message.ConfigXMLPath);
            if (IsEncrypt) XMLDecrypt(xmlPath);
            XDocument xmldata = XDocument.Load(xmlPath);
            foreach (XMLConfigModel item in model)
            {
                XElement node = xmldata.Root.Elements("Config").Where(w => (string)w.Element("ConfigId") == item.ConfigId).FirstOrDefault();
                node.SetElementValue("ConfigValue", item.ConfigValue);
            }
            try
            {
                xmldata.Save(xmlPath);
                if (IsEncrypt) XMLEncrypt(xmlPath);
                return true;
            }
            catch(Exception ex)
            {
                message = ex.Message;
                if (ex.InnerException != null)
                {
                    message = ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                        message = ex.InnerException.InnerException.Message;
                }
                return false;
            }
        }


        public bool XMLEncrypt(string xmlFile)
        {
            // Create an XmlDocument object.
            XmlDocument xmlDoc = new XmlDocument();
            // Load an XML file into the XmlDocument object.
            try
            {
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Load(xmlFile);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                return false;
            }


            // Create a new CspParameters object to specify
            // a key container.
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = @Resource.Message.KeyContainerName;

            // Create a new RSA key and save it in the container.  This key will encrypt
            // a symmetric key, which will then be encryped in the XML document.
            RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);

            try
            {
                // Encrypt the "Config" element.
                Encrypt(xmlDoc, "Config", "ConfigId", rsaKey, @Resource.Message.KeyName);
                // Save the XML document.
                xmlDoc.Save(xmlFile);

            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                // Clear the RSA key.
                rsaKey.Clear();
            }
            return true;
        }

        public void Encrypt(XmlDocument Doc, string ElementToEncrypt, string EncryptionElementID, RSA Alg, string KeyName)
        {
            // Check the arguments.
            if (Doc == null)
                throw new ArgumentNullException("Doc");
            if (ElementToEncrypt == null)
                throw new ArgumentNullException("ElementToEncrypt");
            if (EncryptionElementID == null)
                throw new ArgumentNullException("EncryptionElementID");
            if (Alg == null)
                throw new ArgumentNullException("Alg");
            if (KeyName == null)
                throw new ArgumentNullException("KeyName");

            ////////////////////////////////////////////////
            // Find the specified element in the XmlDocument
            // object and create a new XmlElemnt object.
            ////////////////////////////////////////////////
            XmlNodeList elementList = Doc.GetElementsByTagName(ElementToEncrypt);
            int listCount = elementList.Count;
            for (int i =0; i < listCount; i++)
            {
                XmlElement elementToEncrypt = elementList[0] as XmlElement;
                if (elementToEncrypt == null)
                {
                    throw new XmlException("The specified element was not found");
                }
                string elementID = EncryptionElementID + "_" + (i+1);
                EncryptElement(elementToEncrypt, elementID, Alg, KeyName);
            }
        }

        public void EncryptElement(XmlElement elementToEncrypt, string EncryptionElementID, RSA Alg, string KeyName)
        {
            RijndaelManaged sessionKey = null;

            try
            {
                //////////////////////////////////////////////////
                // Create a new instance of the EncryptedXml class
                // and use it to encrypt the XmlElement with the
                // a new random symmetric key.
                //////////////////////////////////////////////////

                // Create a 256 bit Rijndael key.
                sessionKey = new RijndaelManaged();
                sessionKey.KeySize = 256;

                EncryptedXml eXml = new EncryptedXml();

                byte[] encryptedElement = eXml.EncryptData(elementToEncrypt, sessionKey, false);
                ////////////////////////////////////////////////
                // Construct an EncryptedData object and populate
                // it with the desired encryption information.
                ////////////////////////////////////////////////

                EncryptedData edElement = new EncryptedData();
                edElement.Type = EncryptedXml.XmlEncElementUrl;
                edElement.Id = EncryptionElementID;
                // Create an EncryptionMethod element so that the
                // receiver knows which algorithm to use for decryption.

                edElement.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url);
                // Encrypt the session key and add it to an EncryptedKey element.
                EncryptedKey ek = new EncryptedKey();

                byte[] encryptedKey = EncryptedXml.EncryptKey(sessionKey.Key, Alg, false);

                ek.CipherData = new CipherData(encryptedKey);

                ek.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncRSA15Url);

                // Create a new DataReference element
                // for the KeyInfo element.  This optional
                // element specifies which EncryptedData
                // uses this key.  An XML document can have
                // multiple EncryptedData elements that use
                // different keys.
                DataReference dRef = new DataReference();

                // Specify the EncryptedData URI.
                dRef.Uri = "#" + EncryptionElementID;

                // Add the DataReference to the EncryptedKey.
                ek.AddReference(dRef);
                // Add the encrypted key to the
                // EncryptedData object.

                edElement.KeyInfo.AddClause(new KeyInfoEncryptedKey(ek));
                // Set the KeyInfo element to specify the
                // name of the RSA key.


                // Create a new KeyInfoName element.
                KeyInfoName kin = new KeyInfoName();

                // Specify a name for the key.
                kin.Value = KeyName;

                // Add the KeyInfoName element to the
                // EncryptedKey object.
                ek.KeyInfo.AddClause(kin);
                // Add the encrypted element data to the
                // EncryptedData object.
                edElement.CipherData.CipherValue = encryptedElement;
                ////////////////////////////////////////////////////
                // Replace the element from the original XmlDocument
                // object with the EncryptedData element.
                ////////////////////////////////////////////////////
                EncryptedXml.ReplaceElement(elementToEncrypt, edElement, false);
            }
            catch (Exception e)
            {
                // re-throw the exception.
                throw e;
            }
            finally
            {
                if (sessionKey != null)
                {
                    sessionKey.Clear();
                }

            }
        }



        public bool XMLDecrypt(string xmlFile)
        {
            // Create an XmlDocument object.
            XmlDocument xmlDoc = new XmlDocument();
            // Load an XML file into the XmlDocument object.
            try
            {
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.Load(xmlFile);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                return false;
            }

            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = @Resource.Message.KeyContainerName;

            // Get the RSA key from the key container.  This key will decrypt
            // a symmetric key that was imbedded in the XML document.
            RSACryptoServiceProvider rsaKey = new RSACryptoServiceProvider(cspParams);

            try
            {

                // Decrypt the elements.
                Decrypt(xmlDoc, rsaKey, @Resource.Message.KeyName);
                // Save the XML document.
                xmlDoc.Save(xmlFile);
            }
            catch (Exception e)
            {
                string msg = e.Message;
                return false;
            }
            finally
            {
                // Clear the RSA key.
                rsaKey.Clear();
            }
            return true;
        }

        //public void Decrypt(XmlDocument Doc, RSA Alg, string KeyName)
        public void Decrypt(XmlDocument Doc, RSA Alg, string KeyName)
        {
            // Check the arguments.  
            if (Doc == null)
                throw new ArgumentNullException("Doc");
            if (Alg == null)
                throw new ArgumentNullException("Alg");
            if (KeyName == null)
                throw new ArgumentNullException("KeyName");

            // Create a new EncryptedXml object.
            EncryptedXml exml = new EncryptedXml(Doc);

            // Add a key-name mapping.
            // This method can only decrypt documents
            // that present the specified key name.
            exml.AddKeyNameMapping(KeyName, Alg);

            // Decrypt the element.
            exml.DecryptDocument();

        }
    }

    public static class DocumentExtensions
    {
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }
}