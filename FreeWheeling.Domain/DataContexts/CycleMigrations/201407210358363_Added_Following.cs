namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Following : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserFollowingUsers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userId = c.String(),
                        followedUserId = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserFollowingUsers");
        }
    }
}
