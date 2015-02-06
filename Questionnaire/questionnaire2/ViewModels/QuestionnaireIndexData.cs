using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Questionnaire2.Models;

namespace Questionnaire2.ViewModels
{
    public class QuestionnaireIndexData
    {
        public IEnumerable<Questionnaire> Questionnaires { get; set; }
        public IEnumerable<Question> Questions { get; set; }
    }
}