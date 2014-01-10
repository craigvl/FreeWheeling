using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FreeWheeling.Domain.Entities;
using FreeWheeling.UI.DataContexts;
using Microsoft.AspNet.Identity;
using FreeWheeling.Domain.Abstract;
using FreeWheeling.UI.Models;

namespace FreeWheeling.UI.Controllers
{
    [Authorize]
    public class GroupController : Controller
    {
        private IdentityDb idb = new IdentityDb(); 

        private ICycleRepository repository;

        public GroupController(ICycleRepository repoParam)
        {

            repository = repoParam;

        }

        // GET: /Group/
        public ActionResult Index()
        {

            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Member _Member = repository.GetMemberByUserID(currentUser.Id);
            GroupModel _GroupModel = new GroupModel();
            _GroupModel._Groups = repository.GetGroupsByLocation(currentUser.LocationID).ToList();
            _GroupModel._NextRideDetails = new List<NextRideDetails>();
            _GroupModel.UserLocation = repository.GetLocationName(currentUser.LocationID);
            _GroupModel.title = "All Groups";

            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone);


            foreach (Group item in _GroupModel._Groups)
            {

                item.Rides = item.Rides.Where(t => t.RideDate >= LocalNow).ToList();
                Ride NextRide = repository.GetNextRideForGroup(item,TZone);
                
                if (NextRide != null)
                {
                    _GroupModel._NextRideDetails.Add(new NextRideDetails { Date = NextRide.RideDate, GroupId = item.id, NumberofRiders = NextRide.Riders.Where(i => i.PercentKeen == "100") .Count() });    
                }
                else
                {
                    if (item.RideDays != null)
                    {
                        repository.PopulateRideDates(item);
                        repository.Save();
                    }
                    
                }
                
            }

            _GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);

