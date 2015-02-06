namespace Questionnaire2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class latticeitems2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LatticeItems", "ItemName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LatticeItems", "ItemName");
        }
    }
}
