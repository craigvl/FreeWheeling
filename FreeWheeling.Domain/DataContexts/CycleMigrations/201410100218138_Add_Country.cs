namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Country : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Groups", "Country", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "Country");
        }
    }
}
