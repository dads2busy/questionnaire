using System.Globalization;
using Questionnaire2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Questionnaire2.Helpers
{
    public class FormatUserInformation
    {
        public class Paragraph
        {
            public string Text { get; set; }
            public Novacode.Formatting Format { get; set; }
            public Novacode.Alignment Align { get; set; }
        }
        
        private List<Response> Items { get; set; }
        private List<string> CategoryNames { get; set; }
        private readonly Novacode.Formatting _title = new Novacode.Formatting()
        {
            FontFamily = new System.Drawing.FontFamily("Arial Black"),
            Size = 16D,
            Bold = true
        };
        private readonly Novacode.Formatting _subtitle = new Novacode.Formatting()
        {
            FontFamily = new System.Drawing.FontFamily("Calibri"),
            Size = 12D,
            Bold = true
        };
        private readonly Novacode.Formatting _normal = new Novacode.Formatting()
        {
            FontFamily = new System.Drawing.FontFamily("Calibri"),
            Size = 10D,
            Bold = false
        };

        private const Novacode.Alignment Center = Novacode.Alignment.center;
        private const Novacode.Alignment Left = Novacode.Alignment.left;

        public FormatUserInformation(List<Response> items, List<string> categoryNames)
        {
            this.Items = items;
            this.CategoryNames = categoryNames;
        }


        public List<Paragraph> Format()
        {
            var combinedList = new List<Paragraph>();

            foreach (string catName in CategoryNames)
            {
                var formatItems = Items.Where(x => x.QCategoryName.Contains(catName) == true).ToList();

                switch (catName)
                {
                    case "Personal Information":
                        combinedList = combinedList.Concat(PersonalInformation(formatItems)).ToList();
                        break;
                    case "Employment":
                        combinedList = combinedList.Concat(EmploymentInformation(formatItems)).ToList();
                        break;
                    case "Certifications":
                        combinedList = combinedList.Concat(CertificationInformation(formatItems)).ToList();
                        break;
                    case "Licenses":
                        combinedList = combinedList.Concat(LicenseInformation(formatItems)).ToList();
                        break;
                    case "Education":
                        combinedList = combinedList.Concat(EducationInformation(formatItems)).ToList();
                        break;
                    case "Credentials":
                        combinedList = combinedList.Concat(CredentialEndorsementInformation(formatItems)).ToList();
                        break;
                    case "Coursework":
                        combinedList = combinedList.Concat(OtherCourseworkInformation(formatItems)).ToList();
                        break;
                    case "Training":
                        combinedList = combinedList.Concat(ProfessionalTrainingInformation(formatItems)).ToList();
                        break;
                        
                    default:
                        return null;
                }                
            }
            return combinedList;
        }


        protected List<Paragraph> PersonalInformation(List<Response> pi)
        {
            var sections = new List<Paragraph>();

            if (!pi.Any()) return sections;

            var notfound = new List<Response> {new Response() {ResponseItem = "Not Found"}};

            var response1 = pi.Any(x => x.QuestionText == "First Name") ? pi.Where(x => x.QuestionText == "First Name") : notfound;
            var enumerable1 = response1 as IList<Response> ?? response1.ToList();
            var item1 = enumerable1.Single().ResponseItem ?? "test string";

            var response2 = pi.Any(x => x.QuestionText == "Middle Initial") ? pi.Where(x => x.QuestionText == "Middle Initial") : notfound;
            var enumerable2 = response2 as IList<Response> ?? response2.ToList();
            var item2 = enumerable2.Single().ResponseItem ?? "test string";

            var response3 = pi.Any(x => x.QuestionText == "Last Name") ? pi.Where(x => x.QuestionText == "Last Name") : notfound;
            var enumerable3 = response3 as IList<Response> ?? response3.ToList();
            var item3 = enumerable3.Single().ResponseItem ?? "test string";
            sections.Add(new Paragraph() { Text = item1 + " " + item2 + " " + item3, Format = _title, Align = Center });

            var response4 = pi.Any(x => x.QuestionText == "Email Address") ? pi.Where(x => x.QuestionText == "Email Address") : notfound;
            var enumerable4 = response4 as IList<Response> ?? response4.ToList();
            var item4 = enumerable4.Single().ResponseItem ?? "test string";
            sections.Add(new Paragraph() { Text = item4, Format = _subtitle, Align = Center });

            var response5 = pi.Any(x => x.QuestionText == "Phone Number") ? pi.Where(x => x.QuestionText == "Phone Number") : notfound;
            var enumerable5 = response5 as IList<Response> ?? response5.ToList();
            var item5 = enumerable5.Single().ResponseItem ?? "test string";
            sections.Add(new Paragraph() { Text = item5, Format = _subtitle, Align = Center });
            return sections;
        }

        protected List<Paragraph> EmploymentInformation(List<Response> pi)
        {
            var sections = new List<Paragraph>();

            if (!pi.Any()) return sections;

            var notfound = new List<Response> {new Response() {ResponseItem = "Not Found"}};

            var subordinalMax = pi.Select(x => x.SubOrdinal).Max();

            sections.Add(new Paragraph() { Text = "Employment History", Format = _title, Align = Center });

            for (var i = 0; i < subordinalMax + 1; i++)
            {
                /* paragraph spacer */
                sections.Add(new Paragraph() { Text = " ", Format = _normal, Align = Left });

                /* Employer Business Name */
                var i1 = i;
                var response1 = pi.Any(x => x.QuestionText == "Employer Business Name" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Employer Business Name" && x.SubOrdinal == i1) : notfound;
                var enumerable1 = response1 as IList<Response> ?? response1.ToList();
                var item1 = enumerable1.Where(x=>x.ResponseItem != "").First().ResponseItem ?? "Employer Business Name";
                sections.Add(new Paragraph() { Text = item1, Format = _subtitle, Align = Left });

                /* Employer Business Address */
                var response2 = pi.Any(x => x.QuestionText == "Employer Business Address" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Employer Business Address" && x.SubOrdinal == i1) : notfound;
                var enumerable2 = response2 as IList<Response> ?? response2.ToList();
                var item2 = enumerable2.Single().ResponseItem ?? "Employer Business Address";
                response2 = pi.Any(x => x.QuestionText == "City" && x.SubOrdinal == i1 && x.QCategoryName.Contains("Employment")) ? pi.Where(x => x.QuestionText == "City" && x.SubOrdinal == i1) : notfound;
                var enumerable3 = response2 as IList<Response> ?? response2.ToList();
                item2 += enumerable3.Single().ResponseItem != null ? ", " + enumerable3.Single().ResponseItem : ", Employer City";
                response2 = pi.Any(x => x.QuestionText == "State" && x.SubOrdinal == i1 && x.QCategoryName.Contains("Employment")) ? pi.Where(x => x.QuestionText == "State" && x.SubOrdinal == i1) : notfound;
                var enumerable4 = response2 as IList<Response> ?? response2.ToList();
                item2 += enumerable4.Single().ResponseItem != null ? ", " + enumerable4.Single().ResponseItem : ", Employer State";
                response2 = pi.Any(x => x.QuestionText == "Zip" && x.SubOrdinal == i1 && x.QCategoryName.Contains("Employment")) ? pi.Where(x => x.QuestionText == "Zip" && x.SubOrdinal == i1) : notfound;
                var enumerable5 = response2 as IList<Response> ?? response2.ToList();
                item2 += enumerable5.Single().ResponseItem != null ? " " + enumerable5.Single().ResponseItem : " Employer Zip";
                sections.Add(new Paragraph() { Text = item2, Format = _normal, Align = Left });

                /* Position Title */
                IEnumerable<Response> response3;
                var response3Test1 = pi.Where(x => x.QuestionText.Contains("Direct Services") && x.SubOrdinal == i1 && x.QCategoryName.Contains("Employment"));
                var response3Test2 = pi.Where(x => x.QuestionText.Contains("Indirect Services") && x.SubOrdinal == i1 && x.QCategoryName.Contains("Employment"));
                var response3Test3 = pi.Where(x => x.QuestionText.Contains("Other Position Title") && x.SubOrdinal == i1 && x.QCategoryName.Contains("Employment"));
                var enumerable6 = response3Test1 as IList<Response> ?? response3Test1.ToList();
                var firstOrDefault = enumerable6.FirstOrDefault();
                if (firstOrDefault != null && (enumerable6.Any() && firstOrDefault.ResponseItem != null))
                    response3 = pi.Where(x => x.QuestionText.Contains("Direct Services") && x.SubOrdinal == i1 && x.QCategoryName.Contains("Employment"));
                else
                {
                    var enumerable7 = response3Test2 as IList<Response> ?? response3Test2.ToList();
                    if (enumerable7.Any() && enumerable7.FirstOrDefault().ResponseItem != null)
                        response3 = pi.Where(x => x.QuestionText.Contains("Indirect Services") && x.SubOrdinal == i1 && x.QCategoryName.Contains("Employment"));
                    else
                    {
                        var enumerable8 = response3Test3 as IList<Response> ?? response3Test3.ToList();
                        if (enumerable8.Any() && enumerable8.FirstOrDefault().ResponseItem != null)
                            response3 = pi.Where(x => x.QuestionText.Contains("Other Position Title") && x.SubOrdinal == i1 && x.QCategoryName.Contains("Employment"));
                        else response3 = notfound;
                    }
                }
                var enumerable9 = response3 as IList<Response> ?? response3.ToList();
                var item3 = enumerable9.Single().ResponseItem ?? "Position Title";
                sections.Add(new Paragraph() { Text = item3, Format = _subtitle, Align = Left });

                /* Position Description */
                var response4 = pi.Any(x => x.QuestionText == "Position Description" && x.SubOrdinal == i) ? pi.Where(x => x.QuestionText == "Position Description" && x.SubOrdinal == i1) : notfound;
                var enumerable10 = response4 as IList<Response> ?? response4.ToList();
                var item4 = enumerable10.Single().ResponseItem ?? "Position Description";
                sections.Add(new Paragraph() { Text = item4, Format = _normal, Align = Left });
            }

            return sections;
        }

        protected List<Paragraph> CertificationInformation(List<Response> pi)
        {
            var sections = new List<Paragraph>();

            if (!pi.Any()) return sections;

            var notfound = new List<Response> {new Response() {ResponseItem = "Not Found"}};

            var subordinalMax = pi.Select(x => x.SubOrdinal).Max();

            sections.Add(new Paragraph() { Text = "Current Professional Certifications", Format = _title, Align = Center });

            for (var i = 0; i < subordinalMax + 1; i++)
            {
                /* paragraph spacer */
                sections.Add(new Paragraph() { Text = " ", Format = _normal, Align = Left });

                /* Name of Professional Certification */
                var i1 = i;
                var response1 = pi.Any(x => x.QuestionText == "Name of Professional Certification" && x.SubOrdinal == i) ? pi.Where(x => x.QuestionText == "Name of Professional Certification" && x.SubOrdinal == i1) : notfound;
                var enumerable1 = response1 as IList<Response> ?? response1.ToList();
                var item1 = enumerable1.Single().ResponseItem != null ? "Certification Name: " + enumerable1.Single().ResponseItem : "Name of Professional Certification";
                sections.Add(new Paragraph() { Text = item1, Format = _subtitle, Align = Left });

                /* Awarding Organization/Institution */
                var response2 = pi.Any(x => x.QuestionText == "Awarding Organization/Institution" && x.SubOrdinal == i) ? pi.Where(x => x.QuestionText == "Awarding Organization/Institution" && x.SubOrdinal == i1) : notfound;
                var enumerable2 = response2 as IList<Response> ?? response2.ToList();
                var item2 = enumerable2.Single().ResponseItem != null ? "Awarding Organization: " + enumerable2.Single().ResponseItem : "Awarding Organization/Institution";
                sections.Add(new Paragraph() { Text = item2, Format = _normal, Align = Left });

                /* Date Awarded, if applicable */
                var response3 = pi.Any(x => x.QuestionText == "Date Awarded, if applicable" && x.SubOrdinal == i) ? pi.Where(x => x.QuestionText == "Date Awarded, if applicable" && x.SubOrdinal == i1) : notfound;
                var enumerable3 = response3 as IList<Response> ?? response3.ToList();
                var item3 = enumerable3.Single().ResponseItem != null ? "Date Awarded: " + enumerable3.Single().ResponseItem : "Date Awarded, if applicable";
                sections.Add(new Paragraph() { Text = item3, Format = _normal, Align = Left });

                /* Date Expires, if applicable */
                var response4 = pi.Any(x => x.QuestionText == "Date Expires, if applicable" && x.SubOrdinal == i) ? pi.Where(x => x.QuestionText == "Date Expires, if applicable" && x.SubOrdinal == i1) : notfound;
                var enumerable4 = response4 as IList<Response> ?? response4.ToList();
                var item4 = enumerable4.Single().ResponseItem != null ? "Date Expires: " + enumerable4.Single().ResponseItem : "Date Expires, if applicable";
                sections.Add(new Paragraph() { Text = item4, Format = _normal, Align = Left });
            }

            return sections;
        }

        protected List<Paragraph> LicenseInformation(List<Response> pi)
        {
            var sections = new List<Paragraph>();

            if (!pi.Any()) return sections;

            var notfound = new List<Response> {new Response() {ResponseItem = "Not Found"}};

            var subordinalMax = pi.Select(x => x.SubOrdinal).Max();

            sections.Add(new Paragraph() { Text = "Current Professional Licenses", Format = _title, Align = Center });

            for (var i = 0; i < subordinalMax + 1; i++)
            {
                /* paragraph spacer */
                sections.Add(new Paragraph() { Text = " ", Format = _normal, Align = Left });

                /* Name of Professional License */
                int i1 = i;
                var response1 = pi.Any(x => x.QuestionText == "Name of Professional License" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Name of Professional License" && x.SubOrdinal == i1) : notfound;
                var enumerable1 = response1 as IList<Response> ?? response1.ToList();
                var item1 = enumerable1.Single().ResponseItem != null ? "License Name: " + enumerable1.Single().ResponseItem : "Name of Professional License";
                sections.Add(new Paragraph() { Text = item1, Format = _subtitle, Align = Left });

                /* License Number */
                var response2 = pi.Any(x => x.QuestionText == "License Number" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "License Number" && x.SubOrdinal == i1) : notfound;
                var enumerable2 = response2 as IList<Response> ?? response2.ToList();
                var item2 = enumerable2.Single().ResponseItem != null ? "License Number: " + enumerable2.Single().ResponseItem : "License Number";
                sections.Add(new Paragraph() { Text = item2, Format = _normal, Align = Left });

                /* Date Awarded, if applicable */
                var response3 = pi.Any(x => x.QuestionText == "Date Awarded, if applicable" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Date Awarded, if applicable" && x.SubOrdinal == i1) : notfound;
                var enumerable3 = response3 as IList<Response> ?? response3.ToList();
                var item3 = enumerable3.Single().ResponseItem != null ? "Date Awarded: " + enumerable3.Single().ResponseItem : "Date Awarded, if applicable";
                sections.Add(new Paragraph() { Text = item3, Format = _normal, Align = Left });

                /* Date Expires, if applicable */
                var response4 = pi.Any(x => x.QuestionText == "Date Expires, if applicable" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Date Expires, if applicable" && x.SubOrdinal == i1) : notfound;
                var enumerable4 = response4 as IList<Response> ?? response4.ToList();
                var item4 = enumerable4.Single().ResponseItem != null ? "Date Expires: " + enumerable4.Single().ResponseItem : "Date Expires, if applicable";
                sections.Add(new Paragraph() { Text = item4, Format = _normal, Align = Left });
            }

            return sections;
        }

        protected List<Paragraph> EducationInformation(List<Response> pi)
        {
            var sections = new List<Paragraph>();

            if (!pi.Any()) return sections;

            var notfound = new List<Response> {new Response() {ResponseItem = "Not Found"}};

            var subordinalMax = pi.Select(x => x.SubOrdinal).Max();

            sections.Add(new Paragraph() { Text = "Education Degree(s) Completed", Format = _title, Align = Center });

            for (var i = 0; i < subordinalMax + 1; i++)
            {
                /* paragraph spacer */
                sections.Add(new Paragraph() { Text = " ", Format = _normal, Align = Left });

                /* Degree Type */
                int i1 = i;
                var response1 = pi.Any(x => x.QuestionText == "Degree Type" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Degree Type" && x.SubOrdinal == i1) : notfound;
                var enumerable1 = response1 as IList<Response> ?? response1.ToList();
                var item1 = enumerable1.Single().ResponseItem != null ? "Degree Type: " + enumerable1.Single().ResponseItem : "Degree Type";
                sections.Add(new Paragraph() { Text = item1, Format = _subtitle, Align = Left });

                /* Name of Awarding Institution */
                var response2 = pi.Any(x => x.QuestionText == "Name of Awarding Institution" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Name of Awarding Institution" && x.SubOrdinal == i1) : notfound;
                var enumerable2 = response2 as IList<Response> ?? response2.ToList();
                var item2 = enumerable2.Single().ResponseItem != null ? "Awarding Institution: " + enumerable2.Single().ResponseItem : "Name of Awarding Institution";
                sections.Add(new Paragraph() { Text = item2, Format = _normal, Align = Left });

                /* Date Degree Awarded */
                var response3 = pi.Any(x => x.QuestionText == "Date Degree Awarded" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Date Degree Awarded" && x.SubOrdinal == i1) : notfound;
                var enumerable3 = response3 as IList<Response> ?? response3.ToList();
                var item3 = enumerable3.Single().ResponseItem != null ? "Date Awarded: " + enumerable3.Single().ResponseItem : "Date Degree Awarded";
                sections.Add(new Paragraph() { Text = item3, Format = _normal, Align = Left });

                /* Educational Institution Location */
                var response4 = pi.Any(x => x.QuestionText == "Educational Institution Location" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Educational Institution Location" && x.SubOrdinal == i1) : notfound;
                var enumerable4 = response4 as IList<Response> ?? response4.ToList();
                var item4 = enumerable4.Single().ResponseItem != null ? "Educational Institution Location: " + enumerable4.Single().ResponseItem : "Educational Institution Location";
                sections.Add(new Paragraph() { Text = item4, Format = _normal, Align = Left });

                /* Major Area */
                var response5 = pi.Any(x => x.QuestionText == "Major Area" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Major Area" && x.SubOrdinal == i1) : notfound;
                var enumerable5 = response5 as IList<Response> ?? response5.ToList();
                var item5 = enumerable5.Single().ResponseItem != null ? "Major Area: " + enumerable5.Single().ResponseItem : "Major Area";
                sections.Add(new Paragraph() { Text = item5, Format = _normal, Align = Left });
            }

            return sections;
        }

        protected List<Paragraph> CredentialEndorsementInformation(List<Response> pi)
        {
            var sections = new List<Paragraph>();

            if (!pi.Any()) return sections;
            
            var notfound = new List<Response> {new Response() {ResponseItem = "Not Found"}};

            var subordinalMax = pi.Select(x => x.SubOrdinal).Max();

            sections.Add(new Paragraph() { Text = "Professional Credentials/Endorsements", Format = _title, Align = Center });

            for (var i = 0; i < subordinalMax + 1; i++)
            {
                /* paragraph spacer */
                sections.Add(new Paragraph() { Text = " ", Format = _normal, Align = Left });

                /* Name of Professional Credential/Endorsement */
                var i1 = i;
                var response1 = pi.Any(x => x.QuestionText == "Name of Professional Credential/Endorsement" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Name of Professional Credential/Endorsement" && x.SubOrdinal == i1) : notfound;
                var enumerable1 = response1 as IList<Response> ?? response1.ToList();
                var item1 = enumerable1.Single().ResponseItem != null ? "Name of Professional Credential/Endorsement: " + enumerable1.Single().ResponseItem : "Name of Professional Credential/Endorsement";
                sections.Add(new Paragraph() { Text = item1, Format = _subtitle, Align = Left });

                /* Credential/Endorsement Type */
                var response2 = pi.Any(x => x.QuestionText == "Credential/Endorsement Type" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Credential/Endorsement Type" && x.SubOrdinal == i1) : notfound;
                var enumerable2 = response2 as IList<Response> ?? response2.ToList();
                var item2 = enumerable2.Single().ResponseItem != null ? "Credential/Endorsement Type: " + enumerable2.Single().ResponseItem : "Credential/Endorsement Type";
                sections.Add(new Paragraph() { Text = item2, Format = _normal, Align = Left });

                /* Credential/Endorsement Sub-Type */
                var response3 = pi.Any(x => x.QuestionText == "Credential/Endorsement Sub-Type" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Credential/Endorsement Sub-Type" && x.SubOrdinal == i1) : notfound;
                var enumerable3 = response3 as IList<Response> ?? response3.ToList();
                var item3 = enumerable3.Single().ResponseItem != null ? "Credential/Endorsement Sub-Type: " + enumerable3.Single().ResponseItem : "Credential/Endorsement Sub-Type";
                sections.Add(new Paragraph() { Text = item3, Format = _normal, Align = Left });

                /* Awarding Organization/Institution */
                var response4 = pi.Any(x => x.QuestionText == "Awarding Organization/Institution" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Awarding Organization/Institution" && x.SubOrdinal == i1) : notfound;
                var enumerable4 = response4 as IList<Response> ?? response4.ToList();
                var item4 = enumerable4.Single().ResponseItem != null ? "Awarding Organization/Institution: " + enumerable4.Single().ResponseItem : "Awarding Organization/Institution";
                sections.Add(new Paragraph() { Text = item4, Format = _normal, Align = Left });

                /* Date Awarded, if applicable */
                var response5 = pi.Any(x => x.QuestionText == "Date Awarded, if applicable" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Date Awarded, if applicable" && x.SubOrdinal == i1) : notfound;
                var enumerable5 = response5 as IList<Response> ?? response5.ToList();
                var item5 = enumerable5.Single().ResponseItem != null ? "Date Awarded: " + enumerable5.Single().ResponseItem : "Date Awarded, if applicable";
                sections.Add(new Paragraph() { Text = item5, Format = _normal, Align = Left });

                /* Date Expires, if applicable */
                var response6 = pi.Any(x => x.QuestionText == "Date Expires, if applicable" && x.SubOrdinal == i) ? pi.Where(x => x.QuestionText == "Date Expires, if applicable" && x.SubOrdinal == i1) : notfound;
                var enumerable6 = response6 as IList<Response> ?? response6.ToList();
                var item6 = enumerable6.Single().ResponseItem != null ? "Date Expires: " + enumerable6.Single().ResponseItem : "Date Expires, if applicable";
                sections.Add(new Paragraph() { Text = item6, Format = _normal, Align = Left });
            }
            return sections;
        }

        protected List<Paragraph> OtherCourseworkInformation(List<Response> pi)
        {
            var sections = new List<Paragraph>();

            if (!pi.Any()) return sections;

            var notfound = new List<Response> {new Response() {ResponseItem = "Not Found"}};

            var subordinalMax = pi.Select(x => x.SubOrdinal).Max();

            sections.Add(new Paragraph() { Text = "Other College Coursework Completed", Format = _title, Align = Center });

            for (var i = 0; i < subordinalMax + 1; i++)
            {
                /* paragraph spacer */
                sections.Add(new Paragraph() { Text = " ", Format = _normal, Align = Left });

                /* Name of Course */
                var i1 = i;
                var response1 = pi.Any(x => x.QuestionText == "Name of Course" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Name of Course" && x.SubOrdinal == i1) : notfound;
                var enumerable1 = response1 as IList<Response> ?? response1.ToList();
                var item1 = enumerable1.Single().ResponseItem != null ? "Name of Course: " + enumerable1.Single().ResponseItem : "Name of Course";
                sections.Add(new Paragraph() { Text = item1, Format = _subtitle, Align = Left });

                /* Course Number */
                var response2 = pi.Any(x => x.QuestionText == "Course Number" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Course Number" && x.SubOrdinal == i1) : notfound;
                var enumerable2 = response2 as IList<Response> ?? response2.ToList();
                var item2 = enumerable2.Single().ResponseItem != null ? "Course Number: " + enumerable2.Single().ResponseItem : "Course Number";
                sections.Add(new Paragraph() { Text = item2, Format = _normal, Align = Left });

                /* Name of Awarding Institution */
                var response3 = pi.Any(x => x.QuestionText == "Name of Awarding Institution" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Name of Awarding Institution" && x.SubOrdinal == i1) : notfound;
                var enumerable3 = response3 as IList<Response> ?? response3.ToList();
                var item3 = enumerable3.Single().ResponseItem != null ? "Name of Awarding Institution: " + enumerable3.Single().ResponseItem : "Name of Awarding Institution";
                sections.Add(new Paragraph() { Text = item3, Format = _normal, Align = Left });

                /* Institution Location */
                var response4 = pi.Any(x => x.QuestionText == "Institution Location" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Institution Location" && x.SubOrdinal == i1) : notfound;
                var enumerable4 = response4 as IList<Response> ?? response4.ToList();
                var item4 = enumerable4.Single().ResponseItem != null ? "Institution Location: " + enumerable4.Single().ResponseItem : "Institution Location";
                sections.Add(new Paragraph() { Text = item4, Format = _normal, Align = Left });

                /* No. of Credit Hours, if applicable */
                var response5 = pi.Any(x => x.QuestionText == "No. of Credit Hours, if applicable" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "No. of Credit Hours, if applicable" && x.SubOrdinal == i1) : notfound;
                var enumerable5 = response5 as IList<Response> ?? response5.ToList();
                var item5 = enumerable5.Single().ResponseItem != null ? "No. of Credit Hours: " + enumerable5.Single().ResponseItem : "No. of Credit Hours, if applicable";
                sections.Add(new Paragraph() { Text = item5, Format = _normal, Align = Left });

                /* Date Completed */
                var response6 = pi.Any(x => x.QuestionText == "Date Completed" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Date Completed, if applicable" && x.SubOrdinal == i1) : notfound;
                var enumerable6 = response6 as IList<Response> ?? response6.ToList();
                if (enumerable6.Any())
                {
                    var item6 = enumerable6.Single().ResponseItem != null ? "Date Completed: " + enumerable6.Single().ResponseItem : "Date Completed";
                    sections.Add(new Paragraph() { Text = item6, Format = _normal, Align = Left });
                }

                /* No. of Quarter Hours, if applicable */
                var response7 = pi.Any(x => x.QuestionText == "No. of Quarter Hours, if applicable" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "No. of Quarter Hours, if applicable" && x.SubOrdinal == i1) : notfound;
                var enumerable7 = response7 as IList<Response> ?? response7.ToList();
                var item7 = enumerable7.Single().ResponseItem != null ? "No. of Quarter Hours: " + enumerable7.Single().ResponseItem : "No. of Quarter Hours, if applicable";
                sections.Add(new Paragraph() { Text = item7, Format = _normal, Align = Left });
            }

            return sections;
        }

        protected List<Paragraph> ProfessionalTrainingInformation(List<Response> pi)
        {
            var sections = new List<Paragraph>();

            if (!pi.Any()) return sections;

            var notfound = new List<Response> {new Response() {ResponseItem = "Not Found"}};

            var subordinalMax = pi.Select(x => x.SubOrdinal).Max();

            sections.Add(new Paragraph() { Text = "Professional Training", Format = _title, Align = Center });

            for (var i = 0; i < subordinalMax + 1; i++)
            {
                /* paragraph spacer */
                sections.Add(new Paragraph() { Text = " ", Format = _normal, Align = Left });

                /* Training Title */
                var i1 = i;
                var response1 = pi.Any(x => x.QuestionText == "Training Title" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Training Title" && x.SubOrdinal == i1) : notfound;
                var enumerable1 = response1 as IList<Response> ?? response1.ToList();
                var item1 = enumerable1.Single().ResponseItem != null ? "Training Title: " + enumerable1.Single().ResponseItem : "Training Title";
                sections.Add(new Paragraph() { Text = item1, Format = _subtitle, Align = Left });

                /* Training Type */
                var response2 = pi.Any(x => x.QuestionText == "Training Type" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Training Type" && x.SubOrdinal == i1) : notfound;
                var enumerable2 = response2 as IList<Response> ?? response2.ToList();
                var item2 = enumerable2.Single().ResponseItem != null ? "Training Type: " + enumerable2.Single().ResponseItem : "Training Type";
                sections.Add(new Paragraph() { Text = item2, Format = _normal, Align = Left });

                /* Core Competency Areas */
                var response3 = pi.Any(x => x.QuestionText == "Core Competency Areas" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Core Competency Areas" && x.SubOrdinal == i1) : notfound;
                var enumerable3 = response3 as IList<Response> ?? response3.ToList();
                var item3 = enumerable3.Single().ResponseItem != null ? "Core Competency Areas: " + enumerable3.Single().ResponseItem : "Core Competency Areas";
                sections.Add(new Paragraph() { Text = item3, Format = _normal, Align = Left });

                /* Date Completed */
                var response4 = pi.Any(x => x.QuestionText == "Date Completed" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Date Completed" && x.SubOrdinal == i1) : notfound;
                var enumerable4 = response4 as IList<Response> ?? response4.ToList();
                var item4 = enumerable4.Single().ResponseItem != null ? "Date Completed: " + enumerable4.Single().ResponseItem : "Date Completed";
                sections.Add(new Paragraph() { Text = item4, Format = _normal, Align = Left });

                /* Number of Clock Hours */
                var response5 = pi.Any(x => x.QuestionText == "Number of Clock Hours" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Number of Clock Hours" && x.SubOrdinal == i1) : notfound;
                var enumerable5 = response5 as IList<Response> ?? response5.ToList();
                var item5 = enumerable5.Single().ResponseItem != null ? "Number of Clock Hours: " + enumerable5.Single().ResponseItem : "Number of Clock Hours";
                sections.Add(new Paragraph() { Text = item5, Format = _normal, Align = Left });

                /* Number of CEUs, if available */
                var response6 = pi.Any(x => x.QuestionText == "Number of CEUs, if available" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Number of CEUs, if available" && x.SubOrdinal == i1) : notfound;
                var enumerable6 = response6 as IList<Response> ?? response6.ToList();
                var item6 = enumerable6.Single().ResponseItem != null ? "Number of CEUs: " + enumerable6.Single().ResponseItem : "Number of CEUs, if available";
                sections.Add(new Paragraph() { Text = item6, Format = _normal, Align = Left });

                /* Training Sponsor/Org. */
                var response7 = pi.Any(x => x.QuestionText == "Training Sponsor/Org." && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Training Sponsor/Org." && x.SubOrdinal == i1) : notfound;
                var enumerable7 = response7 as IList<Response> ?? response7.ToList();
                var item7 = enumerable7.Single().ResponseItem != null ? "Training Sponsor/Org: " + enumerable7.Single().ResponseItem : "Training Sponsor/Org.";
                sections.Add(new Paragraph() { Text = item7, Format = _normal, Align = Left });

                /* Trainer Name */
                var response8 = pi.Any(x => x.QuestionText == "Trainer Name" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Trainer Name" && x.SubOrdinal == i1) : notfound;
                var enumerable8 = response8 as IList<Response> ?? response8.ToList();
                var item8 = enumerable8.Single().ResponseItem != null ? "Trainer Name: " + enumerable8.Single().ResponseItem : "Trainer Name";
                sections.Add(new Paragraph() { Text = item8, Format = _normal, Align = Left });

                /* Training Location */
                var response9 = pi.Any(x => x.QuestionText == "Training Location" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Training Location" && x.SubOrdinal == i1) : notfound;
                var item9 = response9.Single().ResponseItem != null ? "Training Location: " + response9.Single().ResponseItem : "Training Location";
                sections.Add(new Paragraph() { Text = item9, Format = _normal, Align = Left });

                /* Grade */
                var response10 = pi.Any(x => x.QuestionText == "Grade" && x.SubOrdinal == i1) ? pi.Where(x => x.QuestionText == "Grade" && x.SubOrdinal == i1) : notfound;
                var enumerable10 = response10 as IList<Response> ?? response10.ToList();
                var item10 = enumerable10.Single().ResponseItem != null ? "Grade: " + enumerable10.Single().ResponseItem : "Grade";
                sections.Add(new Paragraph() { Text = item10, Format = _normal, Align = Left });
            }

            return sections;
        }
    }
}