            return View(_GroupModel);
        }

        public PartialViewResult GetGroupDetails(int id)
        {

            Group _Group = new Group();

            _Group = repository.GetGroupByID(id);

            MoreGroupDetailsModel _MoreGroupDetailsModel = new MoreGroupDetailsModel();

            _MoreGroupDetailsModel.AverageSpeed = _Group.AverageSpeed;
            _MoreGroupDetailsModel.StartLocation = _Group.StartLocation;

            //Name of our PartialView is Restaurant
            return PartialView("_GroupDetailPartial", _MoreGroupDetailsModel);
        }

        public ActionResult CreateAdHoc()
        {
            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone);
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();

            AdHocCreateModel _Ad_HocRide = new AdHocCreateModel();
            _Ad_HocRide.Locations = repository.GetLocations().ToList();
            _Ad_HocRide.RideDate = LocalNow;
            _Ad_HocRide.DateString = LocalNow.ToShortDateString();
            _Ad_HocRide.LocationsId = _Location.id;
            _Ad_HocRide.Hour = 5;
            _Ad_HocRide.Minute = 30;
       

            return View(_Ad_HocRide);

        }

        [HttpPost]
        public ActionResult CreateAdHoc(AdHocCreateModel _AdHocCreateModel)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Location _Location = repository.GetLocations().Where(l => l.id == _AdHocCreateModel.LocationsId).FirstOrDefault();

            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");

            //DateTime da = new DateTime(_AdHocCreateModel.RideDate.Year, _AdHocCreateModel.RideDate.Month, _AdHocCreateModel.RideDate.Day,_AdHocCreateModel.Hour, _AdHocCreateModel.Minute,0);

            DateTime da = DateTime.ParseExact(_AdHocCreateModel.DateString, "dd/mm/yyyy",null);
            DateTime _RideDate = da.Date.Add(new TimeSpan(_AdHocCreateModel.Hour, _AdHocCreateModel.Minute, 0));

            Ad_HocRide NewAdHoc = new Ad_HocRide
            {
                Name = _AdHocCreateModel.Name,
                AverageSpeed = _AdHocCreateModel.AverageSpeed,
                Location = _Location,
                RideDate = _RideDate,
                Creator = currentUser.UserName,
                StartLocation = _AdHocCreateModel.StartLocation,
                RideTime = _RideDate.TimeOfDay.ToString(),
                RideHour = _RideDate.Hour,
                RideMinute = _RideDate.Minute
            };

            repository.AddAdHocRide(NewAdHoc);
            repository.Save();

            Member _Member = repository.GetMemberByUserID(currentUser.Id);
            GroupModel _GroupModel = new GroupModel();
            _GroupModel._Groups = repository.GetGroupsByLocation(currentUser.LocationID).ToList();
            _GroupModel._NextRideDetails = new List<NextRideDetails>();
            _GroupModel.UserLocation = repository.GetLocationName(currentUser.LocationID);
            _GroupModel.title = "All Groups";

            foreach (Group item in _GroupModel._Groups)
            {

                item.Rides = item.Rides.Where(t => t.RideDate >= DateTime.Now).ToList();
                Ride NextRide = repository.GetNextRideForGroup(item,TZone);

                if (NextRide != null)
                {
                    _GroupModel._NextRideDetails.Add(new NextRideDetails { Date = NextRide.RideDate, GroupId = item.id, NumberofRiders = NextRide.Riders.Where(i => i.PercentKeen == "100").Count() });
                }

            }

            _GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);

            return RedirectToAction("index", "home");   

        }


        public ActionResult Create()
        {

            GroupCreateModel _GroupCreateModel = new GroupCreateModel();
            _GroupCreateModel.Locations = repository.GetLocations().ToList();

            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();

            _GroupCreateModel.LocationsId = _Location.id;

            return View(_GroupCreateModel);

        }

        
        [HttpPost]
        public ActionResult Create(GroupCreateModel _GroupCreateModel)
        {

            List<CycleDays> _CycleDays = new List<CycleDays>();
            Location _Location = repository.GetLocations().Where(l => l.id == _GroupCreateModel.LocationsId).FirstOrDefault();
            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");

            foreach (DayOfWeekViewModel item in _GroupCreateModel.DaysOfWeek)
            {

                if (item.Checked)
                {

                    CycleDays NewDay = new CycleDays { DayOfWeek = item.Name };
                    _CycleDays.Add(NewDay);

                }

            }

            Group NewGroup = new Group
            {
                name = _GroupCreateModel.Name,
                RideTime = _GroupCreateModel.Hour.ToString() +":"+ _GroupCreateModel.Minute.ToString() + " " + _GroupCreateModel.AM_PM,
                RideDays = _CycleDays, Location = _Location, Rides = new List<Ride>(), AverageSpeed = _GroupCreateModel.AverageSpeed, 
                StartLocation = _GroupCreateModel.StartLocation,
                RideHour = _GroupCreateModel.Hour,
                RideMinute = _GroupCreateModel.Minute
            };

            repository.AddGroup(NewGroup);
            repository.Save();

            NewGroup = repository.PopulateRideDates(NewGroup);
            repository.Save();
            
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Member _Member = repository.GetMemberByUserID(currentUser.Id);
            GroupModel _GroupModel = new GroupModel();
            _GroupModel._Groups = repository.GetGroupsByLocation(currentUser.LocationID).ToList();
            _GroupModel._NextRideDetails = new List<NextRideDetails>();
            _GroupModel.UserLocation = repository.GetLocationName(currentUser.LocationID);
            _GroupModel.title = "All Groups";

            foreach (Group item in _GroupModel._Groups)
            {

                item.Rides = item.Rides.Where(t => t.RideDate >= DateTime.Now).ToList();
                Ride NextRide = repository.GetNextRideForGroup(item,TZone);

                if (NextRide != null)
                {
                    _GroupModel._NextRideDetails.Add(new NextRideDetails { Date = NextRide.RideDate, GroupId = item.id, NumberofRiders = NextRide.Riders.Where(i => i.PercentKeen == "100").Count() });
                }

            }

            _GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);

            return View("Index", _GroupModel);

        }

        public ActionResult RemoveFromFavouriteList(int id, string title)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Member _Member = repository.GetMemberByUserID(currentUser.Id);
            Group group = repository.GetGroupByID(id);

            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");

            GroupModel _GroupModel = new GroupModel();
            _GroupModel._Groups = repository.GetGroupsByLocation(currentUser.LocationID).ToList();
            _GroupModel._NextRideDetails = new List<NextRideDetails>();
            _GroupModel.UserLocation = repository.GetLocationName(currentUser.LocationID);
            _GroupModel.title = title;

            if (group == null)
            {
                return HttpNotFound();
            }

            repository.RemoveMember(currentUser.Id, group);
            repository.Save();

            _GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);

            foreach (Group item in _GroupModel._Groups)
            {

                item.Rides = item.Rides.Where(t => t.RideDate >= DateTime.Now).ToList();
                Ride NextRide = repository.GetNextRideForGroup(item,TZone);

                if (NextRide != null)
                {
                    _GroupModel._NextRideDetails.Add(new NextRideDetails { Date = NextRide.RideDate, GroupId = item.id, NumberofRiders = NextRide.Riders.Where(i => i.PercentKeen == "100").Count() });
                }

            }

            if (title == "Favourite Groups")
	        {

                _GroupModel._Groups = repository.GetGroups().Where(u => u.Members.Any(m => m.userId == currentUser.Id)).ToList();
                _GroupModel._Groups = repository.GetGroupsByLocation(currentUser.LocationID).Where(u => u.Members.Any(m => m.userId == currentUser.Id)).ToList();
                _GroupModel.title = "Favourite Groups";

                _GroupModel._NextRideDetails = new List<NextRideDetails>();
                _GroupModel.UserLocation = _GroupModel.UserLocation = repository.GetLocationName(currentUser.LocationID);

                foreach (Group item in _GroupModel._Groups)
                {

                    item.Rides = item.Rides.Where(t => t.RideDate >= DateTime.Now).ToList();
                    Ride NextRide = repository.GetNextRideForGroup(item,TZone);

                    if (NextRide != null)
                    {
                        _GroupModel._NextRideDetails.Add(new NextRideDetails { Date = NextRide.RideDate, GroupId = item.id, NumberofRiders = NextRide.Riders.Where(i => i.PercentKeen == "100").Count() });
                    }
                    
                }


                _GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);

		 
	        }
            
            return View("Index", _GroupModel);

        }

        //// GET: /Group/Details/5
        public ActionResult Join(int id, string title)
        {

            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Member _Member = repository.GetMemberByUserID(currentUser.Id);
            Group group = repository.GetGroupByID(id);
            GroupModel _GroupModel = new GroupModel();
            _GroupModel._Groups = repository.GetGroupsByLocation(currentUser.LocationID).ToList();
            _GroupModel._NextRideDetails = new List<NextRideDetails>();
            _GroupModel.UserLocation = _GroupModel.UserLocation = repository.GetLocationName(currentUser.LocationID);
            _GroupModel.title = title;
            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");

            if (group == null)
            {
                return HttpNotFound();
            }

            repository.AddMember(currentUser.Id, group);
            repository.Save();

            _GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);

            foreach (Group item in _GroupModel._Groups)
            {

                item.Rides = item.Rides.Where(t => t.RideDate >= DateTime.Now).ToList();
                Ride NextRide = repository.GetNextRideForGroup(item,TZone);

                if (NextRide != null)
                {
                    _GroupModel._NextRideDetails.Add(new NextRideDetails { Date = NextRide.RideDate, GroupId = item.id, NumberofRiders = NextRide.Riders.Where(i => i.PercentKeen == "100").Count() });
                }

            }

            return View("Index", _GroupModel);
        }

        public ViewResult MyGroups()
        {

            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Member _Member = repository.GetMemberByUserID(currentUser.Id);
            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
            GroupModel _GroupModel = new GroupModel();

            _GroupModel._Groups = repository.GetGroupsByLocation(currentUser.LocationID).Where(u => u.Members.Any(m => m.userId == currentUser.Id)).ToList();
            _GroupModel.title = "Favourite Groups";

            _GroupModel._NextRideDetails = new List<NextRideDetails>();
            _GroupModel.UserLocation = repository.GetLocationName(currentUser.LocationID);

            foreach (Group item in _GroupModel._Groups)
            {

                item.Rides = item.Rides.Where(t => t.RideDate >= DateTime.Now).ToList();
                Ride NextRide = repository.GetNextRideForGroup(item,TZone);

                if (NextRide != null)
                {
                    _GroupModel._NextRideDetails.Add(new NextRideDetails { Date = NextRide.RideDate, GroupId = item.id, NumberofRiders = NextRide.Riders.Where(i => i.PercentKeen == "100").Count() });
                }
                else
                {

                    if (item.RideDays != null)
                    {
                        repository.PopulateRideDates(item);
                    }

                }

            }


            _GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);

            return View("Index",_GroupModel);

        }

    }
}
