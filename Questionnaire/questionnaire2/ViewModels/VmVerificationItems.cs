using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Questionnaire2.Models;

namespace Questionnaire2.ViewModels
{
    public class VmVerificationItems
    {
        public ICollection<VmVerificationItem> VerificationItems { get; set; }
        public IList<SelectListItem> LatticeItems { get; set; }
    }
    
    public class VmVerificationItem
    {
        public Verification Verification { get; set; }
        public ICollection<File> Files { get; set; }
    }
}