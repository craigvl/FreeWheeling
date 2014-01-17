namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Description : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ad_HocRide", "Description", c => c.String());
            AddColumn("dbo.Groups", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "Description");
            DropColumn("dbo.Ad_HocRide", "Description");
        }
    }
}
