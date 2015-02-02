namespace Questionnaire2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uploas : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.QCategories", "Uploads", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.QCategories", "Uploads");
        }
    }
}
