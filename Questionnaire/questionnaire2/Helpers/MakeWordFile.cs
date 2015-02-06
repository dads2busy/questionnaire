using Novacode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Questionnaire2.Helpers
{
    public static class MakeWordFile
    {
        public static MemoryStream CreateDocument(List<FormatUserInformation.Paragraph> paras)
        {
            using (var ms = new MemoryStream())
            {
                var doc = DocX.Create(ms);

                foreach (var p in paras)
                {
                    var para = doc.InsertParagraph(p.Text, false, p.Format);
                    para.Alignment = p.Align;
                }

                doc.Save();
                return ms;
            }
            
        }
        
        public static void CreateSampleDocument()
        {
            // Modify to suit your machine:
            var fileName = Navigation.GetRoot() + @"\DocXExample.docx";
            var headlineText = "Constitution of the United States";
            var paraOne = ""
                + "We the People of the United States, in Order to form a more perfect Union, "
                + "establish Justice, insure domestic Tranquility, provide for the common defence, "
                + "promote the general Welfare, and secure the Blessings of Liberty to ourselves "
                + "and our Posterity, do ordain and establish this Constitution for the United "
                + "States of America.";

            // A formatting object for our headline:
            var headLineFormat = new Formatting
            {
                FontFamily = new System.Drawing.FontFamily("Arial Black"),
                Size = 18D,
                Position = 12
            };

            // A formatting object for our normal paragraph text:
            var paraFormat = new Formatting {FontFamily = new System.Drawing.FontFamily("Calibri"), Size = 10D};

            // Create a document in memory:
            var doc = DocX.Create(fileName);

            // Insert the now text obejcts;
            doc.InsertParagraph(headlineText, false, headLineFormat);
            doc.InsertParagraph(paraOne, false, paraFormat);

            // Save to the output directory:
            doc.Save();

            // Open in Word:
            // Process.Start("WINWORD.EXE", fileName);
        }
    }
}