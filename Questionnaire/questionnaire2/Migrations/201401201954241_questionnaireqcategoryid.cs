namespace Questionnaire2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class questionnaireqcategoryid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Responses", "QuestionnaireQCategoryId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Responses", "QuestionnaireQCategoryId");
        }
    }
}
