using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using FreeWheeling.Domain.Entities;

namespace FreeWheeling.Domain.DataContexts
{
    public class CycleDb : DbContext
    {

        public CycleDb()
            : base("DefaultConnection")
        {
        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Ride> Rides { get; set; }
        public DbSet<Rider> Riders { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Ad_HocRide> Ad_HocRide { get; set; }
        public DbSet<AdHocRider> AdHocRider { get; set; }
        public DbSet<AdHocComment> AdHocComment { get; set; }
  
    }
}