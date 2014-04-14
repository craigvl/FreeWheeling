using FreeWheeling.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FreeWheeling.UI.Models
{
    public class HomeIndexModel
    {
        public int LocationsId { get; set; }
        public string CurrentUserLocation { get; set; }
        public List<Location> Locations { get; set; }
        public int UpCommingAd_HocCount { get; set; }
        public List<Group> FavouriteBunches { get; set; }
        public List<Group> PrivateBunches { get; set; }
        public List<Ad_HocRide> PrivateRandomBunches { get; set; }
        public Ride HomePageRide { get; set; }
    }

    public class LocationChangeModel
    {
        public int LocationsId { get; set; }
        public string CurrentUserLocation { get; set; }
        public List<Location> Locations { get; set; }
    }
}