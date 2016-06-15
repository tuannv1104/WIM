using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using WIM_MVC.Models;
using WIM_MVC.Models.EntityFramework;
using WIM_MVC.CustomFilters;

namespace WIM_MVC.Controllers
{
    public class GeneralViewController : Controller
    {
        private WIMEntities db = new WIMEntities();

        // GET: /GeneralView/
        [AuthLog(Roles = "V_General")]
        public ActionResult GeneralView()
        {
            if (!BaseModel.TestConnection())
            {
                return View("DbConnectionFailed");
            }

            List<SelectListItem> res = new List<SelectListItem>();
            res.Add(new SelectListItem { Text = "5", Value = "5", Selected = false });
            res.Add(new SelectListItem { Text = "10", Value = "10", Selected = false });
            res.Add(new SelectListItem { Text = "15", Value = "15", Selected = false });
            ViewBag.RefreshTime = res;

            List<SelectListItem> lane = new List<SelectListItem>();
            lane.Add(new SelectListItem { Text = "Tất cả", Value = string.Empty, Selected = true });
            var lsLane = db.LS_Lane.ToList();
            lsLane.ForEach(l =>
                            {
                                lane.Add(new SelectListItem
                                {
                                    Text = l.LaneCode,
                                    Value = l.LaneCode,
                                    Selected = false
                                });
                            });
            ViewBag.Lane = lane;

            List<SelectListItem> dis = new List<SelectListItem>();
            dis.Add(new SelectListItem { Text = "5", Value = "5", Selected = false });
            dis.Add(new SelectListItem { Text = "10", Value = "10", Selected = true });
            dis.Add(new SelectListItem { Text = "20", Value = "20", Selected = false });
            ViewBag.DisplayRaw = dis;

            return View();
        }

        public PartialViewResult GeneralAuto(string lane, string raw)
        {
            var trans = db.Trans_VehicleInfo.Where(p => p.LaneCode == lane || string.IsNullOrEmpty(lane))
                                                .OrderByDescending(o => o.WeightEndTime).Take(int.Parse(raw))
                                                .ToList();
            string sId = string.Empty;
            trans.ToList().ForEach(t =>
            {
                sId = sId + "*" + t.TranId.ToString();
            });
            ViewBag.SID = sId;
            return PartialView("_GeneralTable", trans);

        }

        // GET: /GeneralView/Details/5
        [AuthLog(Roles = "V_Details")]
        public ActionResult Details(Guid? id, string sid, string act, string newPlateNumber)
        {
            if (!BaseModel.TestConnection())
            {
                return View("DbConnectionFailed");
            }
            Trans_VehicleInfo trans_vehicleinfo = new Trans_VehicleInfo();
            if (!string.IsNullOrEmpty(act) && act.Equals("UpdatePlate") && !string.IsNullOrEmpty(newPlateNumber) && newPlateNumber.Length > 4)
            {
                var item = db.Trans_VehicleInfo.Find(id);
                item.LicensePlate = newPlateNumber;
                db.SaveChanges();
                trans_vehicleinfo = item;

                ViewBag.SID = sid;
            }
            else if (sid == null)
            {
                trans_vehicleinfo = db.Trans_VehicleInfo.OrderByDescending(info => info.WeightBeginTime).First();
                ViewBag.SID = trans_vehicleinfo.TranId.ToString();
                if (trans_vehicleinfo == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    trans_vehicleinfo.Trans_RawAxles = db.Trans_RawAxles
                                                         .Where(w => w.TranId == trans_vehicleinfo.TranId)
                                                         .OrderBy(o => o.AxleGroupId)
                                                         .ThenBy(t => t.Distance)
                                                         .ToList();
                }
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                ViewBag.SID = sid;
                if(!string.IsNullOrEmpty(act))
                {
                    var listID = sid.Split('*').ToList();
                    listID.RemoveAt(0);
                    if(act.Equals("next"))
                    {
                        int index = listID.IndexOf(id.ToString());
                        trans_vehicleinfo = db.Trans_VehicleInfo.Find(Guid.Parse(listID.ElementAt(index + 1)));
                    }
                    else if (act.Equals("previous"))
                    {
                        int index = listID.IndexOf(id.ToString());
                        trans_vehicleinfo = db.Trans_VehicleInfo.Find(Guid.Parse(listID.ElementAt(index - 1)));
                    }
                }
                else
                {
                    trans_vehicleinfo = db.Trans_VehicleInfo.Find(id);
                }
                if (trans_vehicleinfo == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    trans_vehicleinfo.Trans_RawAxles = db.Trans_RawAxles.Where(w => w.TranId == trans_vehicleinfo.TranId)
                                                                        .OrderBy(o => o.AxleGroupId)
                                                                        .ThenBy(t => t.Distance)
                                                                        .ToList();
                }
            }
            return View(trans_vehicleinfo);
        }

