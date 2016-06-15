using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using WIM_MVC.Models.EntityFramework;

namespace WIM_MVC.Models
{
    public class SystemManageModels
    {
        private static SystemManageModels _instance;
        public static SystemManageModels Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SystemManageModels();
                }
                return _instance;
            }
        }
    }

    public class DBConfigLoginModels
    {

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string DBConfigPassword { get; set; }

    }

    public class DBConfigModels
    {
        [Required(ErrorMessage = "Vui lòng nhập tên máy chủ.")]
        [Display(Name = "Tên máy chủ")]
        public string DBServer { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên cơ sở dữ liệu.")]
        [Display(Name = "Tên cơ sở dữ liệu")]
        public string DBDatabase { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [Display(Name = "Tên đăng nhập")]
        public string DBUserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string DBPassword { get; set; }
    }

    public class XMLConfigModel
    {
        public string ConfigId { get; set; }
        public string ConfigName { get; set; }
        public string ConfigValue { get; set; }
        public string ConfigNote { get; set; }
    }

}