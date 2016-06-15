using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WIM_MVC.CustomFilters;

namespace WIM_MVC.Controllers
{
    public class DetailViewController : Controller
    {
        //
        // GET: /DetailView/
        [AuthLog(Roles = "V_Details")]
        public ActionResult DetailView()
        {
            return View();
        }

        //
        // GET: /DetailView/Details/5
        [AuthLog(Roles = "V_Details")]
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /DetailView/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DetailView/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /DetailView/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /DetailView/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /DetailView/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /DetailView/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
