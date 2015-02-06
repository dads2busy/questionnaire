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
    public class QCategoryController : Controller
    {
        private readonly QuestionnaireContext _db = new QuestionnaireContext();

        //
        // GET: /QCategory/

        public ActionResult Index()
        {
            return View(_db.QCategories.ToList());
        }

        //
        // GET: /QCategory/Details/5

        public ActionResult Details(int id = 0)
        {
            QCategory qcategory = _db.QCategories.Find(id);
            if (qcategory == null)
            {
                return HttpNotFound();
            }
            return View(qcategory);
        }

        //
        // GET: /QCategory/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /QCategory/Create

        [HttpPost]
        public ActionResult Create(QCategory qcategory)
        {
            if (ModelState.IsValid)
            {
                _db.QCategories.Add(qcategory);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(qcategory);
        }

        //
        // GET: /QCategory/Edit/5

        public ActionResult Edit(int id = 0)
        {
            QCategory qcategory = _db.QCategories.Find(id);
            if (qcategory == null)
            {
                return HttpNotFound();
            }
            return View(qcategory);
        }

        //
        // POST: /QCategory/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(QCategory qcategory)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(qcategory).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(qcategory);
        }

        //
        // GET: /QCategory/Delete/5

        public ActionResult Delete(int id = 0, int err = 0)
        {
            if (err != 0)
                ViewBag.message = "The section is still being used by a questionnaire and cannot be deleted.";
            else
                ViewBag.message = "";
            
            QCategory qcategory = _db.QCategories.Find(id);
            if (qcategory == null)
            {
                return HttpNotFound();
            }
            return View(qcategory);
        }

        //
        // POST: /QCategory/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            QCategory qcategory = _db.QCategories.Find(id);

            try
            {
                _db.QCategories.Remove(qcategory);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Delete", "QCategory", new { id, err = 1 });
            }
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}