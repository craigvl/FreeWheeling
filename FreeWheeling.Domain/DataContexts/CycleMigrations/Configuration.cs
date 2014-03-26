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
            
            //Location Townsville = new Location { Name = "Townsivlle" };
            //Location Cairns = new Location { Name = "Cairns" };

            //context.Locations.AddOrUpdate(Townsville);
            //context.Locations.AddOrUpdate(Cairns);

            //List<CycleDays> RideDaysHJs = new List<CycleDays>();
            //RideDaysHJs.Add(new CycleDays { DayOfWeek = "Thursday" });
            //RideDaysHJs.Add(new CycleDays { DayOfWeek = "Tuesday" });

            //Group HJs = new Group
            //{
            //    name = "HJs Group",
            //    IsPrivate = false,
            //    RideDays = RideDaysHJs.ToList(),
            //    RideTime = "5:15am",
            //    Location = Townsville
            //};
            //Group Pats = new Group
            //{
            //    name = "Pats",
            //    IsPrivate = false,
            //    RideDays = RideDaysHJs.ToList(),
            //    RideTime = "5:15am",
            //    Location = Cairns
            //};

            //context.Groups.AddOrUpdate(
            //  HJs
            //);

            //context.Groups.AddOrUpdate(
            //  Pats
            //);


            //Ride HJRide1 = new Ride { Group = HJs, RideDate = new DateTime(2014, 1, 1, 5, 30, 0), RideTime = "5:15am" };
            //Ride HJRide2 = new Ride { Group = HJs, RideDate = new DateTime(2013, 12, 28, 5, 15, 0), RideTime = "5:15am" };

            //context.Rides.AddOrUpdate(HJRide1);
            //context.Rides.AddOrUpdate(HJRide2);

            
         
        }
    }
}
