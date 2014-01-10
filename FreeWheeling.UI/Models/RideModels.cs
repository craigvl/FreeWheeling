using FreeWheeling.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FreeWheeling.UI.Models
{
    public class RideModelIndex
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime RideDate { get; set; }
        public string RideTime { get; set; }
        public Group Group { get; set; }
        public Ride Ride { get; set; }
        public Ride NextRide { get; set; }
        public Ride PreviousRide { get; set; }
        public List<Rider> Riders { get; set; } 
        public List<Route> Routes { get; set; }
        public List<Comment> Comments { get; set; }
        public int CommentCount { get; set; }
    }

    public class RideCommentModel
    {
        public int RideId { get; set; }
        public Ride Ride { get; set; }
        public string Comment { get; set; }
        public int GroupId { get; set; }
        public Ride NextRide { get; set; }
        public Ride PreviousRide { get; set; }
    }

    public class AdHocViewModel
    {
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime RideDate { get; set; }
        public string RideTime { get; set; }
        public Ad_HocRide Ride { get; set; }
        public List<AdHocRider> Riders { get; set; }
        public List<Route> Routes { get; set; }
        public List<AdHocComment> Comments { get; set; }
        public int CommentCount { get; set; }
    }

    public class AdHocRideCommentModel
    {
        public int adhocrideid { get; set; }
        public Ad_HocRide Ride { get; set; }
        public string Comment { get; set; }
    }

    public class AllRideComments
    {
        public List<Comment> Comments { get; set; }
        public int RideId { get; set; }
        public int GroupId { get; set; }
    }

    public class AllAdHocRideComments
    {
        public List<AdHocComment> Comments { get; set; }
        public int adhocrideid { get; set; }
    }
}