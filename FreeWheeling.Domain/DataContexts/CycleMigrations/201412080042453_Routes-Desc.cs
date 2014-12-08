namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoutesDesc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Routes", "Desc", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Routes", "Desc");
        }
    }
}
