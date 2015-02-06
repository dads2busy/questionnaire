using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Questionnaire2.ViewModels
{
    public class UserInfo
    {
        public int UserId { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Number Verified")]
        public int VerifiedCount { get; set; }
        [Display(Name = "Number Unverified")]
        public int UnverifiedCount { get; set; }
        [Display(Name = "Editable")]
        public bool Editable { get; set; }
    }
}