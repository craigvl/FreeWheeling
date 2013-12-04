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
  
    }
}