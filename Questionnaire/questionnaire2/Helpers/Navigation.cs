using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Questionnaire2.Helpers
{
    public class Navigation
    {
        public static string GetRoot()
        {
            return HostingEnvironment.MapPath("~/");
        }
    }
}