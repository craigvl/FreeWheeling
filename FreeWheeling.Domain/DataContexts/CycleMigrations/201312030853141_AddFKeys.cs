namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFKeys : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        IsPrivate = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userId = c.String(),
                        Group_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Groups", t => t.Group_id)
                .Index(t => t.Group_id);
            
            CreateTable(
                "dbo.Rides",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Group_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Groups", t => t.Group_id)
                .Index(t => t.Group_id);
            
            CreateTable(
                "dbo.Routes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Group_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Groups", t => t.Group_id)
                .Index(t => t.Group_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Routes", "Group_id", "dbo.Groups");
            DropForeignKey("dbo.Rides", "Group_id", "dbo.Groups");
            DropForeignKey("dbo.Members", "Group_id", "dbo.Groups");
            DropIndex("dbo.Routes", new[] { "Group_id" });
            DropIndex("dbo.Rides", new[] { "Group_id" });
            DropIndex("dbo.Members", new[] { "Group_id" });
            DropTable("dbo.Routes");
            DropTable("dbo.Rides");
            DropTable("dbo.Members");
            DropTable("dbo.Groups");
        }
    }
}
