namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Location_And_AdHocRide : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ad_HocRide",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        RideDate = c.DateTime(nullable: false),
                        RideTime = c.String(),
                        Location_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Locations", t => t.Location_id)
                .Index(t => t.Location_id);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            AddColumn("dbo.Groups", "Location_id", c => c.Int());
            AddColumn("dbo.Comments", "Ad_HocRide_id", c => c.Int());
            AddColumn("dbo.Riders", "Ad_HocRide_id", c => c.Int());
            CreateIndex("dbo.Groups", "Location_id");
            CreateIndex("dbo.Comments", "Ad_HocRide_id");
            CreateIndex("dbo.Riders", "Ad_HocRide_id");
            AddForeignKey("dbo.Groups", "Location_id", "dbo.Locations", "id");
            AddForeignKey("dbo.Comments", "Ad_HocRide_id", "dbo.Ad_HocRide", "id");
            AddForeignKey("dbo.Riders", "Ad_HocRide_id", "dbo.Ad_HocRide", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Riders", "Ad_HocRide_id", "dbo.Ad_HocRide");
            DropForeignKey("dbo.Ad_HocRide", "Location_id", "dbo.Locations");
            DropForeignKey("dbo.Comments", "Ad_HocRide_id", "dbo.Ad_HocRide");
            DropForeignKey("dbo.Groups", "Location_id", "dbo.Locations");
            DropIndex("dbo.Riders", new[] { "Ad_HocRide_id" });
            DropIndex("dbo.Ad_HocRide", new[] { "Location_id" });
            DropIndex("dbo.Comments", new[] { "Ad_HocRide_id" });
            DropIndex("dbo.Groups", new[] { "Location_id" });
            DropColumn("dbo.Riders", "Ad_HocRide_id");
            DropColumn("dbo.Comments", "Ad_HocRide_id");
            DropColumn("dbo.Groups", "Location_id");
            DropTable("dbo.Locations");
            DropTable("dbo.Ad_HocRide");
        }
    }
}
