namespace Questionnaire2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class latticeitems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LatticeItems",
                c => new
                    {
                        ItemId = c.Int(nullable: false, identity: true),
                        Step = c.String(),
                        LatticeLevel = c.String(),
                        ItemType = c.String(),
                        DropdownText = c.String(),
                    })
                .PrimaryKey(t => t.ItemId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LatticeItems");
        }
    }
}
