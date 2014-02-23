using FreeWheeling.Domain.Abstract;
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
        public bool OnFavPage;
    }

    public class GroupModelHelper
    {

        private IdentityDb idb = new IdentityDb(); 

        private ICycleRepository repository;

        public GroupModelHelper(ICycleRepository repoParam)
        {

            repository = repoParam;

        }

        public GroupModel PopulateGroupModel(string UserId, int? LocationId, string searchString, Boolean FavouritePage = false)
        {
            GroupModel _GroupModel = new GroupModel();

            if (!String.IsNullOrEmpty(searchString))
            {
                if (!FavouritePage)
                {
                    _GroupModel._Groups = repository.GetGroupsByLocationWithSearch(LocationId,searchString).ToList();
                    _GroupModel.title = "All bunches";
                }
                else
                {
                    _GroupModel._Groups = repository.GetFavouriteGroupsByLocationWithSearch(LocationId,searchString).Where(u => u.Members.Any(m => m.userId == UserId)).ToList();
                    _GroupModel.OnFavPage = true;
                    _GroupModel.title = "Favourite bunches";
                }


            }
            else
            {

                if (!FavouritePage)
                {
                    _GroupModel._Groups = repository.GetGroupsByLocation(LocationId).ToList();
                    _GroupModel.title = "All bunches";
                }
                else
                {
                    _GroupModel._Groups = repository.GetFavouriteGroupsByLocation(LocationId).Where(u => u.Members.Any(m => m.userId == UserId)).ToList();
                    _GroupModel.OnFavPage = true;
                    _GroupModel.title = "Favourite bunches";
                }

            }

            _GroupModel._NextRideDetails = new List<NextRideDetails>();
            _GroupModel.UserLocation = repository.GetLocationName(LocationId);

            CultureHelper _CultureHelper = new CultureHelper(repository);

            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(LocationId);
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone);

            _GroupModel._OwnerGroupList = new List<int>();

            foreach (Group item in _GroupModel._Groups)
            {
                //Delete any old rides

                repository.DeleteOldRides(item.id, TZone);

                item.Rides = item.Rides.Where(t => t.RideDate >= LocalNow).ToList();
                Ride NextRide = repository.GetNextRideForGroup(item, TZone);

                if (NextRide != null)
                {
                    _GroupModel._NextRideDetails.Add(new NextRideDetails { Date = NextRide.RideDate, GroupId = item.id, NumberofRiders = NextRide.Riders.Where(i => i.PercentKeen == "100").Count() });
                }
                else
                {
                    if (item.RideDays != null)
                    {
                        repository.PopulateRideDates(item,TZone);
                        repository.Save();
                    }

                }

                if (repository.IsGroupCreator(item.id, UserId))
                {

                    _GroupModel._OwnerGroupList.Add(item.id);

                }

            }

            _GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(UserId);

            return _GroupModel;

        }

    }

    public class AdHocRidesModel
    {
        public List<Ad_HocRide> _Ad_HocRide;
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
        public string Description { get; set; }
        public string MapUrl { get; set; }
        public List<AdHocCreateUserModel> InviteUsers { get; set; }
    }

    public class AdHocCreateUserModel
    {
        public string UserName { get; set; }   
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
        public int LocationsId { get; set; }
        public List<Location> Locations { get; set; }
        public IList<DayOfWeekViewModel> DaysOfWeek { get; set; }
        public string Description { get; set; }
        public string MapUrl { get; set; }
        
    }

    public class DeleteGroupModel
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
    }
}