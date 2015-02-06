namespace Questionnaire2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class third : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Verifications", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Verifications", "Notes");
        }
    }
}
