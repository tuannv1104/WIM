using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WIM_MVC.CustomFilters;
using WIM_MVC.Models;
using WIM_MVC.Models.EntityFramework;

namespace WIM_MVC.Controllers
{
    public class ReportController : Controller
    {
        private WIMEntities db = new WIMEntities();

        [HttpGet]
        [AuthLog(Roles = "V_Report")]
        public ActionResult Index()
        {
            if (!BaseModel.TestConnection())
            {
                return View("DbConnectionFailed");
            }
            List<LS_Report> model = db.LS_Report.ToList();
            return View(model);
        }


        [HttpPost]
        [AuthLog(Roles = "V_Report")]
        public ActionResult GeneratePDF(string ReportId, string fromDate, string toDate)
        {
            Stream stream = null;
            FileStreamResult pdfFile = null;
            DateTime from = StringToDateTime(fromDate);
            DateTime to = StringToDateTime(toDate);
            if (ReportId == "1")
            {
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/rpt_Report_1.rpt")));
                //rd.SetParameterValue("DonVi", "ABCD");
                //rd.SetParameterValue("TenTram", "TRẠM CÂN QL 20");
                //rd.SetParameterValue("NgayThangNam", "Tháng 01/2016");
                //DateTime fromdate = new DateTime(2016, 04, 15);
                //DateTime todate = DateTime.Now;
                List<Report_1_Models> report = ReportModels.Instant.GetReport1(from, to);
                rd.SetDataSource(report);
                rd.SetParameterValue("DonVi", "01");
                rd.SetParameterValue("TenTram", "TRẠM CÂN QL 20");
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                pdfFile = File(stream, "application/pdf");
            }
            else if (ReportId == "2")
            {
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/rpt_Report_2.rpt")));
                //rd.SetParameterValue("DonVi", "ABCD");
                //rd.SetParameterValue("TenTram", "TRẠM CÂN QL 20");
                //rd.SetParameterValue("NgayThangNam", "Tháng 01/2016");
                //DateTime fromdate = new DateTime(2016, 04, 15);
                //DateTime todate = DateTime.Now;
                List<Report_2_Models> report = ReportModels.Instant.GetReport2(from, to);
                rd.SetDataSource(report);
                rd.SetParameterValue("DonVi", "01");
                rd.SetParameterValue("TenTram", "TRẠM CÂN QL 20");
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                pdfFile = File(stream, "application/pdf");
            }

            return pdfFile;
        }

        /// <summary>
        /// Parse string "DD/MM/YYYY" to DateTime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        DateTime StringToDateTime(string date)
        {
            string[] d = date.Split('/');
            int dd = Convert.ToInt32(d[0]);
            int mm = Convert.ToInt32(d[1]);
            int yy = Convert.ToInt32(d[2]);
            return new DateTime(yy, mm, dd);
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
