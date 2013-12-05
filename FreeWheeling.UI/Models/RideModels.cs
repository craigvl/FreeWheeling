using FreeWheeling.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreeWheeling.UI.Models
{
    public class RideModelIndex
    {
        public DateTime RideDate { get; set; }
        public string RideTime { get; set; }
        public Group Group { get; set; }
        public List<Ride> Rides { get; set; } 
        public List<Rider> Riders { get; set; } 
        public List<Route> Routes { get; set; }
    }
}