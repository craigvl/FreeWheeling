namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class riderequired : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AdHocComments", "AdHocRide_id", "dbo.Ad_HocRide");
            DropForeignKey("dbo.AdHocRiders", "AdHocRide_id", "dbo.Ad_HocRide");
            DropForeignKey("dbo.Riders", "Ride_id", "dbo.Rides");
            DropForeignKey("dbo.Comments", "Ride_id", "dbo.Rides");
            DropForeignKey("dbo.Rides", "Group_id", "dbo.Groups");
            DropIndex("dbo.AdHocComments", new[] { "AdHocRide_id" });
            DropIndex("dbo.AdHocRiders", new[] { "AdHocRide_id" });
            DropIndex("dbo.Riders", new[] { "Ride_id" });
            DropIndex("dbo.Comments", new[] { "Ride_id" });
            DropIndex("dbo.Rides", new[] { "Group_id" });
            AlterColumn("dbo.AdHocComments", "AdHocRide_id", c => c.Int(nullable: false));
            AlterColumn("dbo.AdHocRiders", "AdHocRide_id", c => c.Int(nullable: false));
            AlterColumn("dbo.Comments", "Ride_id", c => c.Int(nullable: false));
            AlterColumn("dbo.Rides", "Group_id", c => c.Int(nullable: false));
            AlterColumn("dbo.Riders", "Ride_id", c => c.Int(nullable: false));
            CreateIndex("dbo.AdHocComments", "AdHocRide_id");
            CreateIndex("dbo.AdHocRiders", "AdHocRide_id");
            CreateIndex("dbo.Riders", "Ride_id");
            CreateIndex("dbo.Comments", "Ride_id");
            CreateIndex("dbo.Rides", "Group_id");
            AddForeignKey("dbo.AdHocComments", "AdHocRide_id", "dbo.Ad_HocRide", "id", cascadeDelete: true);
            AddForeignKey("dbo.AdHocRiders", "AdHocRide_id", "dbo.Ad_HocRide", "id", cascadeDelete: true);
            AddForeignKey("dbo.Riders", "Ride_id", "dbo.Rides", "id", cascadeDelete: true);
            AddForeignKey("dbo.Comments", "Ride_id", "dbo.Rides", "id", cascadeDelete: true);
            AddForeignKey("dbo.Rides", "Group_id", "dbo.Groups", "id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rides", "Group_id", "dbo.Groups");
            DropForeignKey("dbo.Comments", "Ride_id", "dbo.Rides");
            DropForeignKey("dbo.Riders", "Ride_id", "dbo.Rides");
            DropForeignKey("dbo.AdHocRiders", "AdHocRide_id", "dbo.Ad_HocRide");
            DropForeignKey("dbo.AdHocComments", "AdHocRide_id", "dbo.Ad_HocRide");
            DropIndex("dbo.Rides", new[] { "Group_id" });
            DropIndex("dbo.Comments", new[] { "Ride_id" });
            DropIndex("dbo.Riders", new[] { "Ride_id" });
            DropIndex("dbo.AdHocRiders", new[] { "AdHocRide_id" });
            DropIndex("dbo.AdHocComments", new[] { "AdHocRide_id" });
            AlterColumn("dbo.Riders", "Ride_id", c => c.Int());
            AlterColumn("dbo.Rides", "Group_id", c => c.Int());
            AlterColumn("dbo.Comments", "Ride_id", c => c.Int());
            AlterColumn("dbo.AdHocRiders", "AdHocRide_id", c => c.Int());
            AlterColumn("dbo.AdHocComments", "AdHocRide_id", c => c.Int());
            CreateIndex("dbo.Rides", "Group_id");
            CreateIndex("dbo.Comments", "Ride_id");
            CreateIndex("dbo.Riders", "Ride_id");
            CreateIndex("dbo.AdHocRiders", "AdHocRide_id");
            CreateIndex("dbo.AdHocComments", "AdHocRide_id");
            AddForeignKey("dbo.Rides", "Group_id", "dbo.Groups", "id");
            AddForeignKey("dbo.Comments", "Ride_id", "dbo.Rides", "id");
            AddForeignKey("dbo.Riders", "Ride_id", "dbo.Rides", "id");
            AddForeignKey("dbo.AdHocRiders", "AdHocRide_id", "dbo.Ad_HocRide", "id");
            AddForeignKey("dbo.AdHocComments", "AdHocRide_id", "dbo.Ad_HocRide", "id");
        }
    }
}
