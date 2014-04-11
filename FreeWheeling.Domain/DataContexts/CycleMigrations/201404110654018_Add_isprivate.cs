namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_isprivate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ad_HocRide", "IsPrivate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Ad_HocRide", "IsPrivate");
        }
    }
}
