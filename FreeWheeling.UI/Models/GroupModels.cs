﻿using FreeWheeling.Domain.Abstract;
using FreeWheeling.Domain.Entities;
using FreeWheeling.UI.DataContexts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FreeWheeling.UI.Infrastructure;
using System.Web.Script.Serialization;

namespace FreeWheeling.UI.Models
{
    public class GroupModel
    {
        public string Name;
        public List<Group> _Groups;
        public List<int> CurrentGroupMembership;
        public List<NextRideDetails> _NextRideDetails;
        public string UserLocation;
        public string title;
        public List<int> _OwnerGroupList;
        public List<Group> PrivateBunches;
        public IEnumerable<NextRideGroupbyDayOfWeek> _NextRideGroupbyDayOfWeek;
    }

    public class NextRideGroupbyDayOfWeek
    {
        public DayOfWeek Thedayofweek;
        public List<int> Groupids;
    } 

    public class GroupModelHelper
    {
        private IdentityDb idb = new IdentityDb(); 
        private ICycleRepository repository;

        public GroupModelHelper(ICycleRepository repoParam)
        {
            repository = repoParam;
        }

        public GroupModel PopulateGroupModel(string UserId, Location _Location, string searchString, string Email)
        {
            GroupModel _GroupModel = new GroupModel();

            if (!String.IsNullOrEmpty(searchString))
            {
                    _GroupModel._Groups = repository.GetGroupsByLocationWithSearch(_Location.id, searchString).ToList();
                    _GroupModel.title = "All bunches";
                           }
            else
            {
                    _GroupModel._Groups = repository.GetGroupsByLocation(_Location.id).ToList();
                    _GroupModel.title = "All bunches";
            }

            _GroupModel.PrivateBunches = repository.GetPrivateGroupsByUserID(UserId,
                       _Location);

            _GroupModel._NextRideDetails = new List<NextRideDetails>();
            _GroupModel.UserLocation = _Location.Name;
            CultureHelper _CultureHelper = new CultureHelper(repository);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(_Location.id);
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone);
            _GroupModel._OwnerGroupList = new List<int>();

            foreach (Group item in _GroupModel._Groups)
            {
                int RideCount = item.Rides.Count();

                //If Ride count does not equal rides greater than now then there are old ride so 
                //call delete rides and populate new from latest ride date, note this should have been done by console app
                if (item.Rides.Where(t => t.RideDate >= LocalNow).Count() != RideCount) 
                {
                    repository.DeleteOldRides(item.id,TZone);
                    //If no ride is greater than localtime then just need to set as current date to use.
                    DateTime _NextDateTime = item.Rides.OrderByDescending(g => g.RideDate).Select(h => h.RideDate).FirstOrDefault();
                    if (_NextDateTime == DateTime.MinValue)
                        _NextDateTime = DateTime.Now;
                    repository.PopulateRideDatesFromDate(item, _NextDateTime,TZone);
                }

                item.Rides = item.Rides.Where(t => t.RideDate >= LocalNow).ToList();
                Ride NextRide = repository.GetClosestNextRide(item, TZone);

                if (NextRide != null)
                {
                    _GroupModel._NextRideDetails.Add(new NextRideDetails { Date = NextRide.RideDate,
                        GroupId = item.id,
                        NumberofRiders = NextRide.Riders.Where(i => i.PercentKeen == "100").Count() });
                }
                else
                {
                    if (item.RideDays != null)
                    {
                        repository.PopulateRideDates(item,TZone);
                        repository.Save();
                        NextRide = repository.GetClosestNextRide(item, TZone);
                        _GroupModel._NextRideDetails.Add(new NextRideDetails { Date = NextRide.RideDate,
                            GroupId = item.id,
                            NumberofRiders = NextRide.Riders.Where(i => i.PercentKeen == "100").Count() });
                    }
                }

                if (repository.IsGroupCreator(item.id, UserId))
                {
                    _GroupModel._OwnerGroupList.Add(item.id);
                }
            }

            //Same as above but for Private bunches.
            //TODO: think about combining functions.
            foreach (Group item in _GroupModel.PrivateBunches)
            {
                int RideCount = item.Rides.Count();

                //If Ride count does not equal rides greater than now then there are old ride so 
                //call delete rides and populate new from latest ride date, note this should have been done by console app
                if (item.Rides.Where(t => t.RideDate >= LocalNow).Count() != RideCount)
                {
                    repository.DeleteOldRides(item.id, TZone);
                    repository.PopulateRideDatesFromDate(item, item.Rides.OrderByDescending(g => g.RideDate).Select(h => h.RideDate).FirstOrDefault(), TZone);
                }

                item.Rides = item.Rides.Where(t => t.RideDate >= LocalNow).ToList();
                Ride NextRide = repository.GetClosestNextRide(item, TZone);

                if (NextRide != null)
                {
                    _GroupModel._NextRideDetails.Add(new NextRideDetails
                    {
                        Date = NextRide.RideDate,
                        GroupId = item.id,
                        NumberofRiders = NextRide.Riders.Where(i => i.PercentKeen == "100").Count()
                    });
                }
                else
                {
                    if (item.RideDays != null)
                    {
                        repository.PopulateRideDates(item, TZone);
                        repository.Save();
                        NextRide = repository.GetClosestNextRide(item, TZone);
                        _GroupModel._NextRideDetails.Add(new NextRideDetails
                        {
                            Date = NextRide.RideDate,
                            GroupId = item.id,
                            NumberofRiders = NextRide.Riders.Where(i => i.PercentKeen == "100").Count()
                        });
                    }
                }

                if (repository.IsGroupCreator(item.id, UserId))
                {
                    _GroupModel._OwnerGroupList.Add(item.id);
                }
            }


