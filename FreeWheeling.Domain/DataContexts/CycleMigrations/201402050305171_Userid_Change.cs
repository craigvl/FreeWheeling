namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Userid_Change : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserExpands", "userId", c => c.String());
            DropColumn("dbo.UserExpands", "userName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserExpands", "userName", c => c.String());
            DropColumn("dbo.UserExpands", "userId");
        }
    }
}
