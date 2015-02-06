using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Questionnaire2.Models
{
    public class Questionnaire
    {
        public int QuestionnaireId { get; set; }
        public string QuestionnaireName { get; set; }

        public ICollection<QuestionnaireQuestion> QuestionnaireQuestions { get; set; }
        public ICollection<QuestionnaireQCategory> QuestionnaireQCategories { get; set; }
    }
}