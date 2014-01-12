namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class another : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Ad_HocRide", "CreatedTimeStamp", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Ad_HocRide", "CreatedTimeStamp", c => c.String());
        }
    }
}
