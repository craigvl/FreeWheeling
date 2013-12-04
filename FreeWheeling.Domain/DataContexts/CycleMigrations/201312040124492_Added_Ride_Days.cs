namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Ride_Days : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CycleDays", "Group_id", "dbo.Groups");
            DropIndex("dbo.CycleDays", new[] { "Group_id" });
            DropTable("dbo.CycleDays");
        }
    }
}
