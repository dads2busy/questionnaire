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
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.IO;
using iTextSharp.text;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using System.Net.Http;

namespace Questionnaire2.Controllers
{
       
    public class VerificationController : Controller
    {
        private readonly QuestionnaireContext _db = new QuestionnaireContext();
        private readonly UsersContext _udb = new UsersContext();

        public ActionResult Index()
        {
            var users = new List<UserInfo>();
            var userIds = _db.Verifications.Select(x => x.UserId).Distinct().ToList();
            for (var i = 0; i < userIds.Count(); i++)
            {
                var userInfo = new UserInfo {UserId = userIds[i]};
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

                users.Add(userInfo);
            }

            var usersVerified = users.Where(x => x.UnverifiedCount == 0);
            var usersUnverified = users.Where(x => x.UnverifiedCount != 0);

            var userVerifications = new UserVerifications();
            userVerifications.UsersVerified = usersVerified.ToList();
            userVerifications.UsersUnverified = usersUnverified.ToList();

            return View(userVerifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(RegisterExternalLoginModel mReg, string Command, int id = 0)
        {

            var users = new List<UserInfo>();
            var userIds = _db.Verifications.Select(x => x.UserId).Distinct().ToList();
            for (var i = 0; i < userIds.Count(); i++)
            {
                var userInfo = new UserInfo { UserId = userIds[i] };
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

                users.Add(userInfo);
            }

            var usersVerified = users.Where(x => x.UnverifiedCount == 0);
            var usersUnverified = users.Where(x => x.UnverifiedCount != 0);

            var userVerifications = new UserVerifications();
            userVerifications.UsersVerified = usersVerified.ToList();
            userVerifications.UsersUnverified = usersUnverified.ToList();

            var tableHtml = "<html><head></head><body><table>";
            tableHtml += "<tr><td colspan=4><h1>User Verification Status</h1></td></tr>";
            tableHtml += "<tr><td colspan=4><h2>Unverified Users</h2></td></tr>";
            tableHtml += "<tr><th>First Name</th><th>Last Name</th><th>Username</th><th>Status</th></tr>";
            foreach (var user in usersUnverified)
            {
                tableHtml += "<tr><td>" + user.FirstName + "</td>";
                tableHtml += "<tr><td>" + user.LastName + "</td>";
                tableHtml += "<tr><td>" + user.UserName + "</td>";
                tableHtml += "<tr><td>" + user.VerifiedCount + "/" + user.UnverifiedCount  + "</td>";
            }
            tableHtml += "<tr><td colspan=4><h2>Verified Users</h2></td></tr>";
            tableHtml += "<tr><th>First Name</th><th>Last Name</th><th>Username</th><th>Status</th></tr>";
            foreach (var user in usersVerified)
            {
                tableHtml += "<tr><td>" + user.FirstName + "</td>";
                tableHtml += "<tr><td>" + user.LastName + "</td>";
                tableHtml += "<tr><td>" + user.UserName + "</td>";
                tableHtml += "<tr><td>" + user.VerifiedCount + "/" + user.UnverifiedCount + "</td>";
            }
            tableHtml += "</table></body></html>";

            tableToPdf(this, new EventArgs(), tableHtml);
            return RedirectToAction("Index");
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
                        _db.Entry(verification).State = (EntityState)System.Data.Entity.EntityState.Modified;
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
                _db.Entry(item.Verification).State = (EntityState) System.Data.Entity.EntityState.Modified;
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

        public void tableToPdf(object sender, EventArgs e, string pageHtml)
        {
            //var table = document.GetElementbyId("verificationsTable");
            
            //Set page size as A4
            Document pdfDoc = new Document(PageSize.A4, 20, 10, 10, 10);

            try
            {
                var ms = new MemoryStream();
                PdfWriter.GetInstance(pdfDoc, ms);

                //Open PDF Document to write data
                pdfDoc.Open();

                ////Assign Html content in a string to write in PDF
                //string contents = "";

                //StreamReader sr;
                //try
                //{
                //    //Read file from server path
                //    sr = System.IO.File.OpenText(Server.MapPath("~/sample.html"));
                //    //store content in the variable
                //    contents = sr.ReadToEnd();
                //    sr.Close();
                //}
                //catch (Exception ex)
                //{

                //}

                //Read string contents using stream reader and convert html to parsed conent
                var parsedHtmlElements = HTMLWorker.ParseToList(new StringReader(pageHtml), null);

                //Get each array values from parsed elements and add to the PDF document
                foreach (var htmlElement in parsedHtmlElements)
                    pdfDoc.Add(htmlElement as IElement);

                //Close your PDF
                pdfDoc.Close();

                var ms2 = new MemoryStream(ms.ToArray());

                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                //Set default file Name as current datetime
                Response.AddHeader("content-disposition", "attachment; filename=" + DateTime.Now.ToString("yyyyMMdd") + ".pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                ms2.WriteTo(Response.OutputStream);
                Response.End();

            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
        }

        public string GetHtmlPage(string strURL)
        {
            String strResult;
            WebRequest objRequest = WebRequest.Create(strURL);
            WebResponse objResponse = objRequest.GetResponse();
            using (var sr = new StreamReader(objResponse.GetResponseStream()))
            {
                strResult = sr.ReadToEnd();
                sr.Close();
            }
            return strResult;
        }     
    }
}
