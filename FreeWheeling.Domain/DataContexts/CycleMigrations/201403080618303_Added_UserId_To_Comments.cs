namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_UserId_To_Comments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdHocComments", "userId", c => c.String());
            AddColumn("dbo.Comments", "userId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "userId");
            DropColumn("dbo.AdHocComments", "userId");
        }
    }
}
