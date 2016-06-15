using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;
using WIM_MVC.Models;
using WIM_MVC.CustomFilters;
using WIM_MVC.Models.EntityFramework;

namespace WIM_MVC.Controllers
{
    public class SystemManageController : Controller
    {
        private WIMEntities db = new WIMEntities();

        //
        // GET: /SystemManage/DBConfigLogin
        [HttpGet]
        public ActionResult DBConfigLogin()
        {
            return View();
        }

        //
        // POST: /SystemManage/DBConfigLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DBConfigLogin(DBConfigLoginModels model)
        {
            XMLHelpers helpers = new XMLHelpers();
            List<XMLConfigModel> configdata = helpers.ReadXMLConfigs();
            if (configdata == null)
            {
                return View("DbConnectionFailed");
            }
            //string pass = configdata.Where(w => w.ConfigId == "DBConfigPassword").Select(s => s.ConfigValue).FirstOrDefault();
            string pass = "123";
            if (ModelState.IsValid)
            {
                if (model.DBConfigPassword == pass)
                {
                    DBConfigModels scmodel = new DBConfigModels();
                    scmodel.DBServer = configdata.Where(w => w.ConfigId == "DBServer").Select(s => s.ConfigValue).FirstOrDefault();
                    scmodel.DBDatabase = configdata.Where(w => w.ConfigId == "DBDatabase").Select(s => s.ConfigValue).FirstOrDefault();
                    scmodel.DBUserName = configdata.Where(w => w.ConfigId == "DBUserName").Select(s => s.ConfigValue).FirstOrDefault();
                    scmodel.DBPassword = configdata.Where(w => w.ConfigId == "DBPassword").Select(s => s.ConfigValue).FirstOrDefault();
                    return RedirectToAction("DBConfigView", "SystemManage", scmodel);
                }
                else
                {
                    ModelState.AddModelError("", "Mật khẩu không đúng");
                    return View(model);
                }
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Lỗi truy cập hệ thống.");
            return View(model);
        }

        //
        // GET: /SystemManage/DBConfigView
        [HttpGet]
        public ActionResult DBConfigView(DBConfigModels model)
        {
            return View(model);
        }

        //
        // POST: /SystemManage/Test Connection
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DBConfigView(DBConfigModels model, string returnUrl)
        {
            ViewBag.SuccessMessage = string.Empty;
            ViewBag.ErrorMessage = string.Empty;
            string strError = "";
            if (Request.Form["Save"] != null)
            {
                if (ModelState.IsValid)
                {
                    List<XMLConfigModel> xmldata = new List<XMLConfigModel>();
                    XMLConfigModel item = new XMLConfigModel();
                    item.ConfigId = "DBServer";
                    item.ConfigName = "DBServer";
                    item.ConfigValue = model.DBServer;
                    item.ConfigNote = ".";
                    xmldata.Add(item);
                    item = new XMLConfigModel();
                    item.ConfigId = "DBDatabase";
                    item.ConfigName = "DBDatabase";
                    item.ConfigValue = model.DBDatabase;
                    item.ConfigNote = ".";
                    xmldata.Add(item);
                    item = new XMLConfigModel();
                    item.ConfigId = "DBUserName";
                    item.ConfigName = "DBUserName";
                    item.ConfigValue = model.DBUserName;
                    item.ConfigNote = ".";
                    xmldata.Add(item);
                    item = new XMLConfigModel();
                    item.ConfigId = "DBPassword";
                    item.ConfigName = "DBPassword";
                    item.ConfigValue = model.DBPassword;
                    item.ConfigNote = ".";
                    xmldata.Add(item);
                    XMLHelpers helpers = new XMLHelpers();
                    if (helpers.SaveXMLConfig(xmldata, ref strError))
                    {
                        BaseModel.SetDBConfig(model.DBServer, model.DBDatabase, model.DBUserName, model.DBPassword);
                        return Redirect("~/Home");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Lưu cấu hình thất bại! " + strError;
                        return View(model);
                    }
                }
                ViewBag.ErrorMessage = "Lỗi! ";
                return View(model);
            }
            else //if (Request.Form["TestConnection"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (BaseModel.TestConnection(model.DBServer, model.DBDatabase, model.DBUserName, model.DBPassword, "10"))
                    {
                        ViewBag.SuccessMessage = "Kết nối cơ sở dữ liệu thành công";
                    }
                    else
                    {
                        ViewBag.ConnectionErrorMessage = "Kết nối cơ sở dữ liệu thất bại !";
                    }
                    return View(model);
                }

                ViewBag.ErrorMessage = "Lỗi";
                return View(model);
            }
        }

