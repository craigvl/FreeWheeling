namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_Create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ad_HocRide",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Creator = c.String(),
                        RideDate = c.DateTime(nullable: false),
                        RideTime = c.String(),
                        StartLocation = c.String(),
                        AverageSpeed = c.String(),
                        Location_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Locations", t => t.Location_id)
                .Index(t => t.Location_id);
            
            CreateTable(
                "dbo.AdHocComments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        CommentText = c.String(),
                        Date = c.DateTime(nullable: false),
                        userName = c.String(),
                        AdHocRide_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Ad_HocRide", t => t.AdHocRide_id)
                .Index(t => t.AdHocRide_id);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.AdHocRiders",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userId = c.String(),
                        PercentKeen = c.String(),
                        LeaveTime = c.String(),
                        Name = c.String(),
                        AdHocRide_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Ad_HocRide", t => t.AdHocRide_id)
                .Index(t => t.AdHocRide_id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        CommentText = c.String(),
                        Date = c.DateTime(nullable: false),
                        userName = c.String(),
                        Ride_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Rides", t => t.Ride_id)
                .Index(t => t.Ride_id);
            
            CreateTable(
                "dbo.Rides",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        RideDate = c.DateTime(nullable: false),
                        RideTime = c.String(),
                        Group_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Groups", t => t.Group_id)
                .Index(t => t.Group_id);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        IsPrivate = c.Boolean(nullable: false),
                        RideTime = c.String(),
                        StartLocation = c.String(),
                        AverageSpeed = c.String(),
                        Location_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Locations", t => t.Location_id)
                .Index(t => t.Location_id);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userId = c.String(),
                        Group_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Groups", t => t.Group_id)
                .Index(t => t.Group_id);
            
            CreateTable(
                "dbo.CycleDays",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        DayOfWeek = c.String(),
                        Group_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Groups", t => t.Group_id)
                .Index(t => t.Group_id);
            
            CreateTable(
                "dbo.Routes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Group_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Groups", t => t.Group_id)
                .Index(t => t.Group_id);
            
            CreateTable(
                "dbo.Riders",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userId = c.String(),
                        PercentKeen = c.String(),
                        LeaveTime = c.String(),
                        Name = c.String(),
                        Ride_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Rides", t => t.Ride_id)
                .Index(t => t.Ride_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Riders", "Ride_id", "dbo.Rides");
            DropForeignKey("dbo.Routes", "Group_id", "dbo.Groups");
            DropForeignKey("dbo.Rides", "Group_id", "dbo.Groups");
            DropForeignKey("dbo.CycleDays", "Group_id", "dbo.Groups");
            DropForeignKey("dbo.Members", "Group_id", "dbo.Groups");
            DropForeignKey("dbo.Groups", "Location_id", "dbo.Locations");
            DropForeignKey("dbo.Comments", "Ride_id", "dbo.Rides");
            DropForeignKey("dbo.AdHocRiders", "AdHocRide_id", "dbo.Ad_HocRide");
            DropForeignKey("dbo.Ad_HocRide", "Location_id", "dbo.Locations");
            DropForeignKey("dbo.AdHocComments", "AdHocRide_id", "dbo.Ad_HocRide");
            DropIndex("dbo.Riders", new[] { "Ride_id" });
            DropIndex("dbo.Routes", new[] { "Group_id" });
            DropIndex("dbo.Rides", new[] { "Group_id" });
            DropIndex("dbo.CycleDays", new[] { "Group_id" });
            DropIndex("dbo.Members", new[] { "Group_id" });
            DropIndex("dbo.Groups", new[] { "Location_id" });
            DropIndex("dbo.Comments", new[] { "Ride_id" });
            DropIndex("dbo.AdHocRiders", new[] { "AdHocRide_id" });
            DropIndex("dbo.Ad_HocRide", new[] { "Location_id" });
            DropIndex("dbo.AdHocComments", new[] { "AdHocRide_id" });
            DropTable("dbo.Riders");
            DropTable("dbo.Routes");
            DropTable("dbo.CycleDays");
            DropTable("dbo.Members");
            DropTable("dbo.Groups");
            DropTable("dbo.Rides");
            DropTable("dbo.Comments");
            DropTable("dbo.AdHocRiders");
            DropTable("dbo.Locations");
            DropTable("dbo.AdHocComments");
            DropTable("dbo.Ad_HocRide");
        }
    }
}
