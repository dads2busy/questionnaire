using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Questionnaire2.Models;
using Questionnaire2.DAL;
using Questionnaire2.ViewModels;
using WebMatrix.WebData;
using Questionnaire2.Helpers;
using System.IO;
using System.Transactions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Web;
using System.Text;
using Aspose.Words;
using Aspose.Words.Saving;

namespace Questionnaire2.Controllers
{
    [Authorize(Roles = "Administrator, CareProvider")]
    public class ResponseController : Controller
    {
        private readonly QuestionnaireContext _db = new QuestionnaireContext();

        //
        // GET: /Response/

        public ActionResult Index()
        {
            return View(_db.Responses.ToList());
        }

        //
        // GET: /Response/Details/5

        public ActionResult Details(int id = 0)
        {
            Response response = _db.Responses.Find(id);
            if (response == null)
            {
                return HttpNotFound();
            }
            return View(response);
        }

        //
        // GET: /Response/Create

        public ActionResult Download()
        {
            return View();
        }

        //
        // POST: /Response/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Download(RegisterExternalLoginModel mReg, string Command, int id = 0)
        {
            if (Command == "MS Word")
            {
                try
                {
                    var userId = WebSecurity.GetUserId(User.Identity.Name);
                    var responses = _db.Responses.Where(x => x.UserId == userId).OrderBy(x => x.Ordinal).ThenBy(x => x.SubOrdinal).ThenBy(x => x.QQOrd).ToList();
                    var categories = new List<string> { "Personal Information", "Employment", "Education", "Coursework", "Certifications", "Licenses", "Credentials", "Training" };
                    var fui = new FormatUserInformation(responses, categories);
                    var formatted = fui.Format();
                    var ms = MakeWordFile.CreateDocument(formatted);
                    var ms2 = new MemoryStream(ms.ToArray());
            
                    Response.Clear();
                    Response.AddHeader("content-disposition", "attachment; filename=\"Portfolio.docx\"");
                    Response.ContentType = "application/msword";
                    ms2.WriteTo(Response.OutputStream);
                    Response.End(); 
                }
                catch (Exception ex)
                { Response.Write(ex.Message); }
            }
            else if (Command == "Pdf")
            {
                try
                {
                    var userId = WebSecurity.GetUserId(User.Identity.Name);
                    var responses = _db.Responses.Where(x => x.UserId == userId).OrderBy(x => x.Ordinal).ThenBy(x => x.SubOrdinal).ThenBy(x => x.QQOrd).ToList();
                    var categories = new List<string> { "Personal Information", "Employment", "Education", "Coursework", "Certifications", "Licenses", "Credentials", "Training" };
                    var fui = new FormatUserInformation(responses, categories);
                    var formatted = fui.Format();
                    var ms = MakeWordFile.CreateDocument(formatted);
                    var ms2 = new MemoryStream(ms.ToArray());

                    Aspose.Words.Document doc = new Aspose.Words.Document(ms2);
                    var ms3 = new MemoryStream();
                    doc.Save(ms3, SaveFormat.Pdf);

                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment; filename=\"Portfolio.pdf\"");

                    ms3.WriteTo(Response.OutputStream);
                    Response.End();
                }
                catch (Exception ex)
                { Response.Write(ex.Message); }
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        //
        // GET: /Response/Edit/5

        public ActionResult Edit(int id = 1)
        {
            ViewBag.Title = "Child Care Professional Registry Application";
            ViewBag.Message = "";
            var userId = WebSecurity.GetUserId(User.Identity.Name);

            var lockedSections =
                _db.Verifications.Where(x => x.UserId == userId && x.Editable == false).Select(x => x.QQCategoryId).ToList();     

            var viewModel = new QuestionnaireAppData();

            /* Add Fully Loaded (.Included) Questionnaire to the ViewModel */
            viewModel.Questionnaire =
                _db.Questionnaires
                    .Include(a => a.QuestionnaireQuestions
                        .Select(b => b.Question.QType).Select(c => c.Answers))
                    .Include(a => a.QuestionnaireQCategories
                        .Select(b => b.QCategory))
                    .Where(n => n.QuestionnaireId == id)
                    .Single();

            viewModel.Questionnaire.QuestionnaireQuestions = viewModel.Questionnaire.QuestionnaireQuestions.Where(x => x.UserId == userId || x.UserId == 0).ToList();

            var appQuestions = new List<Response>();
           
            var qqList = viewModel.Questionnaire.QuestionnaireQuestions.ToList();

            var distinctQQCId = qqList.Select(x => x.QQCategoryId).Distinct();

            for (var i = 0; i < viewModel.Questionnaire.QuestionnaireQuestions.Count(); i++)
            {
                
                var qqId = qqList[i].Id;
                var qqCatId = qqList.Single(x => x.Id == qqId).QQCategoryId;
                if (qqCatId != null)
                {
                    var qqCId = (int)qqCatId;

                    if (lockedSections.Contains(qqCId)) continue;
                }

                var responseItem = _db.Responses.Any(a => a.UserId == userId && a.QuestionnaireQuestionId == qqId) ? _db.Responses.Single(a => a.UserId == userId && a.QuestionnaireQuestionId == qqId).ResponseItem : "";

                var answers = qqList[i].Question.QType.Answers;

                var qCategoryName = qqList[i].QuestionnaireQCategory.QCategory.QCategoryName;
                if (qqList[i].QuestionnaireQCategory.QCategory.QCategoryName != "Personal Information" && qqList[i].QuestionnaireQCategory.SubOrdinal > 0)
                    qCategoryName += " (" + (qqList[i].QuestionnaireQCategory.SubOrdinal + 1) + ")";

                appQuestions.Add(new Response
                {
                    QuestionId = (int)qqList[i].QuestionId,
                    QuestionText = qqList[i].Question.QuestionText,
                    QTitle = qqList[i].Question.QTitle,
                    QTypeResponse = qqList[i].Question.QType.QTypeResponse,
                    QuestionnaireId = (int)qqList[i].QuestionnaireId,
                    QCategoryId = (int)qqList[i].QuestionnaireQCategory.QCategoryId,
                    QCategoryName = qCategoryName,
                    QuestionnaireQuestionId = qqList[i].Id,
                    QuestionnaireQCategoryId = (int)qqList[i].QQCategoryId,
                    QQOrd = qqList[i].Ordinal,
                    Ordinal = qqList[i].QuestionnaireQCategory.Ordinal,
                    SubOrdinal = qqList[i].QuestionnaireQCategory.SubOrdinal,
                    UserId = userId,
                    ResponseItem = responseItem,   
                    Answers = answers
                });
            }

            var returnList = appQuestions.OrderBy(x => x.Ordinal).ThenBy(x => x.SubOrdinal).ThenBy(x => x.QQOrd).ToList();
            viewModel.Responses = returnList;

            var qCategories = _db.QCategories.ToList();
            viewModel.QCategories = qCategories;

            return View(viewModel);
        }

        //
        // POST: /Response/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IList<Response> model, FormCollection formCollection)
        {
            var qqcIds = model.Select(x => x.QuestionnaireQCategoryId).Distinct().ToList();

            
            var scope = new TransactionScope(
                // a new transaction will always be created
                TransactionScopeOption.RequiresNew,
                // we will allow volatile data to be read during transaction
                new TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted
                }
            );

            if (ModelState.IsValid)
            {
                using (scope)
                {
                    try
                    {
                        foreach (var record in _db.Responses)
                        {
                            if (record.UserId == model[0].UserId && qqcIds.Contains(record.QuestionnaireQCategoryId))
                                _db.Responses.Remove(record);
                        }
                        _db.SaveChanges();

                        foreach (var r in model)
                        {
                            Response response = r;
                            _db.Responses.Add(response);
                            _db.SaveChanges();
                            var check = _db.Responses.Single(x => x.ResponseId == response.ResponseId);
                        }
                        scope.Complete();
                        return RedirectToAction("Edit", "Response", new { area="", id = 1 });
                    }
                    catch { }
                }
                //db.Entry(Response).State = EntityState.Modified;                
            }
            return View();
        }

        //
        // GET: /Response/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Response response = _db.Responses.Find(id);
            if (response == null)
            {
                return HttpNotFound();
            }
            return View(response);
        }

        //
        // POST: /Response/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Response response = _db.Responses.Find(id);
            _db.Responses.Remove(response);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult AjaxTest()
        {
            var testString = "Hi there";
            return PartialView("_AjaxTest", testString);
        }
    }
}