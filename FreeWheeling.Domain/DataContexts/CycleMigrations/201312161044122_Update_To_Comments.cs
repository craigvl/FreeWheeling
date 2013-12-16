namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_To_Comments : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "Rider_id", "dbo.Riders");
            DropIndex("dbo.Comments", new[] { "Rider_id" });
            AddColumn("dbo.Comments", "userName", c => c.String());
            DropColumn("dbo.Comments", "Rider_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "Rider_id", c => c.Int());
            DropColumn("dbo.Comments", "userName");
            CreateIndex("dbo.Comments", "Rider_id");
            AddForeignKey("dbo.Comments", "Rider_id", "dbo.Riders", "id");
        }
    }
}
