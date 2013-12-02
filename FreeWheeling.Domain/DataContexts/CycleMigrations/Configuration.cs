namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using FreeWheeling.Domain;

    internal sealed class Configuration : DbMigrationsConfiguration<FreeWheeling.Domain.DataContexts.CycleDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"DataContexts\CycleMigrations";
        }

        protected override void Seed(FreeWheeling.Domain.DataContexts.CycleDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.

            Group HJs = new Group { name = "HJs Group", IsPrivate = false };
            Group Pats = new Group { name = "Pats", IsPrivate = false };

            context.Groups.AddOrUpdate(
              HJs
            );
            context.Groups.AddOrUpdate(
              Pats
            );
            
        }
    }
}