        //
        // GET: /SystemManage/SystemConfigView

        [HttpGet]
        [AuthLog(Roles = "V_SystemManage")]
        public ActionResult SystemConfigView()
        {
            if (!BaseModel.TestConnection())
            {
                return View("DbConnectionFailed");
            }

            List<SYS_ConfigInfo> model = null;
            try
            {
                model = db.SYS_ConfigInfo.OrderBy(o => o.ConfigInfoID).ToList();
                //using (WIMEntities db = new WIMEntities())
                //{
                //    model = db.SYS_ConfigInfo.OrderBy(o => o.ConfigInfoID).ToList();
                //    if (model != null && model.Count == 0)
                //    {
                //        // Create SYS_ConfigInfo
                //        SYS_ConfigInfo sys = new SYS_ConfigInfo();
                //        sys.Key = "DataLogger_1_IP";
                //        sys.Value = "192.168.1.10";
                //        sys.Description = "Địa chỉ IP của Data Logger 1";
                //        sys.InfoType = "";
                //        sys.IsUsed = true;
                //        model.Add(sys);
                //        db.SYS_ConfigInfo.Add(sys);
                //        sys = new SYS_ConfigInfo();
                //        sys.Key = "DataLogger_2_IP";
                //        sys.Value = "192.168.1.11";
                //        sys.Description = "Địa chỉ IP của Data Logger 2";
                //        sys.InfoType = "";
                //        sys.IsUsed = true;
                //        model.Add(sys);
                //        db.SYS_ConfigInfo.Add(sys);
                //        sys = new SYS_ConfigInfo();
                //        sys.Key = "DVR_IP";
                //        sys.Value = "192.168.1.100";
                //        sys.Description = "Địa chỉ IP của đầu ghi Video";
                //        sys.InfoType = "";
                //        sys.IsUsed = true;
                //        model.Add(sys);
                //        db.SYS_ConfigInfo.Add(sys);
                //        sys = new SYS_ConfigInfo();
                //        sys.Key = "Picture_Dir";
                //        sys.Value = @"D:\AnhNhanDang";
                //        sys.Description = "Thư mục lưu ảnh nhận dạng";
                //        sys.InfoType = "";
                //        sys.IsUsed = true;
                //        model.Add(sys);
                //        db.SYS_ConfigInfo.Add(sys);
                //        sys = new SYS_ConfigInfo();
                //        sys.Key = "Video_Dir";
                //        sys.Value = @"D:\Video";
                //        sys.Description = "Thư mục lưu video";
                //        sys.InfoType = "";
                //        sys.IsUsed = true;
                //        model.Add(sys);
                //        db.SYS_ConfigInfo.Add(sys);
                //        db.SaveChanges();
                //    }
                //}
            }
            catch
            {
                //Log
            }
            return View(model);
        }

        [HttpPost]
        [AuthLog(Roles = "V_SystemManage")]
        [ValidateAntiForgeryToken]
        public ActionResult SystemConfigView(int configId, string configValue)
        {
            if (!BaseModel.TestConnection())
            {
                return View("DbConnectionFailed");
            }
            List<SYS_ConfigInfo> model = null;
            if (ModelState.IsValid)
            {
                SYS_ConfigInfo sys = db.SYS_ConfigInfo.Find(configId);
                if (sys != null)
                {
                    sys.Value = configValue;
                    db.SaveChanges();
                }
                //model = SystemManageModels.Instance.GetAllSYSConfigInfo();
                //return RedirectToAction("SystemConfigView", model);
            }
            model = db.SYS_ConfigInfo.OrderBy(o => o.ConfigInfoID).ToList();
            return View(model);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }

}
