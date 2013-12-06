namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Rider : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Riders",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userId = c.String(),
                        PercentKeen = c.String(),
                        Ride_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Rides", t => t.Ride_id)
                .Index(t => t.Ride_id);
            
            AddColumn("dbo.Rides", "RideDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Rides", "RideTime", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Riders", "Ride_id", "dbo.Rides");
            DropIndex("dbo.Riders", new[] { "Ride_id" });
            DropColumn("dbo.Rides", "RideTime");
            DropColumn("dbo.Rides", "RideDate");
            DropTable("dbo.Riders");
        }
    }
}
