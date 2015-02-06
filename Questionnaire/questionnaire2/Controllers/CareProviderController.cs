using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Questionnaire2.DAL;
using Questionnaire2.ViewModels;
using WebMatrix.WebData;

namespace Questionnaire2.Controllers
{
    public class CareProviderController : Controller
    {
        private readonly QuestionnaireContext _db = new QuestionnaireContext();

        public ActionResult Status(int questionnaireId = 1)
        {
            var id = WebSecurity.GetUserId(User.Identity.Name);

            var vmVerificationItems = new VmVerificationItems { VerificationItems = new Collection<VmVerificationItem>() };

            var userVerificationRecords = _db.Verifications.Where(x => x.UserId == id && x.QuestionnaireId == questionnaireId).ToList();

            var latticeItems = _db.LatticeItems.ToList();
            var selectListItems = latticeItems.Select(latticeItem => new SelectListItem
            {
                Text = latticeItem.DropdownText,
                Value = latticeItem.DropdownText
            }).ToList();
            vmVerificationItems.LatticeItems = selectListItems;

            foreach (var userVerificationRecord in userVerificationRecords)
            {
                var record = userVerificationRecord;
                var vmVerificationItem = new VmVerificationItem
                {
                    Verification = record,
                    Files = _db.Files.Where(
                        x =>
                            x.UserId == id && x.QuestionnaireId == questionnaireId &&
                            x.QuestionnaireQCategoryId == record.QCategoryId &&
                            x.QCategorySubOrdinal == record.SubOrdinal).ToList()
                };

                vmVerificationItems.VerificationItems.Add(vmVerificationItem);
            }

            return View(vmVerificationItems);
        }
        
        //
        // GET: /CareProvider/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /CareProvider/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /CareProvider/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /CareProvider/Create

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
        // GET: /CareProvider/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /CareProvider/Edit/5

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
        // GET: /CareProvider/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /CareProvider/Delete/5

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
