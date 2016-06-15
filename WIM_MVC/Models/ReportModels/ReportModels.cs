using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WIM_MVC.Controllers;
using WIM_MVC.Models.EntityFramework;

namespace WIM_MVC.Models
{
    public class ReportModels
    {
        private static ReportModels _instant;
        public static ReportModels Instant
        {
            get {
                if (_instant == null)
                    _instant = new ReportModels();
                return _instant; 
            }
        }

        public List<Report_1_Models> GetReport1(DateTime fromdate, DateTime todate)
        {
            List<Report_1_Models> result = new List<Report_1_Models>();
            try
            {
                using (WIMEntities en = new WIMEntities())
                {
                    // Lấy tất cả record từ fromdate đến todate
                    List<Trans_VehicleInfo> transList = en.Trans_VehicleInfo
                        .Where(w => w.WeightBeginTime >= fromdate && w.WeightBeginTime <= todate)
                        .OrderBy(o => o.WeightBeginTime).ToList();

                    // hiển thị theo từng ngày và từng làn
                    for (DateTime d = fromdate; d <= todate; d = d.AddDays(1))
                    {
                        List<Trans_VehicleInfo> dayList = transList
                            .Where(w => w.WeightBeginTime >= d && w.WeightBeginTime < d.AddDays(1))
                            .OrderBy(o => o.LaneCode).ToList();
                        if (dayList.Count == 0) continue;
                        List<string> lanes = dayList.Select(s => s.LaneCode).Distinct().ToList();
                        foreach (string lane in lanes)
                        {
                            List<Trans_VehicleInfo> laneList = dayList.Where(w => w.LaneCode == lane).ToList();
                            Report_1_Models m = new Report_1_Models();
                            m.ThoiGian = d;
                            m.ThoiGianText = d.ToString("dd/MM/yyyy");
                            m.Lan = lane;
                            m.TongLuotXe = laneList.Count;
                            m.TongTrongTai = laneList.Sum(s => s.GrossWeight).Value / 1000;
                            List<Trans_VehicleInfo> overLoadList = laneList.Where(w => w.OverloadStatus > 0).ToList();
                            List<Guid> overLoadTransIdList = overLoadList.Select(s => s.TranId).ToList();
                            m.TongLuotViPham = overLoadList.Count();
                            m.SoLuotViPhamTongTaiTrong = laneList.Where(w => w.OverloadPercent > 0).Count();
                            m.SoLuotViPhamTaiTrongTruc = en.Trans_RawAxles.Where(w => overLoadTransIdList.Contains(w.TranId.Value) && w.OverloadStatus > 0).Count();
                            foreach (Trans_VehicleInfo vehicle in overLoadList)
                            {
                                m.TongTaiTrongVuotTai += (vehicle.GrossWeight.Value * vehicle.OverloadPercent.Value / (vehicle.OverloadPercent.Value + 100))/1000;
                            }
                            m.TaiTrongQuaTaiTrungBinh = (m.TongLuotViPham > 0) ? m.TongTaiTrongVuotTai / m.TongLuotViPham : 0;
                            result.Add(m);
                        }
                    }
                }
            }
            catch
            {
                // Log
            }
            return result;
        }

