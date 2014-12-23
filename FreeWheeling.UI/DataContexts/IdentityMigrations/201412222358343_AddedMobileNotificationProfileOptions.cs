namespace FreeWheeling.UI.DataContexts.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMobileNotificationProfileOptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ReceiveMobileNotifications", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "ReceiveMobileFollowingNotifications", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "ReceiveMobileKeenNotifications", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "ReceiveMobileGroupNotifications", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ReceiveMobileGroupNotifications");
            DropColumn("dbo.AspNetUsers", "ReceiveMobileKeenNotifications");
            DropColumn("dbo.AspNetUsers", "ReceiveMobileFollowingNotifications");
            DropColumn("dbo.AspNetUsers", "ReceiveMobileNotifications");
        }
    }
}
