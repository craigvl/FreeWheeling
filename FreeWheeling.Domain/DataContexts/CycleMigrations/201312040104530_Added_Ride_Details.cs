namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Ride_Details : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "RideTime", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "RideTime");
        }
    }
}
