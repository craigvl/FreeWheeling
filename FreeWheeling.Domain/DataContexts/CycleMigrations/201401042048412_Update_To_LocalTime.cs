namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_To_LocalTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AdHocRiders", "LeaveTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Riders", "LeaveTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Riders", "LeaveTime", c => c.String());
            AlterColumn("dbo.AdHocRiders", "LeaveTime", c => c.String());
        }
    }
}
