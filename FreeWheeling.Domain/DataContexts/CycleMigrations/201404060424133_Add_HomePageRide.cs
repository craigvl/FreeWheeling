namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_HomePageRide : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HomePageRides",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Rideid = c.Int(nullable: false),
                        Userid = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HomePageRides");
        }
    }
}
