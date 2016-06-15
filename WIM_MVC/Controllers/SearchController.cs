using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WIM_MVC.Models.EntityFramework;
using PagedList;
using WIM_MVC.CustomFilters;
using WIM_MVC.Models;
using System.Web.Routing;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;

namespace WIM_MVC.Controllers
{
    public class SearchController : Controller
    {
        private int PAGESIZE = 20;
        private WIMEntities db = new WIMEntities();
        static private GridView grid = new GridView();
        // GET: /Search/
        [AuthLog(Roles = "V_Search")]
        public ActionResult Index()
        {
            if (!BaseModel.TestConnection())
            {
                return View("DbConnectionFailed");
            }

            List<SelectListItem> violate = new List<SelectListItem>();
            violate.Add(new SelectListItem { Text = "Tất cả", Value = "ALL", Selected = true });
            violate.Add(new SelectListItem { Text = "Không vi phạm", Value = "NONVIOLATE", Selected = false });
            violate.Add(new SelectListItem { Text = "Vi phạm", Value = "VIOLATE", Selected = false });
            ViewBag.ViolateLevel = violate;

            List<SelectListItem> laneItems = new List<SelectListItem>();
            laneItems.Add(new SelectListItem { Text = "Tất cả", Value = string.Empty, Selected = true });
            var listLane = db.LS_Lane.ToList();
            listLane.ForEach(p =>
            {
                laneItems.Add(new SelectListItem { Text = p.Name, Value = p.LaneCode, Selected = false });
            });
            ViewBag.LaneItems = laneItems;

            List<SelectListItem> axleItems = new List<SelectListItem>();
            axleItems.Add(new SelectListItem { Text = "Tất cả" , Value = "0", Selected = true });
            axleItems.Add(new SelectListItem { Text = "2", Value = "2", Selected = false });
            axleItems.Add(new SelectListItem { Text = "3", Value = "3", Selected = false });
            axleItems.Add(new SelectListItem { Text = "4", Value = "4", Selected = false });
            axleItems.Add(new SelectListItem { Text = "5", Value = "5", Selected = false });
            axleItems.Add(new SelectListItem { Text = "6", Value = "6", Selected = false });
            axleItems.Add(new SelectListItem { Text = "7", Value = "7", Selected = false });
            ViewBag.Axles = axleItems;

            DateTime todate = DateTime.Now;
            DateTime fromdate = todate.AddDays(-1);

            IPagedList<Trans_VehicleInfo> trans = db.Trans_VehicleInfo
                                                        .Where(w => w.WeightEndTime >= fromdate && w.WeightEndTime <= todate)
                                                        .OrderBy(o => o.WeightEndTime)
                                                        .ToList()
                                                        .ToPagedList(1, PAGESIZE);

            return View(trans);
            //return View();
        }

        [HttpPost]
        public PartialViewResult Search(string violateLevel, string laneItems, string fromDate, string toDate, string axles, string plateNumber, int? page)
        {
            if (!BaseModel.TestConnection())
            {
                return PartialView("DbConnectionFailed", null);
            }

            if (toDate.CompareTo(fromDate) == -1)
            {
                ViewBag.ErrorPeriodTime = true;
            }

            if (fromDate == "" || toDate == "")
            {
                ViewBag.ErrorGetdate = true;
                fromDate = "01/01/0001 12:00:00 SA";
                toDate = "01/01/0001 12:00:00 SA";
            }

            DateTime fromDateParam = DateTime.Parse(fromDate);
            DateTime toDateParam = DateTime.Parse(toDate);
            int axlesCount = string.IsNullOrEmpty(axles) ? -1 : int.Parse(axles);

            ViewBag.ViolateLevel = violateLevel;
            ViewBag.LaneItems = laneItems;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.Axles = axles;
            ViewBag.PlateNumber = plateNumber;

            List<Trans_VehicleInfo> temp = db.Trans_VehicleInfo
                .Where(p =>
                    (violateLevel.Equals("ALL")
                        || violateLevel.Equals("NONVIOLATE") && (p.OverloadStatus == 0 || p.OverloadStatus == null)
                        || violateLevel.Equals("VIOLATE") && p.OverloadStatus == 1)
                    && (p.LaneCode.Equals(laneItems) || string.IsNullOrEmpty(laneItems))
                    && p.WeightEndTime >= fromDateParam
                    && p.WeightEndTime <= toDateParam
                    && (p.AxlesCount == axlesCount || axlesCount == 0)
                    && (p.LicensePlate.Contains(plateNumber) || string.IsNullOrEmpty(plateNumber)))
                .OrderBy(p => p.WeightEndTime)
                .ToList();
            IPagedList<Trans_VehicleInfo> trans = temp.ToPagedList(page ?? 1, PAGESIZE);
            ViewBag.TotalSearch = temp.Count;

            grid.DataSource = temp;
            grid.DataBind();
            string sId = string.Empty;
            trans.ToList().ForEach(t =>
            {
                sId = sId + "*" + t.TranId.ToString();
            });
            ViewBag.SID = sId;           

            return PartialView("_TransactionTable",trans);
        }

