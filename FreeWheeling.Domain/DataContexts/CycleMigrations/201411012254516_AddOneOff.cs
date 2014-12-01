namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOneOff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "OneOff", c => c.Boolean(nullable: false));
            AddColumn("dbo.Groups", "RideDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "RideDate");
            DropColumn("dbo.Groups", "OneOff");
        }
    }
}
