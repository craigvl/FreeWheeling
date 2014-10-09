namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Location_Improvments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "GoogletzTimeZone", c => c.String());
            AddColumn("dbo.Locations", "GoogletimeZoneName", c => c.String());
            AddColumn("dbo.Locations", "dstOffset", c => c.String());
            AddColumn("dbo.Locations", "rawOffset", c => c.String());
            AddColumn("dbo.Locations", "GoogleStatus", c => c.String());
            AddColumn("dbo.Locations", "Google_ErrorMessage", c => c.String());
            AddColumn("dbo.Locations", "CurrentGoogleUTC", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Locations", "CurrentGoogleUTC");
            DropColumn("dbo.Locations", "Google_ErrorMessage");
            DropColumn("dbo.Locations", "GoogleStatus");
            DropColumn("dbo.Locations", "rawOffset");
            DropColumn("dbo.Locations", "dstOffset");
            DropColumn("dbo.Locations", "GoogletimeZoneName");
            DropColumn("dbo.Locations", "GoogletzTimeZone");
        }
    }
}
