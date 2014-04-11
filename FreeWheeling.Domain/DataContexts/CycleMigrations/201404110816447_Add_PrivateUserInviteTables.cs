namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_PrivateUserInviteTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PrivateGroupUsers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        UserId = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.PrivateRandomUsers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        RideId = c.Int(nullable: false),
                        UserId = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PrivateRandomUsers");
            DropTable("dbo.PrivateGroupUsers");
        }
    }
}
