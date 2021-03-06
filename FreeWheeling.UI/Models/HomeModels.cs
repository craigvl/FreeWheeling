﻿using FreeWheeling.Domain.Entities;
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
        public int BunchCount { get; set; }
        public List<Group> FavouriteBunches { get; set; }
        public Ride HomePageRide { get; set; }
        public Ad_HocRide HomePageRandomRide { get; set; }
        public List<int> _OwnerGroupList;
        public bool IsOnWay { get; set; }
        public bool IsIn { get; set; }
        public bool IsOut { get; set; }
        public int Keencount { get; set; }
        public int Followingcount { get; set; }
    }

    public class LocationChangeModel
    {
        public int LocationsId { get; set; }
        public string CurrentUserLocation { get; set; }
        public List<Location> Locations { get; set; }
    }

    public class LocationCreate
    {
        [Required(ErrorMessage = "Location Name is required")]
        public string Name { get; set; }
        public string TimeZoneId { get; set; }
        public string GoogletzTimeZone { get; set; }
        public string GoogletimeZoneName { get; set; }
        public string dstOffset { get; set; }
        public string rawOffset { get; set; }
        public string GoogleStatus { get; set; }
        public string Google_ErrorMessage { get; set; }
        public string CurrentGoogleUTC { get; set; }
        public TimeZoneInfo TimeZone
        {
            get { return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId); }
            set { TimeZoneId = value.Id; }
        }
        public string lat { get; set; }
        public string lng { get; set; }
    }
}