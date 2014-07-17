namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timezoneadd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "TimeZoneInfo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Locations", "TimeZoneInfo");
        }
    }
}
