using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Questionnaire2.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public string QTypeName { get; set; }
        public bool selectedValue { get; set; }

        public ICollection<Response> Responses { get; set; }
    }
}