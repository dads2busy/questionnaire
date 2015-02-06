using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Questionnaire2.Models;

namespace Questionnaire2.ViewModels
{
    public class File2DB
    {
        [Required]
        public HttpPostedFileBase File { get; set; }
        public int UserId { get; set; }
        public int QuestionnaireId { get; set; }
        public int QuestionnaireQCategoryId { get; set; }
        public int QCategorySubOrdinal { get; set; }
        public string QCategoryName { get; set; }
        public string Description { get; set; }

        public ICollection<File> UserFiles { get; set; }
    }
}