using System.Data.Entity.Migrations;

namespace CH.Tutteli.FarmFinder.Website.Migrations
{
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.Farms",
            //    c => new
            //        {
            //            FarmId = c.Int(nullable: false, identity: true),
            //            Name = c.String(),
            //            Latitude = c.Double(nullable: false),
            //            Longitude = c.Double(nullable: false),
            //            Address = c.String(),
            //            City = c.String(),
            //            Zip = c.String(),
            //            Email = c.String(),
            //            Website = c.String(),
            //            PhoneNumber = c.String(),
            //        })
            //    .PrimaryKey(t => t.FarmId);
            
        }
        
        public override void Down()
        {
            //DropTable("dbo.Farms");
        }
    }
}
