using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WIM_MVC.Models
{
    public class TransVehicleInfoModel
    {
        public System.Guid TranId { get; set; }
        public Nullable<System.DateTime> WeightBeginTime { get; set; }
        public Nullable<System.DateTime> WeightEndTime { get; set; }
        public Nullable<double> WeightDuration { get; set; }
        public Nullable<int> VehicleIdFromLogger { get; set; }
        public string WeightIdFromLogger { get; set; }
        public Nullable<int> LoggerId { get; set; }
        public Nullable<double> Velocity { get; set; }
        public Nullable<double> VehicleLength { get; set; }
        public Nullable<double> WheelBaseLength { get; set; }
        public Nullable<double> LeftWeight { get; set; }
        public Nullable<double> RightWeight { get; set; }
        public Nullable<double> GrossWeight { get; set; }
        public Nullable<double> LimitWeight { get; set; }
        public Nullable<double> OverloadPercent { get; set; }
        public Nullable<int> OverloadStatus { get; set; }
        public Nullable<int> TranStatus { get; set; }
        public string LicensePlate { get; set; }
        public Nullable<int> AxlesCount { get; set; }
        public string LaneCode { get; set; }
        public string VehicleTypeCode { get; set; }
        public string TransactionId { get; set; }
        public string ViolateLevel { get; set; }
        public string Balance { get; set; }

    }

}