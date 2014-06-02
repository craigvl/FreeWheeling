namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class homerandomride : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HomePageRides", "IsRandomRide", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HomePageRides", "IsRandomRide");
        }
    }
}
