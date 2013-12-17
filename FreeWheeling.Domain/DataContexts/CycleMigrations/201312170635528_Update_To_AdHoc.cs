namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_To_AdHoc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ad_HocRide", "Name", c => c.String());
            AddColumn("dbo.Ad_HocRide", "Creator", c => c.String());
            AddColumn("dbo.Ad_HocRide", "StartLocation", c => c.String());
            AddColumn("dbo.Ad_HocRide", "AverageSpeed", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Ad_HocRide", "AverageSpeed");
            DropColumn("dbo.Ad_HocRide", "StartLocation");
            DropColumn("dbo.Ad_HocRide", "Creator");
            DropColumn("dbo.Ad_HocRide", "Name");
        }
    }
}
