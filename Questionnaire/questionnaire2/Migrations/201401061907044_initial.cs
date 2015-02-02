namespace Questionnaire2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        AnswerId = c.Int(nullable: false, identity: true),
                        AnswerText = c.String(),
                        QTypeName = c.String(maxLength: 128),
                        selectedValue = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AnswerId)
                .ForeignKey("dbo.QTypes", t => t.QTypeName)
                .Index(t => t.QTypeName);
            
            CreateTable(
                "dbo.Responses",
                c => new
                    {
                        ResponseId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        QuestionnaireId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        QuestionText = c.String(),
                        QTitle = c.String(),
                        QTypeResponse = c.String(),
                        QuestionnaireQuestionId = c.Int(nullable: false),
                        QQOrd = c.Int(nullable: false),
                        QCategoryId = c.Int(nullable: false),
                        QCategoryName = c.String(),
                        Ordinal = c.Int(nullable: false),
                        SubOrdinal = c.Int(nullable: false),
                        ResponseItem = c.String(),
                    })
                .PrimaryKey(t => t.ResponseId);
            
            CreateTable(
                "dbo.AppSettings",
                c => new
                    {
                        AppSettingId = c.Int(nullable: false, identity: true),
                        AppSettingName = c.String(),
                        AppSettingValue = c.String(),
                    })
                .PrimaryKey(t => t.AppSettingId);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        FileId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        QuestionnaireId = c.Int(nullable: false),
                        QuestionnaireQCategoryId = c.Int(nullable: false),
                        QCategoryName = c.String(),
                        FileName = c.String(),
                        Description = c.String(),
                        FileBytes = c.Binary(),
                    })
                .PrimaryKey(t => t.FileId);
            
            CreateTable(
                "dbo.QCategories",
                c => new
                    {
                        QCategoryId = c.Int(nullable: false, identity: true),
                        QCategoryName = c.String(),
                        Instructions = c.String(),
                    })
                .PrimaryKey(t => t.QCategoryId);
            
            CreateTable(
                "dbo.QTypes",
                c => new
                    {
                        QTypeName = c.String(nullable: false, maxLength: 128),
                        QTypeResponse = c.String(),
                    })
                .PrimaryKey(t => t.QTypeName);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        QuestionId = c.Int(nullable: false, identity: true),
                        QTitle = c.String(),
                        QuestionText = c.String(),
                        QTypeName = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.QuestionId)
                .ForeignKey("dbo.QTypes", t => t.QTypeName)
                .Index(t => t.QTypeName);
            
            CreateTable(
                "dbo.QuestionnaireQCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        QuestionnaireId = c.Int(),
                        QCategoryId = c.Int(),
                        Ordinal = c.Int(nullable: false),
                        SubOrdinal = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QCategories", t => t.QCategoryId)
                .ForeignKey("dbo.Questionnaires", t => t.QuestionnaireId)
                .Index(t => t.QCategoryId)
                .Index(t => t.QuestionnaireId);
            
            CreateTable(
                "dbo.Questionnaires",
                c => new
                    {
                        QuestionnaireId = c.Int(nullable: false, identity: true),
                        QuestionnaireName = c.String(),
                    })
                .PrimaryKey(t => t.QuestionnaireId);
            
            CreateTable(
                "dbo.QuestionnaireQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Ordinal = c.Int(nullable: false),
                        QuestionnaireId = c.Int(),
                        QuestionId = c.Int(),
                        QQCategoryId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.QuestionId)
                .ForeignKey("dbo.Questionnaires", t => t.QuestionnaireId)
                .ForeignKey("dbo.QuestionnaireQCategories", t => t.QQCategoryId)
                .Index(t => t.QuestionId)
                .Index(t => t.QuestionnaireId)
                .Index(t => t.QQCategoryId);
            
            CreateTable(
                "dbo.Verifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionnaireId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        QCategoryId = c.Int(nullable: false),
                        SubOrdinal = c.Int(nullable: false),
                        ItemInfo = c.String(),
                        ItemVerified = c.Boolean(nullable: false),
                        ItemStepLevel = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResponseAnswers",
                c => new
                    {
                        Response_ResponseId = c.Int(nullable: false),
                        Answer_AnswerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Response_ResponseId, t.Answer_AnswerId })
                .ForeignKey("dbo.Responses", t => t.Response_ResponseId, cascadeDelete: true)
                .ForeignKey("dbo.Answers", t => t.Answer_AnswerId, cascadeDelete: true)
                .Index(t => t.Response_ResponseId)
                .Index(t => t.Answer_AnswerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuestionnaireQCategories", "QuestionnaireId", "dbo.Questionnaires");
            DropForeignKey("dbo.QuestionnaireQuestions", "QQCategoryId", "dbo.QuestionnaireQCategories");
            DropForeignKey("dbo.QuestionnaireQuestions", "QuestionnaireId", "dbo.Questionnaires");
            DropForeignKey("dbo.QuestionnaireQuestions", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.QuestionnaireQCategories", "QCategoryId", "dbo.QCategories");
            DropForeignKey("dbo.Questions", "QTypeName", "dbo.QTypes");
            DropForeignKey("dbo.Answers", "QTypeName", "dbo.QTypes");
            DropForeignKey("dbo.ResponseAnswers", "Answer_AnswerId", "dbo.Answers");
            DropForeignKey("dbo.ResponseAnswers", "Response_ResponseId", "dbo.Responses");
            DropIndex("dbo.QuestionnaireQCategories", new[] { "QuestionnaireId" });
            DropIndex("dbo.QuestionnaireQuestions", new[] { "QQCategoryId" });
            DropIndex("dbo.QuestionnaireQuestions", new[] { "QuestionnaireId" });
            DropIndex("dbo.QuestionnaireQuestions", new[] { "QuestionId" });
            DropIndex("dbo.QuestionnaireQCategories", new[] { "QCategoryId" });
            DropIndex("dbo.Questions", new[] { "QTypeName" });
            DropIndex("dbo.Answers", new[] { "QTypeName" });
            DropIndex("dbo.ResponseAnswers", new[] { "Answer_AnswerId" });
            DropIndex("dbo.ResponseAnswers", new[] { "Response_ResponseId" });
            DropTable("dbo.ResponseAnswers");
            DropTable("dbo.Verifications");
            DropTable("dbo.QuestionnaireQuestions");
            DropTable("dbo.Questionnaires");
            DropTable("dbo.QuestionnaireQCategories");
            DropTable("dbo.Questions");
            DropTable("dbo.QTypes");
            DropTable("dbo.QCategories");
            DropTable("dbo.Files");
            DropTable("dbo.AppSettings");
            DropTable("dbo.Responses");
            DropTable("dbo.Answers");
        }
    }
}
