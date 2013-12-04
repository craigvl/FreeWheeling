namespace FreeWheeling.Domain.DataContexts.CycleMigrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using FreeWheeling.Domain.Entities;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<FreeWheeling.Domain.DataContexts.CycleDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DataContexts\CycleMigrations";
        }

        protected override void Seed(FreeWheeling.Domain.DataContexts.CycleDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.

            List<CycleDays> RideDaysHJs = new List<CycleDays>();
            RideDaysHJs.Add(new CycleDays { DayOfWeek = "Thursday" });
            RideDaysHJs.Add(new CycleDays { DayOfWeek = "Tusday" });

            Group HJs = new Group { name = "HJs Group", IsPrivate = false, RideDays = RideDaysHJs.ToList(), RideTime = "5:15am" };
            Group Pats = new Group { name = "Pats", IsPrivate = false, RideDays = RideDaysHJs.ToList(), RideTime = "5:15am" };

            context.Groups.AddOrUpdate(
              HJs
            );

            context.SaveChanges();

            context.Groups.AddOrUpdate(
              Pats
            );

            context.SaveChanges();
            
        }
    }
}
