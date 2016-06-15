using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using WIM_MVC.Models;
using WIM_MVC.Models.EntityFramework;

namespace WIM_MVC.Controllers
{
    public class GlobalController
    {
        WIMEntities db = new WIMEntities();
        public void LoadConfigFromDB()
        {
            if (!BaseModel.TestConnection())
            {
                return;
            }
            //Global.Instance.ColorImagePath = GetConfigValue("ColorImage_Dir");
            //Global.Instance.RecogImagePath = GetConfigValue("RecogImage_Dir");
            Global.Instance.ColorImagePath = "/ColorImagePath";
            Global.Instance.RecogImagePath = "/RecogImagePath";
            Global.Instance.ColorImageType = GetConfigValue("ColorImageType");
            Global.Instance.RecogImageType = GetConfigValue("RecogImageType");
        }

        public string GetConfigValue(string key)
        {
            try
            {
                SYS_ConfigInfo config = db.SYS_ConfigInfo.Where(w => w.Key == key).FirstOrDefault();
                if (config == null)
                    return string.Empty;
                else
                    return config.Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Function trả về đường dẫn đầy đủ của hình nhận dạng và hình màu
        /// </summary>
        public string GetPathFromImageID( string transactionID)
        {
            string pathimage = "";
            try
            {
                string sImage = transactionID.Trim();
                int n = 4;
                string sNam = transactionID.Substring(0, n);
                transactionID = transactionID.Substring(n, transactionID.Length - n);
                n = 2;
                string sThang = transactionID.Substring(0, n);
                transactionID = transactionID.Substring(n, transactionID.Length - n);
                n = 2;
                string sNgay = transactionID.Substring(0, n);
                transactionID = transactionID.Substring(n, transactionID.Length - n);
                n = 2;
                string sTemp = transactionID.Substring(0, n);
                transactionID = transactionID.Substring(n, transactionID.Length - n);

                n = 2;
                string sLane = transactionID.Substring(transactionID.IndexOf('_') + 1, n);
                transactionID = transactionID.Substring(n + 1, transactionID.Length - (n + 1));
                pathimage = string.Format(@"\{0}\{1}\{2}\{3}\{4}\{5}", sNam, sThang, sNgay, sTemp,
                                          sLane, sImage);
            }
            catch
            {
            }
            return pathimage;
        }
    }
}