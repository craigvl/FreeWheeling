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
        public Ride HomePageRide { get; set; }
        public List<int> _OwnerGroupList;
        public bool IsOnWay { get; set; }
        public bool IsIn { get; set; }
        public bool IsOut { get; set; }
    }

    public class LocationChangeModel
    {
        public int LocationsId { get; set; }
        public string CurrentUserLocation { get; set; }
        public List<Location> Locations { get; set; }
    }
}