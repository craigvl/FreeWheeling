namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Rider_Name : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Riders", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Riders", "Name");
        }
    }
}
