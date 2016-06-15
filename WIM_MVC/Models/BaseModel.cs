using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using WIM_MVC.Models.EntityFramework;

namespace WIM_MVC.Models
{
    public class BaseModel
    {
        private static string EFConnectionStringTemplate =
            @"metadata=res://*/Models.EntityFramework.WIMModel.csdl|res://*/Models.EntityFramework.WIMModel.ssdl|res://*/Models.EntityFramework.WIMModel.msl;provider=System.Data.SqlClient;provider connection string='data source={0};initial catalog={1};persist security info=True;user id={2};password={3};MultipleActiveResultSets=True;App=EntityFramework'";
        private static string DefaultConnectionTemplate = @"Data Source={0};Initial Catalog={1};user id={2};password={3}";
        public static string DefaultConnection = @"Data Source=localhost;Initial Catalog=WIM;user id=sa;password=123456";
        public static string EFConnectionString = @"metadata=res://*/Models.EntityFramework.WIMModel.csdl|res://*/Models.EntityFramework.WIMModel.ssdl|res://*/Models.EntityFramework.WIMModel.msl;provider=System.Data.SqlClient;provider connection string='data source=localhost;initial catalog=WIM;persist security info=True;user id=sa;password=123456;MultipleActiveResultSets=True;App=EntityFramework'";


        public static bool LoadDBConfig()
        {
            XMLHelpers helpers = new XMLHelpers();
            List<XMLConfigModel> configdata = helpers.ReadXMLConfigs();
            if (configdata != null)
            {
                string DBServer = configdata.Where(w => w.ConfigId == "DBServer").Select(s => s.ConfigValue).FirstOrDefault();
                string DBDatabase = configdata.Where(w => w.ConfigId == "DBDatabase").Select(s => s.ConfigValue).FirstOrDefault();
                string DBUserName = configdata.Where(w => w.ConfigId == "DBUserName").Select(s => s.ConfigValue).FirstOrDefault();
                string DBPassword = configdata.Where(w => w.ConfigId == "DBPassword").Select(s => s.ConfigValue).FirstOrDefault();
                SetDBConfig(DBServer, DBDatabase, DBUserName, DBPassword);
                return true;
            }
            return false;
        }

        public static void SetDBConfig(string DBServer, string DBDatabase, string DBUserName, string DBPassword)
        {
            DefaultConnection = String.Format(DefaultConnectionTemplate, DBServer, DBDatabase, DBUserName, DBPassword);
            EFConnectionString = String.Format(EFConnectionStringTemplate, DBServer, DBDatabase, DBUserName, DBPassword);
        }


        public static bool TestConnection()
        {
            bool result = false;
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var user = context.Users.Where(w => w.UserName.ToLower() == "admin").FirstOrDefault();
                result = user != null;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public static bool TestConnection(string connectionString)
        {
            using (WIMEntities en = new WIMEntities(connectionString))
            {
                try
                {
                    AspNetUser check = en.AspNetUsers.Where(w => w.UserName == "admin").First();
                    return true;
                }
                catch
                {
                    if (en.Database.Connection.State == System.Data.ConnectionState.Open)
                        en.Database.Connection.Close();
                    return false;
                }
            }
        }

        public static bool TestConnection(string server, string database, string userid, string password, string timeout)
        {
            string connectionString = GetConnectionString(server, database, userid, password, timeout);
            return TestConnection(connectionString);
        }

        public static string GetConnectionString(string server, string database, string userid, string password, string timeout)
        {
            string sConnectionString = string.Format(EFConnectionStringTemplate, server, database, userid, password);
            return sConnectionString;
        }

    }
}