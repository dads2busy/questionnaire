namespace Questionnaire2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Verifications", "Editable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Verifications", "Editable");
        }
    }
}
