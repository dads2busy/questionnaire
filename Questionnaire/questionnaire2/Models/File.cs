using System.ComponentModel.DataAnnotations;

namespace Questionnaire2.Models
{
    public class File
    {
        [Key]
        public int FileId { get; set; }
        public int UserId { get; set; }
        public int QuestionnaireId { get; set; }
        public int QuestionnaireQCategoryId { get; set; }
        public int QCategorySubOrdinal { get; set; }
        public string QCategoryName { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public byte[] FileBytes { get; set; }
    }
}