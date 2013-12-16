namespace FreeWheeling.UI.DataContexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Location_Profile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Location", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Location");
        }
    }
}
