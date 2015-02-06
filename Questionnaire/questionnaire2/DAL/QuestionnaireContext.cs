using System.Data.Entity;
using Questionnaire2.Models;

namespace Questionnaire2.DAL
{
    
    
    public class QuestionnaireContext : DbContext
    {
        static QuestionnaireContext() {
            Database.SetInitializer<QuestionnaireContext>(null);
        }
        public DbSet<Questionnaire> Questionnaires { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionnaireQuestion> QuestionnaireQuestions { get; set; }
        public DbSet<QuestionnaireQCategory> QuestionnaireQCategories { get; set; }
        public DbSet<QType> QTypes { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<QCategory> QCategories { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<Verification> Verifications { get; set; }
        public DbSet<LatticeItem> LatticeItems { get; set; }
    }
}