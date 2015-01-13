namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VoteRideRequired : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RouteVotes", "ride_id", "dbo.Rides");
            DropIndex("dbo.RouteVotes", new[] { "ride_id" });
            AlterColumn("dbo.RouteVotes", "ride_id", c => c.Int(nullable: false));
            CreateIndex("dbo.RouteVotes", "ride_id");
            AddForeignKey("dbo.RouteVotes", "ride_id", "dbo.Rides", "id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RouteVotes", "ride_id", "dbo.Rides");
            DropIndex("dbo.RouteVotes", new[] { "ride_id" });
            AlterColumn("dbo.RouteVotes", "ride_id", c => c.Int());
            CreateIndex("dbo.RouteVotes", "ride_id");
            AddForeignKey("dbo.RouteVotes", "ride_id", "dbo.Rides", "id");
        }
    }
}
