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
        public Ride Ride { get; set; } 
        public List<Rider> Riders { get; set; } 
        public List<Route> Routes { get; set; }
    }

    public class RideCommentModel
    {
        public int RideId { get; set; }
        public Ride Ride { get; set; }
        public string Comment { get; set; }
        public int GroupId { get; set; }
    }

    public class RideModel
    {



    }
}