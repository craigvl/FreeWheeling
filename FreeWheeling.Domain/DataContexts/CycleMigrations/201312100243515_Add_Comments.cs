namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Comments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        CommentText = c.String(),
                        Ride_id = c.Int(),
                        Rider_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Rides", t => t.Ride_id)
                .ForeignKey("dbo.Riders", t => t.Rider_id)
                .Index(t => t.Ride_id)
                .Index(t => t.Rider_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "Rider_id", "dbo.Riders");
            DropForeignKey("dbo.Comments", "Ride_id", "dbo.Rides");
            DropIndex("dbo.Comments", new[] { "Rider_id" });
            DropIndex("dbo.Comments", new[] { "Ride_id" });
            DropTable("dbo.Comments");
        }
    }
}
