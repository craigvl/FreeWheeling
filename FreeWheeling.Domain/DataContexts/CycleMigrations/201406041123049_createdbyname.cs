namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createdbyname : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ad_HocRide", "CreatedByName", c => c.String());
            AddColumn("dbo.Groups", "CreatedByName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "CreatedByName");
            DropColumn("dbo.Ad_HocRide", "CreatedByName");
        }
    }
}