            _GroupModel._NextRideGroupbyDayOfWeek = _GroupModel._NextRideDetails.GroupBy(g => g.Date.DayOfWeek, g => g.GroupId, (key, p) =>
                                                            new NextRideGroupbyDayOfWeek { Thedayofweek = key, Groupids = p.ToList() }).OrderBy(i => i.Thedayofweek);

            //Show private first.
            _GroupModel._Groups.OrderByDescending(g => g.IsPrivate);
            _GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(UserId);

            return _GroupModel;
        }
    }

    public class AdHocRidesModel
    {
        public List<Ad_HocRide> _Ad_HocRide { get; set; }
        public List<Ad_HocRide> PrivateRandomBunches { get; set; }
    }

    public class DayOfWeekViewModel
    {
        public string Name { get; set; }    
        public bool Checked { get; set; }
    }

    public class AdHocCreateModel
    {
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date in the format dd/mm/yyyy")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime RideDate { get; set; }
        [Required(ErrorMessage = "Hour is required")]
        [Range(0, 24, ErrorMessage = "Between 0 and 24")]
        public int Hour { get; set; }
        [Required(ErrorMessage = "Minute is required")]
        [Range(0, 60, ErrorMessage = "Between 0 and 60")]
        public int Minute { get; set; }
        [Required(ErrorMessage = "Please select a date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public string DateString { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required]
        public int LocationsId { get; set; }
        public List<Location> Locations { get; set; }
        [Required(ErrorMessage = "Start location is required")]
        public string StartLocation { get; set; }
        public string AverageSpeed { get; set; }
        public string Creator { get; set; }
        [Required(ErrorMessage = "Your name is required")]
        public string CreatorName { get; set; }
        public string Description { get; set; }
        public string MapUrl { get; set; }
        public bool IsPrivate { get; set; }
        public List<InviteUser> InviteUsers { get; set; }
    }

    public class InviteUser
    {
        public string UserName { get; set; }
    }

    public class InviteOthersToPrivateBunchModel
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public List<InviteUser> InviteUsers { get; set; }
    }
    
    public class GroupCreateModel
    {

    public GroupCreateModel()
    {
        DaysOfWeek = new[]
        {
            new DayOfWeekViewModel { Name = "Monday" },
            new DayOfWeekViewModel { Name = "Tuesday" }, 
            new DayOfWeekViewModel { Name = "Wednesday" },
            new DayOfWeekViewModel { Name = "Thursday" },
            new DayOfWeekViewModel { Name = "Friday" },
            new DayOfWeekViewModel { Name = "Saturday" },
            new DayOfWeekViewModel { Name = "Sunday" },
        }.ToList();
    }

    public string AM_PM { get; set; }
    public IEnumerable<SelectListItem> AM_PMList
    {
        get
        {
            return new[]
            {
                new SelectListItem { Value = "AM", Text = "AM" },
                new SelectListItem { Value = "PM", Text = "PM" }
            };
        }
    }
    [Required]
    public IList<DayOfWeekViewModel> DaysOfWeek { get; set; }
    [Required(ErrorMessage="Name is required")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Hour is required")]
    [Range(0,24,ErrorMessage="Between 0 and 24")]
    public int Hour { get; set; }
    [Required(ErrorMessage = "Minute is required")]
    [Range(0, 60, ErrorMessage = "Between 0 and 60")]
    public int Minute { get; set; }
    [Required(ErrorMessage = "Please select a location")]
    public int LocationsId { get; set; }
    public List<Location> Locations { get; set; }
    [Required(ErrorMessage = "Start location is required")]
    public string StartLocation { get; set; }
    public string AverageSpeed { get; set; }
    public string Description { get; set; }
    public string MapUrl { get; set; }
    public bool IsPrivate { get; set; }
    [Required(ErrorMessage = "Your name is require")]
    public string CreatorName { get; set; }
    }

    public class MyGroupsModel
    {
        public int id { get; set; }
        public List<Group> CycleGroups { get;set; }
    }

    public class NextRideDetails
    {
        public int GroupId { get; set; }
        public DateTime Date { get; set; }
        public int NumberofRiders { get; set; }
    }

    public class MoreGroupDetailsModel
    {
        public string StartLocation {get;set;}
        public string AverageSpeed {get;set;}
        public string Description {get; set;}
        public string CreatedByName { get; set; }
    }

    public class EditGroupModel
    {
        public EditGroupModel()
        {
            DaysOfWeek = new[]
            {
                new DayOfWeekViewModel { Name = "Monday" },
                new DayOfWeekViewModel { Name = "Tuesday" },
                new DayOfWeekViewModel { Name = "Wednesday" },
                new DayOfWeekViewModel { Name = "Thursday" },
                new DayOfWeekViewModel { Name = "Friday" },
                new DayOfWeekViewModel { Name = "Saturday" },
                new DayOfWeekViewModel { Name = "Sunday" },
            }.ToList();
        }

        public int GroupId { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Start location is required")]
        public string StartLocation { get; set; }
        public string AverageSpeed { get; set; }
        [Required(ErrorMessage = "Hour is required")]
        [Range(0, 24, ErrorMessage = "Between 0 and 24")]
        public int Hour { get; set; }
        [Required(ErrorMessage = "Minute is required")]
        [Range(0, 60, ErrorMessage = "Between 0 and 60")]
        public int Minute { get; set; }
        [Required(ErrorMessage = "Your Name is required")]
        public string CreatorName { get; set; } 
        public int LocationsId { get; set; }
        public List<Location> Locations { get; set; }
        public IList<DayOfWeekViewModel> DaysOfWeek { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
        public string MapUrl { get; set; }
    }

    public class DeleteGroupModel
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
    }
}