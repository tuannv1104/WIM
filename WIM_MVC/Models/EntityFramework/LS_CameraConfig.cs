//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WIM_MVC.Models.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class LS_CameraConfig
    {
        public System.Guid Id { get; set; }
        public string Bosch_IPAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int StationID { get; set; }
        public int PreTime { get; set; }
        public int StopTime { get; set; }
    }
}