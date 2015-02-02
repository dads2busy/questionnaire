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
    public class QuestionController : Controller
    {
        private readonly QuestionnaireContext _db = new QuestionnaireContext();

        //
        // GET: /Question/

        public ActionResult Index()
        {
            return View(_db.Questions.ToList());
        }

        //
        // GET: /Question/Details/5

        public ActionResult Details(int id = 0)
        {
            Question question = _db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        //
        // GET: /Question/Create

        public ActionResult Create()
        {
            var qtypes = _db.QTypes.Select(q => q.QTypeName).ToList();
            ViewBag.qtypes = qtypes;
            return View();
        }

        //
        // POST: /Question/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Question question, string selectedQType, string qTitle)
        {
            if (ModelState.IsValid)
            {
                question.QTypeName = selectedQType;
                question.QTitle = qTitle;

                _db.Questions.Add(question);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(question);
        }

        //
        // GET: /Question/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Question question = _db.Questions.Single(i => i.QuestionId == id);

            if (question == null)
            {
                return HttpNotFound();
            }

            string qTypeName = question.QTypeName ?? "none";

            IEnumerable<SelectListItem> sLIs = _db.QTypes
                .Select(q => new SelectListItem
                {
                    Value = q.QTypeName,
                    Text = q.QTypeName,
                    Selected = qTypeName.Equals(q.QTypeName) ? true : false
                });
            ViewBag.QTypeName = sLIs;

            return View(question);
        }

        //
        // POST: /Question/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Question question, string qTypeName)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    question.QTypeName = qTypeName;
                    _db.Entry(question).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes.");
            }
            return View(question);
        }

        //
        // GET: /Question/Delete/5

        public ActionResult Delete(int id = 0, int err = 0)
        {
            if (err != 0)
                ViewBag.message = "The question is still being used by a questionnaire and cannot be deleted.";
            else
                ViewBag.message = "";
            
            Question question = _db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        //
        // POST: /Question/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = _db.Questions.Find(id);
            var errorMessages = new System.Text.StringBuilder();
            try
            {
                _db.Questions.Remove(question);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //for (int i = 0; i < ex.Errors.Count; i++)
                //{
                //    errorMessages.Append("Index #" + i + "\n" +
                //        "Message: " + ex.Errors[i].Message + "\n" +
                //        "Error Number: " + ex.Errors[i].Number + "\n" +
                //        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                //        "Source: " + ex.Errors[i].Source + "\n" +
                //        "Procedure: " + ex.Errors[i].Procedure + "\n");
                //}
                //Console.WriteLine(errorMessages.ToString());
                return RedirectToAction("Delete", "Question", new { id, err = 1 });
            }
            
            
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}