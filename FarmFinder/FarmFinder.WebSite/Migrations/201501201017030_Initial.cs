namespace CH.Tutteli.FarmFinder.Website.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Farms",
                c => new
                    {
                        FarmId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        Address = c.String(nullable: false),
                        City = c.String(nullable: false),
                        Zip = c.String(nullable: false),
                        Email = c.String(),
                        Website = c.String(),
                        PhoneNumber = c.String(),
                        UpdateDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IndexDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.FarmId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        InStock = c.Boolean(nullable: false),
                        FarmRefId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        UpdateDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IndexDateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
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
            DropTable("dbo.Farms");
        }
    }
}