        public List<Report_2_Models> GetReport2(DateTime fromdate, DateTime todate)
        {
            List<Report_2_Models> result = new List<Report_2_Models>();
            try
            {
                using (WIMEntities en = new WIMEntities())
                {
                    // Lấy tất cả record từ fromdate đến todate
                    List<Trans_VehicleInfo> transList = en.Trans_VehicleInfo
                        .Where(w => w.WeightBeginTime >= fromdate && w.WeightBeginTime <= todate)
                        .OrderBy(o => o.WeightBeginTime).ToList();

                    // hiển thị theo từng ngày và từng làn
                    for (DateTime d = fromdate; d <= todate; d = d.AddDays(1))
                    {
                        List<Trans_VehicleInfo> dayList = transList
                            .Where(w => w.WeightBeginTime >= d && w.WeightBeginTime < d.AddDays(1))
                            .OrderBy(o => o.LaneCode).ToList();
                        if (dayList.Count == 0) continue;
                        List<string> lanes = dayList.Select(s => s.LaneCode).Distinct().ToList();
                        foreach (string lane in lanes)
                        {
                            List<Trans_VehicleInfo> laneList = dayList.Where(w => w.LaneCode == lane).ToList();
                            if (laneList.Count == 0) continue;
                            List<int> axles = laneList.OrderBy(o => o.AxlesCount).Select(s => s.AxlesCount.Value).Distinct().ToList();
                            foreach (int axle in axles)
                            {
                                Report_2_Models m = new Report_2_Models();
                                m.ThoiGian = d;
                                m.ThoiGianText = d.ToString("dd/MM/yyyy");
                                m.Lan = lane;
                                m.SoTruc = axle;
                                m.SoLuotXeQuaLan = laneList.Where(w => w.AxlesCount == axle).Count();
                                List<Trans_VehicleInfo> overLoadList = laneList.Where(w => w.AxlesCount == axle && w.OverloadStatus > 0).ToList();
                                m.TongLuotViPham = overLoadList.Count();
                                m.MucDoViPham = (m.TongLuotViPham > 0) ? overLoadList.Sum(s => s.OverloadPercent.Value) / m.TongLuotViPham : 0;
                                foreach (Trans_VehicleInfo vehicle in overLoadList)
                                {
                                    m.TongTaiTrongVuotTai += (vehicle.GrossWeight.Value * vehicle.OverloadPercent.Value / (vehicle.OverloadPercent.Value + 100)) / 1000;
                                }
                                m.TaiTrongQuaTaiTrungBinh = (m.TongLuotViPham > 0) ? m.TongTaiTrongVuotTai / m.TongLuotViPham : 0;
                                result.Add(m);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Log
            }
            return result;
        }

        public Report_3_Models GetReport3(Guid ID)
        {
            Report_3_Models model = new Report_3_Models();
            try
            {
                using (WIMEntities en = new WIMEntities())
                {
                    // Lấy tất cả record từ fromdate đến todate
                    Trans_VehicleInfo result = en.Trans_VehicleInfo.Where(item => item.TranId == ID).FirstOrDefault();
                    model.ThoiGian = (DateTime)result.WeightBeginTime;
                    model.Lan = result.LaneCode;
                    model.SoTruc = (int)result.AxlesCount;
                    model.SoXe = result.LicensePlate.ToUpper();
                    model.KLBanThan = (double)result.GrossWeight;
                    model.KLToanBoCP = (double)result.LimitWeight;
                    model.QuaTaiToanBo = result.OverloadPercent.ToString() + "%";

                    GlobalController gCtrl = new GlobalController();
                    model.HinhLan = Global.Instance.ColorImagePath + gCtrl.GetPathFromImageID(result.TransactionId) + Global.Instance.ColorImageType;

                    if (result.OverloadStatus == 1)
                    {
                        model.KetLuan = "Quá tải";
                    }
                    else if (result.OverloadStatus == 2)
                    {
                        model.KetLuan = "Cảnh báo";
                    }
                    else
                    {
                        model.KetLuan = "Không quá tải";                        
                    }
                    return model;
                }
            }
            catch
            {
                // Log
            }
            return model;
        }
    }

    public class Report_1_Models
    {
        public DateTime ThoiGian { get; set; }
        public string ThoiGianText { get; set; }
        public string Lan { get; set; }
        public int TongLuotXe { get; set; }
        public double TongTrongTai { get; set; }
        public int TongLuotViPham { get; set; }
        public int SoLuotViPhamTongTaiTrong { get; set; }
        public int SoLuotViPhamTaiTrongTruc { get; set; }
        public double TongTaiTrongVuotTai { get; set; }
        public double TaiTrongQuaTaiTrungBinh { get; set; }
    }

    public class Report_2_Models
    {
        public DateTime ThoiGian { get; set; }
        public string ThoiGianText { get; set; }
        public string Lan { get; set; }
        public int SoTruc { get; set; }
        public int SoLuotXeQuaLan { get; set; }
        public int TongLuotViPham { get; set; }
        public double MucDoViPham { get; set; }
        public double TongTaiTrongVuotTai { get; set; }
        public double TaiTrongQuaTaiTrungBinh { get; set; }
    }

    public class Report_3_Models
    {
        public DateTime ThoiGian { get; set; }
        public string ThoiGianText { get; set; }
        public string Lan { get; set; }
        public int SoTruc { get; set; }
        public string SoXe { get; set; }
        public double TongTaiTrong { get; set; }
        public double KLBanThan { get; set; }
        public double KLChuyenChoCP { get; set; }
        public double KLToanBoCP { get; set; }
        public double KLKeoTheoCP { get; set; }
        public double TaiTrongCP { get; set; }
        public string KetLuan { get; set; }
        public string QuaTaiToanBo { get; set; }
        public string HinhLan { get; set; }


    }
}