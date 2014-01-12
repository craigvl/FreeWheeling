namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_CreatedBy_And_TimeStampUpdate_To_LocalTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ad_HocRide", "CreatedBy", c => c.String());
            AddColumn("dbo.Ad_HocRide", "CreatedTimeStamp", c => c.DateTime(nullable: false));
            AddColumn("dbo.Groups", "CreatedBy", c => c.String());
            AddColumn("dbo.Groups", "CreatedTimeStamp", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "CreatedTimeStamp");
            DropColumn("dbo.Groups", "CreatedBy");
            DropColumn("dbo.Ad_HocRide", "CreatedTimeStamp");
            DropColumn("dbo.Ad_HocRide", "CreatedBy");
        }
    }
}
