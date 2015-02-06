using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Questionnaire2.Models
{
    public class Response
    {
        [Key]
        public int ResponseId { get; set; }

        public int UserId { get; set; }
        public int QuestionnaireId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string QTitle { get; set; }
        public string QTypeResponse { get; set; }
        public int QuestionnaireQuestionId { get; set; }
        public int QQOrd { get; set; }
        public int QCategoryId { get; set; }
        public string QCategoryName { get; set; }
        public int QuestionnaireQCategoryId { get; set; }
        public int Ordinal { get; set; }
        public int SubOrdinal { get; set; }
        public string ResponseItem { get; set; }

        public ICollection<Answer> Answers { get; set; }
    }
}