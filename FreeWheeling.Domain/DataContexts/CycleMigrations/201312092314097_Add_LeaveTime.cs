namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_LeaveTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Riders", "LeaveTime", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Riders", "LeaveTime");
        }
    }
}
