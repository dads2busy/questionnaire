using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Questionnaire2.ViewModels
{
    public class UserVerifications
    {
        public List<UserInfo> UsersVerified { get; set; }
        public List<UserInfo> UsersUnverified { get; set; }
    }
}