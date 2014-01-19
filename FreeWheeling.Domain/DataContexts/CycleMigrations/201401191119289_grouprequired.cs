namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class grouprequired : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CycleDays", "Group_id", "dbo.Groups");
            DropIndex("dbo.CycleDays", new[] { "Group_id" });
            AlterColumn("dbo.CycleDays", "Group_id", c => c.Int(nullable: false));
            CreateIndex("dbo.CycleDays", "Group_id");
            AddForeignKey("dbo.CycleDays", "Group_id", "dbo.Groups", "id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CycleDays", "Group_id", "dbo.Groups");
            DropIndex("dbo.CycleDays", new[] { "Group_id" });
            AlterColumn("dbo.CycleDays", "Group_id", c => c.Int());
            CreateIndex("dbo.CycleDays", "Group_id");
            AddForeignKey("dbo.CycleDays", "Group_id", "dbo.Groups", "id");
        }
    }
}
