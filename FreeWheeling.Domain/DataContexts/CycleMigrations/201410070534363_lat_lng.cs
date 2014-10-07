namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lat_lng : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "Lat", c => c.String());
            AddColumn("dbo.Locations", "Lng", c => c.String());
            AddColumn("dbo.Groups", "Lat", c => c.String());
            AddColumn("dbo.Groups", "Lng", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "Lng");
            DropColumn("dbo.Groups", "Lat");
            DropColumn("dbo.Locations", "Lng");
            DropColumn("dbo.Locations", "Lat");
        }
    }
}
