namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_Member_Location : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Members", "Location_id", "dbo.Locations");
            DropIndex("dbo.Members", new[] { "Location_id" });
            DropColumn("dbo.Members", "Location_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Members", "Location_id", c => c.Int());
            CreateIndex("dbo.Members", "Location_id");
            AddForeignKey("dbo.Members", "Location_id", "dbo.Locations", "id");
        }
    }
}