        public ActionResult ConnectionFailed()
        {
            return View("DbConnectionFailed");
        }

        // GET: /Search/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trans_VehicleInfo trans_vehicleinfo = db.Trans_VehicleInfo.Find(id);
            if (trans_vehicleinfo == null)
            {
                return HttpNotFound();
            }
            return View(trans_vehicleinfo);
        }

        // GET: /Search/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Search/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="VehicleID,ID,MetrologicalID,Version,StartTime,StartTimeStr,LaneNo,ErrorFlag,WarningFlag,ViolationFlag,Direction,MoveStatus,FrontToFront,BackToFront,Duration,VehicleLength,GrossWeight,LeftWeight,RightWeight,Velocity,WheelBase,AxlesCount,MassUnit,VelocityUnit,DistanceUnit,licensePlate,UrlImagelicensePlate,UrlLaneImage,ANPRTime,Status,OverloadPercent,Datalogger_ID,TransactionId,VehicleType,LaneID,FusionWarning")] Trans_VehicleInfo trans_vehicleinfo)
        {
            if (ModelState.IsValid)
            {
                db.Trans_VehicleInfo.Add(trans_vehicleinfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(trans_vehicleinfo);
        }

        // GET: /Search/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trans_VehicleInfo trans_vehicleinfo = db.Trans_VehicleInfo.Find(id);
            if (trans_vehicleinfo == null)
            {
                return HttpNotFound();
            }
            return View(trans_vehicleinfo);
        }

        // POST: /Search/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="VehicleID,ID,MetrologicalID,Version,StartTime,StartTimeStr,LaneNo,ErrorFlag,WarningFlag,ViolationFlag,Direction,MoveStatus,FrontToFront,BackToFront,Duration,VehicleLength,GrossWeight,LeftWeight,RightWeight,Velocity,WheelBase,AxlesCount,MassUnit,VelocityUnit,DistanceUnit,licensePlate,UrlImagelicensePlate,UrlLaneImage,ANPRTime,Status,OverloadPercent,Datalogger_ID,TransactionId,VehicleType,LaneID,FusionWarning")] Trans_VehicleInfo trans_vehicleinfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trans_vehicleinfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trans_vehicleinfo);
        }

        // GET: /Search/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trans_VehicleInfo trans_vehicleinfo = db.Trans_VehicleInfo.Find(id);
            if (trans_vehicleinfo == null)
            {
                return HttpNotFound();
            }
            return View(trans_vehicleinfo);
        }

        // POST: /Search/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trans_VehicleInfo trans_vehicleinfo = db.Trans_VehicleInfo.Find(id);
            db.Trans_VehicleInfo.Remove(trans_vehicleinfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void ExportExcel()
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Charset = "";
            string FileName = "Danh Sách" + DateTime.Now + ".xls";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);
            StringWriter strwritter = new StringWriter();
            HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
            grid.RenderControl(htmltextwrtter);
            Response.Write(strwritter.ToString());
            Response.End();
            
        }
    }
}
