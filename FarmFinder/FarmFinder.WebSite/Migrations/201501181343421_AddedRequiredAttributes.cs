using System.Data.Entity.Migrations;

namespace CH.Tutteli.FarmFinder.Website.Migrations
{
    public partial class AddedRequiredAttributes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Farms", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Farms", "Address", c => c.String(nullable: false));
            AlterColumn("dbo.Farms", "City", c => c.String(nullable: false));
            AlterColumn("dbo.Farms", "Zip", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Farms", "Zip", c => c.String());
            AlterColumn("dbo.Farms", "City", c => c.String());
            AlterColumn("dbo.Farms", "Address", c => c.String());
            AlterColumn("dbo.Farms", "Name", c => c.String());
        }
    }
}
