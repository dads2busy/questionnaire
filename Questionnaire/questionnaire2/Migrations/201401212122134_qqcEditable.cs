namespace Questionnaire2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class qqcEditable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QuestionnaireQCategories", "Editable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.QuestionnaireQCategories", "Editable");
        }
    }
}
