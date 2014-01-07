namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Ride_Hour_Minute : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ad_HocRide", "RideHour", c => c.Int(nullable: false));
            AddColumn("dbo.Ad_HocRide", "RideMinute", c => c.Int(nullable: false));
            AddColumn("dbo.Groups", "RideHour", c => c.Int(nullable: false));
            AddColumn("dbo.Groups", "RideMinute", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "RideMinute");
            DropColumn("dbo.Groups", "RideHour");
            DropColumn("dbo.Ad_HocRide", "RideMinute");
            DropColumn("dbo.Ad_HocRide", "RideHour");
        }
    }
}
