using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Questionnaire2.Models
{
    public class QuestionnaireQuestion
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Ordinal { get; set; }

        [ForeignKey("Questionnaire")]
        public int? QuestionnaireId { get; set; }
        public Questionnaire Questionnaire { get; set; }

        [ForeignKey("Question")]
        public int? QuestionId { get; set; }
        public Question Question { get; set; }

        [ForeignKey("QuestionnaireQCategory")]
        public int? QQCategoryId { get; set; }
        public QuestionnaireQCategory QuestionnaireQCategory { get; set; }
    }
}