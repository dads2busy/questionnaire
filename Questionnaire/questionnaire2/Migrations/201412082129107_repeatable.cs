namespace Questionnaire2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class repeatable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QCategories", "Repeatable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.QCategories", "Repeatable");
        }
    }
}
