using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Questionnaire2.Helpers
{
    public static class Word2Pdf
    {
        public static void ConvertWord2Pdf()
        {
            using (var client = new WebClient())
            {

                var fileToConvert = Helpers.Navigation.GetRoot() + "DocXExample.docx";

                Console.WriteLine(string.Format("Converting the file {0} Please wait.", fileToConvert));

                var data = new NameValueCollection();
                data.Add("OutputFileName", "MyFile.pdf");

                try
                {
                    client.QueryString.Add(data);
                    var response = client.UploadFile("http://do.convertapi.com/word2pdf", fileToConvert);
                    var responseHeaders = client.ResponseHeaders;
                    var web2PdfOutputFileName = responseHeaders["OutputFileName"];
                    var path = Helpers.Navigation.GetRoot() + "DocXExample.pdf";
                    File.WriteAllBytes(path, response);
                    Console.WriteLine("The conversion was successful! The word file {0} converted to PDF and saved at {1}", fileToConvert, path);
                }
                catch (WebException e)
                {
                    Console.WriteLine("Exception Message :" + e.Message);
                    if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        Console.WriteLine("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                        Console.WriteLine("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    }

                }


            }

            Console.ReadLine();
        }
    }
}