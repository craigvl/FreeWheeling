namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_Expand : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserExpands",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        userName = c.String(),
                        FirstBunch = c.Boolean(nullable: false),
                        SecondBunch = c.Boolean(nullable: false),
                        FirstKeen = c.Boolean(nullable: false),
                        FirstComment = c.Boolean(nullable: false),
                        SecondKeen = c.Boolean(nullable: false),
                        SecondComment = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserExpands");
        }
    }
}
