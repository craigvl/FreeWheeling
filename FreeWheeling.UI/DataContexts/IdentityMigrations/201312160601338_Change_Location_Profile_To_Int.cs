namespace FreeWheeling.UI.DataContexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_Location_Profile_To_Int : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "LocationID", c => c.Int());
            DropColumn("dbo.AspNetUsers", "Location");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Location", c => c.String());
            DropColumn("dbo.AspNetUsers", "LocationID");
        }
    }
}
