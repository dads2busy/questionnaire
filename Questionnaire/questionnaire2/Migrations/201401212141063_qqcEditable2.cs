namespace Questionnaire2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class qqcEditable2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Verifications", "QQCategoryId", c => c.Int(nullable: false));
            DropColumn("dbo.QuestionnaireQCategories", "Editable");
        }
        
        public override void Down()
        {
            AddColumn("dbo.QuestionnaireQCategories", "Editable", c => c.Boolean(nullable: false));
            DropColumn("dbo.Verifications", "QQCategoryId");
        }
    }
}