        public PartialViewResult UpdatePlateNumber(Guid? id, string newPlateNumber)
        {
            
            string rs = string.Empty;
            try
            {
                var item = db.Trans_VehicleInfo.Find(id);
                if (item != null)
                {
                    item.LicensePlate = newPlateNumber;
                    db.SaveChanges();
                    return PartialView("_UpdatedPlate", newPlateNumber);
                }
                else
                {
                    return PartialView("_UpdatedPlate", newPlateNumber);
                }
            }
            catch
            {
                return PartialView("_UpdatedPlate", newPlateNumber);
            }
        }

        // GET: /GeneralView/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /GeneralView/Create
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

        // GET: /GeneralView/Edit/5
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

        // POST: /GeneralView/Edit/5
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

        // GET: /GeneralView/Delete/5
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

        private DataSet DsImages;

        public ActionResult ExportReport(Guid id)
        {
            Stream stream = null;
            FileStreamResult pdfFile = null;
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/rpt_Report_3.rpt")));
            var rs = ReportModels.Instant.GetReport3(id);
            //rd.SetParameterValue("Ngaythang", rs.ThoiGian);
            //rd.SetParameterValue("Lan", rs.Lan);
            //rd.SetParameterValue("SoTruc", rs.SoTruc);
            //rd.SetParameterValue("SoXe", rs.SoXe);
            //rd.SetParameterValue("TongTaiTrong", rs.TongTaiTrong);
            //rd.SetParameterValue("KLBanThan", rs.KLBanThan);
            //rd.SetParameterValue("KLChuyenChoCP", rs.KLChuyenChoCP);
            //rd.SetParameterValue("KLToanBoCP", rs.KLToanBoCP);
            //rd.SetParameterValue("KLKeoTheoCP", rs.KLKeoTheoCP);
            //rd.SetParameterValue("TTChoPhep", rs.TaiTrongCP);
            //rd.SetParameterValue("KetLuan", rs.KetLuan);
            //rd.SetParameterValue("QuaTaiToanBo", rs.QuaTaiToanBo);
            string logoPath = HttpContext.Server.MapPath(rs.HinhLan);
            if (!System.IO.File.Exists(logoPath))
                logoPath = Server.MapPath("/Images/no-image.jpg");
            FileStream FilStr = new FileStream(logoPath, FileMode.Open);
            BinaryReader BinRed = new BinaryReader(FilStr);
            //create a new data set. 
            DsImages = new DataSet();
            //create a new table with two columns and add the table to the dataset
            DataTable ImageTable = new DataTable("Images");
            //Important note
            //Note the type of the image column. You want to give this column as a blob field to the crystal report.
            //therefore define the column type as System.Byte[]
            ImageTable.Columns.Add(new DataColumn("image", typeof(Byte[])));
            ImageTable.Columns.Add(new DataColumn("SoXe", typeof(string)));
            ImageTable.Columns.Add(new DataColumn("Lan", typeof(string)));
            ImageTable.Columns.Add(new DataColumn("SoTruc", typeof(string)));
            ImageTable.Columns.Add(new DataColumn("TongTaiTrong", typeof(string)));
            ImageTable.Columns.Add(new DataColumn("KLBanThan", typeof(string)));
            ImageTable.Columns.Add(new DataColumn("KLChuyenChoCP", typeof(string)));
            ImageTable.Columns.Add(new DataColumn("KLToanBoCP", typeof(string)));
            ImageTable.Columns.Add(new DataColumn("KLKeoTheoCP", typeof(string)));
            ImageTable.Columns.Add(new DataColumn("TTChoPhep", typeof(string)));
            ImageTable.Columns.Add(new DataColumn("KetLuan", typeof(string)));
            ImageTable.Columns.Add(new DataColumn("QuaTaiToanBo", typeof(string)));
            ImageTable.Columns.Add(new DataColumn("NgayThang", typeof(DateTime)));
            ImageTable.Columns.Add(new DataColumn("imageND", typeof(Byte[])));

            DsImages.Tables.Add(ImageTable);

            DataRow dr = this.DsImages.Tables["Images"].NewRow();
            dr["image"] = BinRed.ReadBytes((int)BinRed.BaseStream.Length);
            dr["imageND"] = BinRed.ReadBytes((int)BinRed.BaseStream.Length);

            dr["SoXe"] = rs.SoXe;
            dr["SoTruc"] = rs.SoTruc;
            dr["Lan"] = rs.Lan;
            dr["TongTaiTrong"] = rs.TongTaiTrong;
            dr["KLBanThan"] = rs.KLBanThan;
            dr["KLChuyenChoCP"] = rs.KLChuyenChoCP;
            dr["KLToanBoCP"] = rs.KLToanBoCP;
            dr["KLKeoTheoCP"] = rs.KLKeoTheoCP;
            dr["TTChoPhep"] = rs.TaiTrongCP;
            dr["KetLuan"] = rs.KetLuan;
            dr["QuaTaiToanBo"] = rs.QuaTaiToanBo;
            dr["NgayThang"] = rs.ThoiGian;

            DsImages.Tables["Images"].Rows.Add(dr);

            FilStr.Close();
            BinRed.Close();

            rd.SetDataSource(DsImages);
            rd.SetParameterValue("DonVi", "01");
            rd.SetParameterValue("TenTram", "TRẠM CÂN QL 20");
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            try
            {
                stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // POST: /GeneralView/Delete/5
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
    }
}
