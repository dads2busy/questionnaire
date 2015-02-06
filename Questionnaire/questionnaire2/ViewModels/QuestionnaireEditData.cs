using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Questionnaire2.Models;

namespace Questionnaire2.ViewModels
{
    public class QuestionnaireEditData
    {
        public Questionnaire Questionnaire { get; set; }
        public List<KeyValuePair<int, SelectList>> QuestionDropDownLists { get; set; }
        public List<KeyValuePair<int, SelectList>> QCategoryDropDownLists { get; set; }
        public List<KeyValuePair<int, SelectList>> OrdinalDropDownLists { get; set; }

        public List<KeyValuePair<int, string>> QuestionnaireSections { get; set; }
        public IEnumerable<SelectListItem> Sections { get; set; }

        public Question Question { get; set; }
        public SelectList QTypeNames { get; set; }
    }

}