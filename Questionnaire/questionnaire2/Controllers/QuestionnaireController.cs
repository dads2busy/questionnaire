using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Questionnaire2.DAL;
using Questionnaire2.Models;
using Questionnaire2.ViewModels;

namespace Questionnaire2.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class QuestionnaireController : Controller
    {
        private readonly QuestionnaireContext _db = new QuestionnaireContext();

        //
        // GET: /Questionnaire/

        public ActionResult Index(int? id)
        {
            var viewModel = new QuestionnaireIndexData
            {
                Questionnaires = _db.Questionnaires.Include(x => x.QuestionnaireQuestions)
            };

            if (id == null) return View(viewModel);

            Questionnaire q = viewModel.Questionnaires.Single(n => n.QuestionnaireId == id);
            ICollection<QuestionnaireQuestion> questionnaireQuestions = q.QuestionnaireQuestions;
            IEnumerable<int> questionIds = from i in questionnaireQuestions
                let questionId = i.QuestionId
                where questionId != null
                select (int) questionId;
            viewModel.Questions = _db.Questions.Where(n => questionIds.Contains(n.QuestionId));

            ViewBag.QuestionnaireId = id.Value;

            return View(viewModel);
        }

        //
        // GET: /Questionnaire/Details/5

        public ActionResult Details(int id = 0)
        {
            var viewModel = new QuestionnaireDetailData();

            /* Add Fully Loaded (.Included) Questionnaire to the ViewModel */
            viewModel.Questionnaire =
                _db.Questionnaires
                    .Include(a => a.QuestionnaireQuestions
                        .Select(b => b.Question.QType))
                    .Include(a => a.QuestionnaireQuestions
                        .Select(b => b.QuestionnaireQCategory))
                    .Include(a => a.QuestionnaireQCategories
                        .Select(b => b.QCategory))
                    .Where(n => n.QuestionnaireId == id)
                    .Single();

            /* Sort by Questionnaire Question Categories: Step 1 - Left Join QuestionnaireQuestions to QuestionnaireQCategories on QCategoryId */
            var query = from questionnaireQuestions in viewModel.Questionnaire.QuestionnaireQuestions
                join questionnaireQCategories in viewModel.Questionnaire.QuestionnaireQCategories
                    on questionnaireQuestions.QuestionnaireQCategory.QCategoryId equals
                    questionnaireQCategories.QCategoryId
                    into qqJoinQqc
                from subset in qqJoinQqc.DefaultIfEmpty()
                select new
                {
                    questionnaireQuestions.Id,
                    questionnaireQuestions.QuestionnaireId,
                    questionnaireQuestions.QuestionId,
                    questionnaireQuestions.QuestionnaireQCategory.QCategoryId,
                    questionnaireQuestions.Ordinal,
                    questionnaireQuestions.Question,
                    questionnaireQuestions.QuestionnaireQCategory.QCategory,
                    QQCOrdinal = (subset == null ? 99 : subset.Ordinal)
                };
            /* Sort by Questionnaire Question Categories: Step 2 - Sort on Ordinal from QuestionnaireQCategories */
            var sortedQuery = query.OrderBy(q => q.QQCOrdinal);
            /* Sort by Questionnaire Question Categories: Step 3 - Build List<QuestionnaireQuestion> (because QuestionnaireQuestions is an ICollection) */
            var sortedQuestionnaireQuestions = new List<QuestionnaireQuestion>();
            foreach (var item in sortedQuery)
            {
                var questionnaireQuestion = new QuestionnaireQuestion
                {
                    Id = item.Id,
                    Ordinal = item.Ordinal,
                    QuestionnaireId = item.QuestionnaireId,
                    QuestionId = item.QuestionId,
                    QuestionnaireQCategory = {QCategoryId = item.QCategoryId},
                    Question = item.Question
                };
                questionnaireQuestion.QuestionnaireQCategory.QCategory = item.QCategory;
                sortedQuestionnaireQuestions.Add(questionnaireQuestion);
            }
            /* Sort by Questionnaire Question Categories: Step 4 - Replace Questionnaire.QuestionnaireQuestions with Sorted List */
            viewModel.Questionnaire.QuestionnaireQuestions = sortedQuestionnaireQuestions;

            viewModel.Questionnaire.QuestionnaireQCategories =
                viewModel.Questionnaire.QuestionnaireQCategories.OrderBy(o => o.Ordinal).ToList();

            var detailRecords = new List<QuestionnaireDetailRecord>();
            foreach (QuestionnaireQuestion qq in viewModel.Questionnaire.QuestionnaireQuestions)
            {
                List<Answer> answers = _db.Answers.Where(x => x.QTypeName == qq.Question.QTypeName).ToList();
                var selectListItems = new List<SelectListItem>();

                for (int i = 0; i < answers.Count(); i++)
                {
                    var selectListItem = new SelectListItem
                    {
                        Text = answers[i].AnswerText,
                        Value = answers[i].AnswerId.ToString()
                    };
                    selectListItems.Add(selectListItem);
                }
                var selectList = new SelectList(selectListItems, "Value", "Text");

                detailRecords.Add(new QuestionnaireDetailRecord
                {
                    Question = qq.Question,
                    QCategory = qq.QuestionnaireQCategory.QCategory,
                    Answers = selectList
                });
            }
            viewModel.DetailRecords = detailRecords;

            return View(viewModel);
        }

        //
        // GET: /Questionnaire/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Questionnaire/Create

        [HttpPost]
        public ActionResult Create(Questionnaire questionnaire)
        {
            if (ModelState.IsValid)
            {
                _db.Questionnaires.Add(questionnaire);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(questionnaire);
        }

        //
        // GET: /Questionnaire/Edit/5

        public ActionResult Edit(int id = 1, int err = 0)
        {
            if (err != 0)
                ViewBag.message = "The section is still being used and cannot be deleted.";
            else
                ViewBag.message = "";
            
            var viewModel = new QuestionnaireEditData();

            List<SelectListItem> qTypeNames = _db.QTypes
                .ToList()
                .Select(q => new SelectListItem
                {
                    Value = q.QTypeName,
                    Text = q.QTypeName
                }).OrderBy(o => o.Text).ToList();

            viewModel.QTypeNames = new SelectList(qTypeNames, "Value", "Text");

            /* Add Fully Loaded (.Included) Questionnaire to the ViewModel */

            viewModel.Questionnaire =
                _db.Questionnaires
                    .Include(a => a.QuestionnaireQuestions
                        .Select(b => b.Question))
                    .Include(a => a.QuestionnaireQuestions
                        .Select(b => b.QuestionnaireQCategory).Select(c => c.QCategory))
                    .Include(a => a.QuestionnaireQCategories)
                    .Where(n => n.QuestionnaireId == id)
                    .Single();

            viewModel.Questionnaire.QuestionnaireQuestions =
                viewModel.Questionnaire.QuestionnaireQuestions.Where(x => x.UserId == 0).ToList();
            viewModel.Questionnaire.QuestionnaireQCategories =
                viewModel.Questionnaire.QuestionnaireQCategories.Where(x => x.UserId == 0).ToList();

            ICollection<QuestionnaireQuestion> sortQQs = viewModel.Questionnaire.QuestionnaireQuestions;

            viewModel.Questionnaire.QuestionnaireQuestions =
                sortQQs.OrderBy(x => x.QuestionnaireQCategory.Ordinal).ThenBy(x => x.Ordinal).ToList();
            
            /* Build Questionnaire Question DropDownList and add to ViewModel */
            List<SelectListItem> allQuestions = _db.Questions
                .ToList()
                .Select(q => new SelectListItem
                {
                    Value = q.QuestionId.ToString(CultureInfo.InvariantCulture),
                    Text = q.QuestionText
                }).ToList();

            viewModel.QuestionDropDownLists = new List<KeyValuePair<int, SelectList>>();
            var newSelectList = new SelectList(allQuestions, "Value", "Text");
            viewModel.QuestionDropDownLists.Add(new KeyValuePair<int, SelectList>(-1, newSelectList));

            foreach (QuestionnaireQuestion QQ in viewModel.Questionnaire.QuestionnaireQuestions)
            {
                newSelectList = new SelectList(allQuestions, "Value", "Text", QQ.QuestionId);
                viewModel.QuestionDropDownLists.Add(new KeyValuePair<int, SelectList>(QQ.Id, newSelectList));
            }

            /* Build Questionnaire Question Category DropDownList and add to ViewModel */
            //var allQCategories = _db.QCategories
            List<SelectListItem> allQCategories = _db.QuestionnaireQCategories.Where(x => x.UserId == 0)
                .ToList()
                .Select(q => new SelectListItem
                {
                    //Value = q.QCategoryId.ToString(CultureInfo.InvariantCulture),
                    Value = q.Id.ToString(CultureInfo.InvariantCulture),
                    //Text = q.QCategoryName
                    Text = _db.QCategories.Single(x => x.QCategoryId == q.QCategoryId).QCategoryName
                }).OrderBy(o => o.Text).ToList();

            viewModel.QCategoryDropDownLists = new List<KeyValuePair<int, SelectList>>();
            newSelectList = new SelectList(allQCategories, "Value", "Text");
            viewModel.QCategoryDropDownLists.Add(new KeyValuePair<int, SelectList>(-1, newSelectList));
            foreach (QuestionnaireQuestion QQ in viewModel.Questionnaire.QuestionnaireQuestions)
            {
                newSelectList = new SelectList(allQCategories, "Value", "Text", QQ.QQCategoryId);
                viewModel.QCategoryDropDownLists.Add(new KeyValuePair<int, SelectList>(QQ.Id, newSelectList));
            }

            /* Build Questionnaire Question Ordinal DropDownList and add to ViewModel */
            List<SelectListItem> allOrdinals = new List<SelectListItem>();
            for (int i = 1; i < 101; i++ )
            {
                var item = new SelectListItem();
                item.Value = i.ToString();
                item.Text = i.ToString();
                allOrdinals.Add(item);
            }
            viewModel.OrdinalDropDownLists = new List<KeyValuePair<int, SelectList>>();
            newSelectList = new SelectList(allOrdinals, "Value", "Text");
            viewModel.OrdinalDropDownLists.Add(new KeyValuePair<int, SelectList>(-1, newSelectList));
            foreach (QuestionnaireQuestion QQ in viewModel.Questionnaire.QuestionnaireQuestions)
            {
                newSelectList = new SelectList(allOrdinals, "Value", "Text", QQ.Ordinal);
                viewModel.OrdinalDropDownLists.Add(new KeyValuePair<int, SelectList>(QQ.Id, newSelectList));
            }

            /* Get Sections Assigned to this Questionnaire*/
            List<KeyValuePair<int, string>> questionnaireSections = _db.QuestionnaireQCategories.Where(x => x.UserId == 0 && x.QuestionnaireId == 1)
                .ToList()
                .Select(q => new KeyValuePair<int, string>(q.Id, q.QCategory.QCategoryName))
                .OrderBy(o => o.Value)
                .ToList();

            viewModel.QuestionnaireSections = questionnaireSections;

            /* Get all Sections*/
            List<SelectListItem> allSections = _db.QCategories
                .ToList()
                .Select(q => new SelectListItem
                {
                    Value = q.QCategoryId.ToString(CultureInfo.InvariantCulture),
                    Text = q.QCategoryName
                }).OrderBy(o => o.Text).ToList();

            allSections.Add(new SelectListItem(){ Value="-1", Text=""});
            viewModel.Sections = allSections;


            ViewBag.QuestionnaireId = id;

            if (viewModel.Questionnaire == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        //
        // POST: /Questionnaire/Edit/5

        [HttpPost]
        public ActionResult Edit(IEnumerable<QuestionnaireQuestion> qQuestions, int[] remove,
            int selectedQuestionAdd = -1, int selectedQQCategoryAdd = -1)
        {
            if (remove == null)
                remove = new int[0];

            QuestionnaireQuestion[] enumerable = qQuestions as QuestionnaireQuestion[] ?? qQuestions.ToArray();
            int? id = enumerable[0].QuestionnaireId;

            if (id == null) return View();

            var questionnaireId = (int)id;

            var scope = new TransactionScope(
                // a new transaction will always be created
                TransactionScopeOption.RequiresNew,
                // we will allow volatile data to be read during transaction
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted
                }
                );

            using (scope)
            {
                try
                {
                    var qqnum = enumerable.Where(x => remove.Contains(x.Id));
                        
                    // updates
                    foreach (QuestionnaireQuestion qQuestion in enumerable)
                    {
                        if (!remove.Contains(qQuestion.Id))
                        {
                            _db.Entry(qQuestion).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    _db.SaveChanges();

                    // deletes
                    foreach (var i in remove)
                    {
                        var qq = _db.QuestionnaireQuestions.Single(x => x.Id == i);
                        _db.Entry(qq).State = System.Data.Entity.EntityState.Deleted;
                    }
                    _db.SaveChanges();

                    // new questionnaire question
                    if (selectedQuestionAdd != -1 && selectedQQCategoryAdd != -1)
                    {
                        int ordinal = 1;
                        if (_db.QuestionnaireQuestions.Any(x => x.QuestionnaireId == id && x.QQCategoryId == selectedQQCategoryAdd))
                        {
                            ordinal =
                                _db.QuestionnaireQuestions.Where(
                                    x => x.QuestionnaireId == id && x.QQCategoryId == selectedQQCategoryAdd)
                                    .OrderBy(x => x.Ordinal)
                                    .ToList()
                                    .Last()
                                    .Ordinal + 1;
                        }
                            
                        var qq = new QuestionnaireQuestion
                        {
                            QuestionnaireId = id,
                            QuestionId = selectedQuestionAdd,
                            QQCategoryId = selectedQQCategoryAdd,
                            Ordinal = ordinal
                        };

                        _db.Entry(qq).State = System.Data.Entity.EntityState.Added;
                    }
                    _db.SaveChanges();

                    scope.Complete();
                    return RedirectToAction("Edit");
                }
                catch (Exception ex)
                {
                    throw;
                }
            }


            return View();
        }


        //
        // POST: /Questionnaire/EditSections/

        [HttpPost]
        public ActionResult EditSections(int[] remove, int selectedSectionAdd = -1)
        {
            if (remove == null)
                remove = new int[0];

            int? id = 1;

            if (id == null) return View();

            var questionnaireId = (int)id;

            var scope = new TransactionScope(
                // a new transaction will always be created
                TransactionScopeOption.RequiresNew,
                // we will allow volatile data to be read during transaction
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted
                }
                );

            using (scope)
            {
                try
                {
                    // deletes
                    foreach (int i in remove)
                    {
                        var qSectionDelete = _db.QuestionnaireQCategories.Where(x => x.Id == i).FirstOrDefault();
                        _db.Entry(qSectionDelete).State = System.Data.Entity.EntityState.Deleted;
                    }
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Edit", null, new { id, err = 1 });
                }

                try
                {
                    // new questionnaire section
                    var ordinal =
                            _db.QuestionnaireQCategories.Where(
                                x => x.QuestionnaireId == id && x.UserId == 0)
                                .OrderBy(x => x.Ordinal)
                                .ToList()
                                .Last()
                                .Ordinal + 1;

                    if (selectedSectionAdd != -1)
                    {
                        var qSectionAdd = new QuestionnaireQCategory
                        {
                            QuestionnaireId = id,
                            UserId = 0,
                            QCategoryId = selectedSectionAdd,
                            Ordinal = ordinal
                        };

                        _db.Entry(qSectionAdd).State = System.Data.Entity.EntityState.Added;
                    }
                    _db.SaveChanges();
                }
                catch
                {
                    throw;
                }
                    scope.Complete();
                    return RedirectToAction("Edit");
                
            }


            return View();
        }


        //
        // POST: /Questionnaire/EditName/

        [HttpPost]
        public ActionResult EditName(FormCollection formCollection)
        {
            var questionnaireId = Int32.Parse(formCollection["Questionnaire.QuestionnaireId"]);
            var questionnaireName = formCollection["Questionnaire.QuestionnaireName"];
            var questionnaire = _db.Questionnaires.Single(x => x.QuestionnaireId == questionnaireId);
            questionnaire.QuestionnaireName = questionnaireName;
            _db.Entry(questionnaire).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Edit");
        }

        //
        // GET: /Questionnaire/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Questionnaire questionnaire = _db.Questionnaires.Find(id);
            if (questionnaire == null)
            {
                return HttpNotFound();
            }
            return View(questionnaire);
        }

        //
        // POST: /Questionnaire/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Questionnaire questionnaire = _db.Questionnaires.Find(id);
            _db.Questionnaires.Remove(questionnaire);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult NewQuestion()
        {
            return RedirectToAction("Index", "Questionnaire", new {area = ""});
        }

        [HttpPost]
        public ActionResult CreateQuestion(Questionnaire questionnaire, QuestionnaireEditData model,
            string selectedQType, int id = 0)
        {
            if (ModelState.IsValid)
            {
                model.Question.QTypeName = selectedQType;

                _db.Questions.Add(model.Question);
                _db.SaveChanges();
                return RedirectToAction("Edit", "Questionnaire", new {id});
            }

            return View(model);
        }
    }
}