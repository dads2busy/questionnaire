using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Questionnaire2.Models
{
    public class QCategory
    {
        //[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int QCategoryId { get; set; }

        [Display(Name = "Section")]
        public string QCategoryName { get; set; }

        [UIHint("tinymce_jquery_full"), AllowHtml]
        public string Instructions { get; set; }

        [Display(Name = "Repeatable")]
        public bool Repeatable { get; set; }

        [Display(Name = "Uploads")]
        public bool Uploads { get; set; }

        ICollection<QuestionnaireQuestion> QuestionnaireQuestions { get; set; }
        ICollection<QuestionnaireQCategory> QuestionnaireQCategories { get; set; }
    }
}