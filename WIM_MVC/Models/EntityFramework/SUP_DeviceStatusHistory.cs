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
    
    public partial class SUP_DeviceStatusHistory
    {
        public System.Guid DeviceStatusHistoryID { get; set; }
        public System.Guid DeviceStatusID { get; set; }
        public Nullable<int> LaneDeviceID { get; set; }
        public string DeviceCodeType { get; set; }
        public string IPAddress { get; set; }
        public short Status { get; set; }
        public string StationCode { get; set; }
        public string LaneCode { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public string Note { get; set; }
    }
}