using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Questionnaire2.Models
{
    public class Verification
    {
        [Key]
        public int Id { get; set; }
        public int QuestionnaireId { get; set; }
        public int UserId { get; set; }
        public int QCategoryId { get; set; }
        public int SubOrdinal { get; set; }
        public int QQCategoryId { get; set; }
        [UIHint("tinymce_jquery_full"), AllowHtml]
        public string ItemInfo { get; set; }
        public bool ItemVerified { get; set; }
        public string ItemStepLevel { get; set; }
        public string Notes { get; set; }
        public bool Editable { get; set; }
    }
}