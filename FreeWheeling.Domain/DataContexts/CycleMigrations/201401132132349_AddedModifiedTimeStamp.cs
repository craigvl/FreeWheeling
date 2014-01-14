namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedModifiedTimeStamp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Ad_HocRide", "ModifiedTimeStamp", c => c.DateTime(nullable: false));
            AddColumn("dbo.Groups", "ModifiedTimeStamp", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Groups", "ModifiedTimeStamp");
            DropColumn("dbo.Ad_HocRide", "ModifiedTimeStamp");
        }
    }
}
