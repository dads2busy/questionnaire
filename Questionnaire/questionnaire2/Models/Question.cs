using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Questionnaire2.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        
        [Display(Name = "Question Info")]
        public string QTitle { get; set; }
        
        [Display(Name = "Question Text")]
        public string QuestionText { get; set; }

        [Display(Name = "Question Type")]
        [ForeignKey("QType")]
        public string QTypeName { get; set; }
        public QType QType { get; set; }

        ICollection<QuestionnaireQuestion> QuestionnaireQuestions { get; set; }
    }
}