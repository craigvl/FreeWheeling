namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_MapUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ad_HocRide", "MapUrl", c => c.String());
            AddColumn("dbo.Groups", "MapUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "MapUrl");
            DropColumn("dbo.Ad_HocRide", "MapUrl");
        }
    }
}
