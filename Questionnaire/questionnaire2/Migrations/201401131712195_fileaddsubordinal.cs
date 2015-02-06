namespace Questionnaire2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fileaddsubordinal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Files", "QCategorySubOrdinal", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Files", "QCategorySubOrdinal");
        }
    }
}
