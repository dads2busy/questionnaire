using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Questionnaire2.Models;
using Questionnaire2.DAL;

namespace Questionnaire2.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AppSettingController : Controller
    {
        private readonly QuestionnaireContext _db = new QuestionnaireContext();

        //
        // GET: /AppSetting/

        public ActionResult Index()
        {
            return View(_db.AppSettings.ToList());
        }

        //
        // GET: /AppSetting/Details/5

        public ActionResult Details(int id = 0)
        {
            AppSetting appsetting = _db.AppSettings.Find(id);
            if (appsetting == null)
            {
                return HttpNotFound();
            }
            return View(appsetting);
        }

        //
        // GET: /AppSetting/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /AppSetting/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AppSetting appsetting)
        {
            if (ModelState.IsValid)
            {
                _db.AppSettings.Add(appsetting);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(appsetting);
        }

        //
        // GET: /AppSetting/Edit/5

        public ActionResult Edit(int id = 0)
        {
            AppSetting appsetting = _db.AppSettings.Find(id);
            if (appsetting == null)
            {
                return HttpNotFound();
            }
            return View(appsetting);
        }

        //
        // POST: /AppSetting/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AppSetting appsetting)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(appsetting).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appsetting);
        }

        //
        // GET: /AppSetting/Delete/5

        public ActionResult Delete(int id = 0)
        {
            AppSetting appsetting = _db.AppSettings.Find(id);
            if (appsetting == null)
            {
                return HttpNotFound();
            }
            return View(appsetting);
        }

        //
        // POST: /AppSetting/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AppSetting appsetting = _db.AppSettings.Find(id);
            _db.AppSettings.Remove(appsetting);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}