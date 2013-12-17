namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_To_Group : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "StartLocation", c => c.String());
            AddColumn("dbo.Groups", "AverageSpeed", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "AverageSpeed");
            DropColumn("dbo.Groups", "StartLocation");
        }
    }
}
