using System.ComponentModel.DataAnnotations.Schema;

namespace Questionnaire2.Models
{
    public class QuestionnaireQCategory
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [ForeignKey("Questionnaire")]
        public int? QuestionnaireId { get; set; }
        public Questionnaire Questionnaire { get; set; }

        [ForeignKey("QCategory")]
        public int? QCategoryId { get; set; }
        public QCategory QCategory { get; set; }

        public int Ordinal { get; set; }

        public int SubOrdinal { get; set; }

    }
}