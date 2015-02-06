using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Questionnaire2.Models;
using Questionnaire2.DAL;
using WebMatrix.WebData;

namespace Questionnaire2.Controllers
{
    //[Authorize(Roles = "Administrator")]
    public class QuestionnaireQCategoryController : Controller
    {
        private readonly QuestionnaireContext _db = new QuestionnaireContext();

        //
        // GET: /QuestionnaireQ/

        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var questionnaireqcategories = _db.QuestionnaireQCategories.Include(q => q.Questionnaire).Include(q => q.QCategory);
            return View(questionnaireqcategories.ToList());
        }

        //
        // GET: /QuestionnaireQ/Details/5

        [Authorize(Roles = "Administrator")]
        public ActionResult Details(int id = 0)
        {
            QuestionnaireQCategory questionnaireqcategory = _db.QuestionnaireQCategories.Find(id);
            if (questionnaireqcategory == null)
            {
                return HttpNotFound();
            }
            return View(questionnaireqcategory);
        }

        //
        // GET: /QuestionnaireQ/Create

        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            ViewBag.QuestionnaireId = new SelectList(_db.Questionnaires, "QuestionnaireId", "QuestionnaireName");
            ViewBag.QCategoryId = new SelectList(_db.QCategories, "QCategoryId", "QCategoryName");
            return View();
        }

        //
        // POST: /QuestionnaireQ/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuestionnaireQCategory questionnaireqcategory)
        {
            if (ModelState.IsValid)
            {
                _db.QuestionnaireQCategories.Add(questionnaireqcategory);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.QuestionnaireId = new SelectList(_db.Questionnaires, "QuestionnaireId", "QuestionnaireName", questionnaireqcategory.QuestionnaireId);
            ViewBag.QCategoryId = new SelectList(_db.QCategories, "QCategoryId", "QCategoryName", questionnaireqcategory.QCategoryId);
            return View(questionnaireqcategory);
        }

        [Authorize(Roles = "CareProvider, Administrator")]
        public ActionResult DuplicateQuestionnaireQCategory(int ordinal = 0)
        {
            var qqc = new QuestionnaireQCategory() { Ordinal = ordinal };
            return View(qqc);
        }

        //
        // POST: /Questionnaire/DuplicateQuestionnaireQCategory

        [Authorize(Roles = "CareProvider, Administrator")]
        [HttpPost]
        public ActionResult DuplicateQuestionnaireQCategory(QuestionnaireQCategory qqc)
        {
            if (qqc.Ordinal <= 0) return View();

            var userId = WebSecurity.GetUserId(User.Identity.Name);
            var allOfOrdinal = _db.QuestionnaireQCategories.Where(a => a.Ordinal == qqc.Ordinal).OrderBy(o => o.SubOrdinal);
            allOfOrdinal = allOfOrdinal.Where(x => x.UserId == 0 || x.UserId == userId).OrderBy(o => o.SubOrdinal);
            var toDuplicate = allOfOrdinal.ToList().Last();

            /* add new QuestionnaireQCategory */
            var questionnaireqcategory = new QuestionnaireQCategory
            {
                QCategoryId = toDuplicate.QCategoryId,
                QuestionnaireId = toDuplicate.QuestionnaireId,
                Ordinal = toDuplicate.Ordinal,
                SubOrdinal = toDuplicate.SubOrdinal + 1,
                UserId = userId
            };
            _db.QuestionnaireQCategories.Add(questionnaireqcategory);
            _db.SaveChanges();

            /* get new Id */
            var newId = questionnaireqcategory.Id;

            /* get QuestionnaireQuestions to duplicate */
            var toDuplicateQQs = _db.QuestionnaireQuestions.Where(x => x.QQCategoryId == toDuplicate.QCategoryId);

            foreach (var dupQ in toDuplicateQQs) //.Select(qq => new QuestionnaireQuestion()
            {
                var qq = new QuestionnaireQuestion
                {
                    QQCategoryId = newId,
                    Ordinal = dupQ.Ordinal,
                    QuestionnaireId = dupQ.QuestionnaireId,
                    QuestionId = dupQ.QuestionId,
                    UserId = userId
                };

                _db.QuestionnaireQuestions.Add(qq);
            }
            _db.SaveChanges();

            return RedirectToAction("Edit", "Response", new { area = "", id = toDuplicate.QuestionnaireId });
        }

        //
        // GET: /QuestionnaireQ/Edit/5

        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id = 0)
        {
            QuestionnaireQCategory questionnaireqcategory = _db.QuestionnaireQCategories.Find(id);
            if (questionnaireqcategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuestionnaireId = new SelectList(_db.Questionnaires, "QuestionnaireId", "QuestionnaireName", questionnaireqcategory.QuestionnaireId);
            ViewBag.QCategoryId = new SelectList(_db.QCategories, "QCategoryId", "QCategoryName", questionnaireqcategory.QCategoryId);
            return View(questionnaireqcategory);
        }

        //
        // POST: /QuestionnaireQ/Edit/5

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(QuestionnaireQCategory questionnaireqcategory)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(questionnaireqcategory).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.QuestionnaireId = new SelectList(_db.Questionnaires, "QuestionnaireId", "QuestionnaireName", questionnaireqcategory.QuestionnaireId);
            ViewBag.QCategoryId = new SelectList(_db.QCategories, "QCategoryId", "QCategoryName", questionnaireqcategory.QCategoryId);
            return View(questionnaireqcategory);
        }

        //
        // GET: /QuestionnaireQ/Delete/5

        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id = 0, int err = 0)
        {
            if (err != 0)
                ViewBag.message = "The section is still being used by a questionnaire and cannot be deleted.";
            else
                ViewBag.message = "";
            
            QuestionnaireQCategory questionnaireqcategory = _db.QuestionnaireQCategories.Find(id);
            if (questionnaireqcategory == null)
            {
                return HttpNotFound();
            }
            return View(questionnaireqcategory);
        }

        //
        // POST: /QuestionnaireQ/Delete/5

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                QuestionnaireQCategory questionnaireqcategory = _db.QuestionnaireQCategories.Find(id);
                _db.QuestionnaireQCategories.Remove(questionnaireqcategory);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return RedirectToAction("Delete", "QuestionnaireQCategory", new { id, err = 1 });
            }
            
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}