namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ad_HocRide",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsPrivate = c.Boolean(nullable: false),
                        Creator = c.String(),
                        RideDate = c.DateTime(nullable: false),
                        RideTime = c.String(),
                        StartLocation = c.String(),
                        AverageSpeed = c.String(),
                        RideHour = c.Int(nullable: false),
                        RideMinute = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedTimeStamp = c.DateTime(nullable: false),
                        ModifiedTimeStamp = c.DateTime(nullable: false),
                        Description = c.String(),
                        MapUrl = c.String(),
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
                        userId = c.String(),
                        AdHocRide_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Ad_HocRide", t => t.AdHocRide_id, cascadeDelete: true)
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
                        LeaveTime = c.DateTime(nullable: false),
                        Name = c.String(),
                        AdHocRide_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Ad_HocRide", t => t.AdHocRide_id, cascadeDelete: true)
                .Index(t => t.AdHocRide_id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        CommentText = c.String(),
                        Date = c.DateTime(nullable: false),
                        userName = c.String(),
                        userId = c.String(),
                        Ride_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Rides", t => t.Ride_id, cascadeDelete: true)
                .Index(t => t.Ride_id);
            
            CreateTable(
                "dbo.Rides",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        RideDate = c.DateTime(nullable: false),
                        RideTime = c.String(),
                        Group_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Groups", t => t.Group_id, cascadeDelete: true)
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
                        RideHour = c.Int(nullable: false),
                        RideMinute = c.Int(nullable: false),
                        CreatedBy = c.String(),
                        CreatedTimeStamp = c.DateTime(nullable: false),
                        ModifiedTimeStamp = c.DateTime(nullable: false),
                        Description = c.String(),
                        MapUrl = c.String(),
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
                        Group_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Groups", t => t.Group_id, cascadeDelete: true)
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
                        LeaveTime = c.DateTime(nullable: false),
                        Name = c.String(),
                        Ride_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Rides", t => t.Ride_id, cascadeDelete: true)
                .Index(t => t.Ride_id);
            
            CreateTable(
                "dbo.HomePageRides",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Rideid = c.Int(nullable: false),
                        Userid = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.PrivateGroupUsers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        UserId = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.PrivateRandomUsers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        RideId = c.Int(nullable: false),
                        UserId = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.UserExpands",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userId = c.String(),
                        FirstBunch = c.Boolean(nullable: false),
                        FirstKeen = c.Boolean(nullable: false),
                        FirstComment = c.Boolean(nullable: false),
                        SecondBunch = c.Boolean(nullable: false),
                        SecondKeen = c.Boolean(nullable: false),
                        SecondComment = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "Ride_id", "dbo.Rides");
            DropForeignKey("dbo.Riders", "Ride_id", "dbo.Rides");
            DropForeignKey("dbo.Rides", "Group_id", "dbo.Groups");
            DropForeignKey("dbo.Routes", "Group_id", "dbo.Groups");
            DropForeignKey("dbo.CycleDays", "Group_id", "dbo.Groups");
            DropForeignKey("dbo.Members", "Group_id", "dbo.Groups");
            DropForeignKey("dbo.Groups", "Location_id", "dbo.Locations");
            DropForeignKey("dbo.AdHocRiders", "AdHocRide_id", "dbo.Ad_HocRide");
            DropForeignKey("dbo.Ad_HocRide", "Location_id", "dbo.Locations");
            DropForeignKey("dbo.AdHocComments", "AdHocRide_id", "dbo.Ad_HocRide");
            DropIndex("dbo.Riders", new[] { "Ride_id" });
            DropIndex("dbo.Routes", new[] { "Group_id" });
            DropIndex("dbo.CycleDays", new[] { "Group_id" });
            DropIndex("dbo.Members", new[] { "Group_id" });
            DropIndex("dbo.Groups", new[] { "Location_id" });
            DropIndex("dbo.Rides", new[] { "Group_id" });
            DropIndex("dbo.Comments", new[] { "Ride_id" });
            DropIndex("dbo.AdHocRiders", new[] { "AdHocRide_id" });
            DropIndex("dbo.AdHocComments", new[] { "AdHocRide_id" });
            DropIndex("dbo.Ad_HocRide", new[] { "Location_id" });
            DropTable("dbo.UserExpands");
            DropTable("dbo.PrivateRandomUsers");
            DropTable("dbo.PrivateGroupUsers");
            DropTable("dbo.HomePageRides");
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
