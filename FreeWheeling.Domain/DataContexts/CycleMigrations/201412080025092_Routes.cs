namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Routes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RouteVotes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        ride_id = c.Int(),
                        route_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Rides", t => t.ride_id)
                .ForeignKey("dbo.Routes", t => t.route_id)
                .Index(t => t.ride_id)
                .Index(t => t.route_id);
            
            AddColumn("dbo.Routes", "MapURL", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RouteVotes", "route_id", "dbo.Routes");
            DropForeignKey("dbo.RouteVotes", "ride_id", "dbo.Rides");
            DropIndex("dbo.RouteVotes", new[] { "route_id" });
            DropIndex("dbo.RouteVotes", new[] { "ride_id" });
            DropColumn("dbo.Routes", "MapURL");
            DropTable("dbo.RouteVotes");
        }
    }
}
