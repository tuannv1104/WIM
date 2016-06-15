using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WIM_MVC.Models
{
    public class Global
    {
        private static Global _instance;
        public static Global Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Global();
                }
                return _instance;
            }
        }

        public string DataLogger_1_IP { get; set; }
        public string DataLogger_2_IP { get; set; }
        public string DVR_IP { get; set; }
        public string ColorImagePath { get; set; }
        public string RecogImagePath { get; set; }
        public string ColorImageType { get; set; }
        public string RecogImageType { get; set; }
        public string Video_Dir { get; set; }

    }
}