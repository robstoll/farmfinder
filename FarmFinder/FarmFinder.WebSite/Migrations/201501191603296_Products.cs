namespace CH.Tutteli.FarmFinder.Website.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Products : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        FarmRefId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        InStock = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.Farms", t => t.FarmRefId, cascadeDelete: true)
                .Index(t => t.FarmRefId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "FarmRefId", "dbo.Farms");
            DropIndex("dbo.Products", new[] { "FarmRefId" });
            DropTable("dbo.Products");
        }
    }
}
