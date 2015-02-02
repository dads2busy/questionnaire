using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Web.Routing;
using Questionnaire2.DAL;
using Questionnaire2.Models;
using Questionnaire2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Questionnaire2.Controllers
{
       
    public class VerificationController : Controller
    {
        private readonly QuestionnaireContext _db = new QuestionnaireContext();
        private readonly UsersContext _udb = new UsersContext();

        public ActionResult Index()
        {
            var unverifiedUsers = new List<UserInfo>();
            var unverifiedIds = _db.Verifications.Where(x => x.ItemVerified == false).Select(x => x.UserId).Distinct().ToList();
            for (var i = 0; i < unverifiedIds.Count(); i++)
            {
                var userInfo = new UserInfo {UserId = unverifiedIds[i]};
                userInfo.VerifiedCount = _db.Verifications.Count(x => x.UserId == userInfo.UserId && x.QuestionnaireId == 1 && x.ItemVerified);
                userInfo.UnverifiedCount = _db.Verifications.Count(x => x.UserId == userInfo.UserId && x.QuestionnaireId == 1 && x.ItemVerified == false);

                userInfo.Editable = !_db.Verifications.Any(
                    x => x.UserId == userInfo.UserId && x.QuestionnaireId == 1 && x.Editable == false);

                var firstOrDefault = _udb.UserProfiles.FirstOrDefault(x => x.UserId == userInfo.UserId);
                if (firstOrDefault != null)
                    userInfo.UserName = firstOrDefault.UserName;
                var responses = _db.Responses.Where(x => x.UserId == userInfo.UserId && x.QCategoryName.ToUpper().Contains("PERSONAL"));

                var orDefault = responses.FirstOrDefault(x => x.QuestionText.ToUpper().Contains("FIRST NAME"));
                if (orDefault != null)
                    userInfo.FirstName = orDefault.ResponseItem;
                var response = responses.FirstOrDefault(x => x.QuestionText.ToUpper().Contains("LAST NAME"));
                if (response != null)
                    userInfo.LastName = response.ResponseItem;

                unverifiedUsers.Add(userInfo);
            }

            return View(unverifiedUsers);
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Verification()
        {
            var userId = WebSecurity.GetUserId(User.Identity.Name);
            var hasVerifications = _db.Verifications.Any(x => x.UserId == userId);
            return View(hasVerifications);
        }

        public ActionResult SendToVerification(int id)
        {
            var userId = WebSecurity.GetUserId(User.Identity.Name);

            // Delete existing verification items for this user
            //var userItems = _db.Verifications.Where(x => x.UserId == userId);
            //var removeRange = _db.Verifications.RemoveRange(userItems);
            //var saveChanges = _db.SaveChanges();

            var lockedSections =
                _db.Verifications.Where(x => x.UserId == userId && x.Editable == false).Select(x => x.QQCategoryId).ToList();

            var applicationDataToVerify = _db.Responses.Where(x => x.UserId == userId && x.QuestionnaireId == 1 && !lockedSections.Contains(x.QuestionnaireQCategoryId)).ToList();

            var distinctQCategoryIds = applicationDataToVerify.Select(x => x.QCategoryId).Distinct();

            foreach (var qCategoryId in distinctQCategoryIds)
            {
                var distinctSubOrdinals = applicationDataToVerify.Where(x => x.QCategoryId == qCategoryId).Select(x => x.SubOrdinal).Distinct().ToList();
                for (var i=0; i < distinctSubOrdinals.Count(); i++)
                {
                    var subOrdinal = distinctSubOrdinals[i];
                    var questionnaireId = applicationDataToVerify[i].QuestionnaireId;

                    var categoryId = qCategoryId;
                    var categoryName = _db.QCategories.Single(x => x.QCategoryId == categoryId).QCategoryName;
                    var qqCategoryId =
                        applicationDataToVerify.First(x => x.QCategoryId == qCategoryId && x.SubOrdinal == subOrdinal)
                            .QuestionnaireQCategoryId;

                    var subOrdinalQuestions = applicationDataToVerify.Where(x => x.QCategoryId == categoryId && x.SubOrdinal == subOrdinal).Select(x => new { x.QuestionText, x.ResponseItem });
                    var itemInfo = "<b>" + categoryName.ToUpper() + "</b><br />";
                    itemInfo += subOrdinalQuestions.Aggregate("", (current, item) => current + ("<i>" + item.QuestionText + ":</i> " + item.ResponseItem + "<br />"));

                    if (_db.Verifications.Any(x => x.QQCategoryId == qqCategoryId))
                    {
                        //update
                        var verification = _db.Verifications.Single(x => x.QQCategoryId == qqCategoryId);
                        verification.ItemInfo = itemInfo;
                        verification.Editable = false;
                        _db.Entry(verification).State = (EntityState)System.Data.EntityState.Modified;
                    }
                    else
                    {                      
                        // make new
                        var verifyQCategory = new Verification
                        {
                            QuestionnaireId = questionnaireId,
                            UserId = userId,
                            QCategoryId = qCategoryId,
                            QQCategoryId = qqCategoryId,
                            SubOrdinal = subOrdinal,
                            ItemInfo = itemInfo,
                            ItemVerified = false,
                            ItemStepLevel = "",
                            Editable = false
                        };
                        _db.Verifications.Add(verifyQCategory);
                    }                                   
                }
            }
            _db.SaveChanges();
            return View();
        }

        public ActionResult List(int id, int questionnaireId = 1)
        {
            var vmVerificationItems = new VmVerificationItems {VerificationItems = new Collection<VmVerificationItem>()};

            var userVerificationRecords = _db.Verifications.Where(x => x.UserId == id && x.QuestionnaireId == questionnaireId).ToList();

            var latticeItems = _db.LatticeItems.ToList();
            var selectListItems = latticeItems.Select(latticeItem => new SelectListItem
            {
                Text = latticeItem.DropdownText, Value = latticeItem.DropdownText
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

        public ActionResult Edit(int id)
        {
            var vmVerificationItems = new VmVerificationItems { VerificationItems = new Collection<VmVerificationItem>() };

            var userVerificationRecords = _db.Verifications.Where(x => x.Id == id).ToList();

            var questionnaireId = userVerificationRecords.First().QuestionnaireId;

            var userId = WebSecurity.GetUserId(User.Identity.Name);

            var latticeItems = _db.LatticeItems.ToList();
            var selectListItems = new List<SelectListItem>();
            foreach (var latticeItem in latticeItems)
            {
                var selectListItem = new SelectListItem
                {
                    Text = latticeItem.DropdownText,
                    Value = latticeItem.DropdownText
                };
                selectListItems.Add(selectListItem);
            }
            vmVerificationItems.LatticeItems = selectListItems;

            foreach (var userVerificationRecord in userVerificationRecords)
            {
                var record = userVerificationRecord;
                var vmVerificationItem = new VmVerificationItem
                {
                    Verification = record,
                    Files = _db.Files.Where(
                        x =>
                            x.UserId == userId && x.QuestionnaireId == questionnaireId &&
                            x.QuestionnaireQCategoryId == record.QCategoryId &&
                            x.QCategorySubOrdinal == record.SubOrdinal).ToList()
                };

                vmVerificationItems.VerificationItems.Add(vmVerificationItem);
            }

            return View(vmVerificationItems);
        }

        [HttpPost]
        public ActionResult Edit(VmVerificationItem item)
        {
            try
            {
                // TODO: Add update logic here
                _db.Entry(item.Verification).State = (EntityState) System.Data.EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("List", new { id = item.Verification.UserId });
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DownloadFile(int id, int vId)
        {
            var fileRecord = _db.Files.First(p => p.FileId == id & p.UserId == WebSecurity.CurrentUserId);
            byte[] fileData = fileRecord.FileBytes;

            String mimeType = null;

            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.ContentType = mimeType;
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=" + fileRecord.FileName));
            Response.BinaryWrite(fileData);
            Response.End();
            return RedirectToAction("Edit", new { id = vId });
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

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

        public ActionResult LockUnlock(int id, int questionnaireId, bool editable)
        {
            var verificationItems = _db.Verifications.Where(x => x.UserId == id && x.QuestionnaireId == questionnaireId);
            if (editable == true)
            {
                foreach (var verificationItem in verificationItems)
                {
                    verificationItem.Editable = true;
                }
            }
            else
            {
                foreach (var verificationItem in verificationItems)
                {
                    verificationItem.Editable = false;
                }
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
