namespace FreeWheeling.UI.DataContexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Email_Settings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ReceiveEmails", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "ReceiveKeen", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "ReceiveComments", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "ReceiveSummary", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "TimeBefore", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "TimeBefore");
            DropColumn("dbo.AspNetUsers", "ReceiveSummary");
            DropColumn("dbo.AspNetUsers", "ReceiveComments");
            DropColumn("dbo.AspNetUsers", "ReceiveKeen");
            DropColumn("dbo.AspNetUsers", "ReceiveEmails");
        }
    }
}
