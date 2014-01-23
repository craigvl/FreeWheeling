using FreeWheeling.Domain.Abstract;
using FreeWheeling.Domain.Entities;
using FreeWheeling.UI.DataContexts;
using FreeWheeling.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FreeWheeling.UI.Filters;
using FreeWheeling.UI.Infrastructure;
using System.Globalization;

namespace FreeWheeling.UI.Controllers
{
    [Authorize]
    public class RideController : Controller
    {
        private IdentityDb idb = new IdentityDb(); 

        private ICycleRepository repository;

        public RideController(ICycleRepository repoParam)
        {

            repository = repoParam;

        }

        [Compress]
        public ActionResult Index(int groupid, int rideid = -1, bool FromFavPage = false)
        {
            //var TimeZone = TimeZoneInfo.Local.Id;
            
            RideModelIndex RideModel = new RideModelIndex();
            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            Group _Group = repository.GetGroupByID(groupid);

            CultureHelper _CultureHelper = new CultureHelper(repository);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone);

            if (rideid == -1)
            {
                RideModel.Ride = _Group.Rides.Where(u => u.RideDate.AddHours(2) >= LocalNow).OrderBy(i => i.RideDate).FirstOrDefault();
                RideModel.NextRide = _Group.Rides.Where(u => u.RideDate > RideModel.Ride.RideDate).OrderBy(i => i.RideDate).FirstOrDefault();
                RideModel.Comments = repository.GetTop2CommentsForRide(RideModel.Ride.id);
                RideModel.RideDate = RideModel.Ride.RideDate;
                RideModel.CommentCount = repository.GetCommentCountForRide(RideModel.Ride.id);
            }
            else
            {
                int _rideid = rideid;
                RideModel.Ride = repository.GetRideByID(rideid);
                RideModel.NextRide = _Group.Rides.Where(u => u.RideDate > RideModel.Ride.RideDate).OrderBy(i => i.RideDate).FirstOrDefault();
                RideModel.Comments = repository.GetTop2CommentsForRide(RideModel.Ride.id);
                RideModel.RideDate = RideModel.Ride.RideDate;
                RideModel.CommentCount = repository.GetCommentCountForRide(RideModel.Ride.id);
            }

            if (RideModel.Ride != null)
            {
                RideModel.FromFavPage = FromFavPage;
                RideModel.Group = _Group;
                RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id,TZone);
                RideModel.Comments = repository.GetTop2CommentsForRide(RideModel.Ride.id);
                RideModel.CommentCount = repository.GetCommentCountForRide(RideModel.Ride.id);
                RideModel.IsOwner = repository.IsGroupCreator(_Group.id, currentUser.Id);
                if (_Group.MapUrl != null)
                {
                    RideModel.MapUrl = string.Concat("<iframe id=mapmyfitness_route src=https://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=http://veloroutes.org/k/%3Fr%3D", _Group.MapUrl, "&output=embed height=300px width=300px frameborder=0></iframe>");
                }
            }
            else
            {         
                GroupModel GroupModel = new GroupModel();
                GroupModel._Groups = repository.GetGroups().ToList();  
                GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);
                return RedirectToAction("index", "group", GroupModel);
            }

            return View(RideModel);
        }

        public ActionResult AddHocList()
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Member _Member = repository.GetMemberByUserID(currentUser.Id);
            AdHocRidesModel _AdHocRidesModel = new AdHocRidesModel();
            CultureHelper _CultureHelper = new CultureHelper(repository);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
            _AdHocRidesModel._Ad_HocRide = repository.GetAdHocRides(repository.GetLocations().Where(o => o.id == currentUser.LocationID).FirstOrDefault(), TZone).OrderBy(c => c.RideDate).ToList();
            return View(_AdHocRidesModel);
        }

        [Compress]
        public ActionResult ViewAdHocRide(int adhocrideid)
        {
           
            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            AdHocViewModel _adHocViewModel = new AdHocViewModel();

            RideModelHelper _AdHocHelper = new RideModelHelper(repository);

            _adHocViewModel = _AdHocHelper.PopulateAdHocModel(adhocrideid, currentUser.Id);

            return View(_adHocViewModel);
        }

        public ActionResult SeeAllComments(int RideId, int GroupId)
        {
            AllRideComments _AllRideComments = new AllRideComments();
            _AllRideComments.RideId = RideId;
            _AllRideComments.GroupId = GroupId;

            _AllRideComments.Comments = repository.GetAllCommentsForRide(RideId);

            return View(_AllRideComments);

        }

        public ActionResult SeeAllAdHocComments(int adhocrideid)
        {
            AllAdHocRideComments _AllAdHocRideComments = new AllAdHocRideComments();
            _AllAdHocRideComments.adhocrideid = adhocrideid;

            _AllAdHocRideComments.Comments = repository.GetAllCommentsForAdHocRide(adhocrideid);

            return View(_AllAdHocRideComments);

        }

        [Compress]
        public ActionResult NextRide(int RideId, int Groupid, int PreviousRideID, bool FromFavPage = false)
        {

            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            RideModelIndex RideModel = new RideModelIndex();

            RideModelHelper _RideHelper = new RideModelHelper(repository);

            RideModel = _RideHelper.PopulateRideModel(RideId, Groupid, currentUser.Id,true);
            RideModel.FromFavPage = FromFavPage;

            if(RideModel.Ride == null)
            {

                GroupModel GroupModel = new GroupModel();
                GroupModel._Groups = repository.GetGroups().ToList();              
                GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);
                return RedirectToAction("index", "group", GroupModel);

            }

            return View(RideModel);

        }

        public ActionResult DeleteAdHocRide(int adhocrideid)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            if (!repository.IsAdHocCreator(adhocrideid, currentUser.Id))
            {

                return RedirectToAction("AddHocList", "Ride");

            }
            else
            {
                Ad_HocRide CurrentRide = repository.GetAdHocRideByID(adhocrideid);
                DeleteAdHocRideModel _DeleteAdHocRideModel = new DeleteAdHocRideModel { AdHocId = adhocrideid, Name = CurrentRide.Name };
                return View(_DeleteAdHocRideModel);

            }


        }


        [HttpPost, ActionName("DeleteAdHocRide")]
        public ActionResult DeleteConfirmed(int adhocrideid)
        {

            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            if (!repository.IsAdHocCreator(adhocrideid, currentUser.Id))
            {

                return RedirectToAction("AddHocList", "Ride");

            }
            else
            {
                repository.DeleteAdHocRide(adhocrideid);
                repository.Save();
                return RedirectToAction("AddHocList", "Ride");
            }


        }

        public ActionResult EditAdHocRide(int adhocrideid)
        {

            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            if (!repository.IsAdHocCreator(adhocrideid,currentUser.Id))
            {

                 return RedirectToAction("AddHocList", "Ride");

            }
            else
            {

                CultureHelper _CultureHelper = new CultureHelper(repository);
                TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
                DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone);
                Ad_HocRide CurrentRide = repository.GetAdHocRideByID(adhocrideid);

                EditAdHocRideModel _EditAdHocRideModel = new EditAdHocRideModel
                {
                    AverageSpeed = CurrentRide.AverageSpeed,
                    Locations = repository.GetLocations().ToList(),
                    Name = CurrentRide.Name,
                    RideDate = CurrentRide.RideDate,
                    RideHour = CurrentRide.RideHour,
                    RideMinute = CurrentRide.RideMinute,
                    RideTime = CurrentRide.RideTime,
                    StartLocation = CurrentRide.StartLocation,
                    LocationsId = CurrentRide.Location.id,
                    adhocrideid = adhocrideid,
                    DateString = CurrentRide.RideDate.ToString("dd/MM/yyyy"),
                    Description = CurrentRide.Description,
                    MapUrl = CurrentRide.MapUrl
                };

                return View(_EditAdHocRideModel);
            }
        }

        [HttpPost]
        public ActionResult EditAdHocRide(EditAdHocRideModel _EditAdHocRideModel)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            CultureHelper _CultureHelper = new CultureHelper(repository);
            TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone);
            Location _Location = repository.GetLocations().Where(l => l.id == _EditAdHocRideModel.LocationsId).FirstOrDefault();

            DateTime dateResult;

            if (!DateTime.TryParse(_EditAdHocRideModel.DateString, _CultureHelper.GetCulture(_Location.id), DateTimeStyles.None, out dateResult))
            {
                ModelState.AddModelError(string.Empty, "Date is not in a valid date format");
                _EditAdHocRideModel.Locations = repository.GetLocations().ToList();
                _EditAdHocRideModel.LocationsId = _Location.id;
                return View(_EditAdHocRideModel);
            }
            else
            {

                DateTime da = DateTime.ParseExact(_EditAdHocRideModel.DateString, "dd/mm/yyyy", null);
                DateTime _RideDate = da.Date.Add(new TimeSpan(_EditAdHocRideModel.RideHour, _EditAdHocRideModel.RideMinute, 0));

                Ad_HocRide adhoc = new Ad_HocRide
                {
                    AverageSpeed = _EditAdHocRideModel.AverageSpeed,
                    ModifiedTimeStamp = LocalNow,
                    Name = _EditAdHocRideModel.Name,
                    RideDate = _RideDate,
                    RideHour = _RideDate.Hour,
                    RideMinute = _RideDate.Minute,
                    RideTime = _RideDate.TimeOfDay.ToString(),
                    StartLocation = _EditAdHocRideModel.StartLocation,
                    Location = _Location,
                    id = _EditAdHocRideModel.adhocrideid,
                    Description = _EditAdHocRideModel.Description,
                    MapUrl = _EditAdHocRideModel.MapUrl
                };

                repository.UpdateAdHocRide(adhoc);
                repository.Save();

                AdHocViewModel _adHocViewModel = new AdHocViewModel();

                RideModelHelper _AdHocHelper = new RideModelHelper(repository);

                _adHocViewModel = _AdHocHelper.PopulateAdHocModel(_EditAdHocRideModel.adhocrideid, currentUser.Id);

                return View("ViewAdHocRide", _adHocViewModel);
            }

        }

        public ActionResult AddAdHocComment(int adhocrideid)
        {

            AdHocRideCommentModel _RideCommentModel = new AdHocRideCommentModel();
            _RideCommentModel.adhocrideid = adhocrideid;

            _RideCommentModel.Ride = repository.GetAdHocRideByID(adhocrideid);

            return View(_RideCommentModel);

        }

        [HttpPost]
        public ActionResult AddAdHocComment(AdHocRideCommentModel RideComment)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            repository.AddAdHocRideComment(RideComment.Comment, RideComment.adhocrideid, currentUser.UserName);
            repository.Save();

            AdHocViewModel _adHocViewModel = new AdHocViewModel();

            RideModelHelper _AdHocHelper = new RideModelHelper(repository);

            _adHocViewModel = _AdHocHelper.PopulateAdHocModel(RideComment.adhocrideid, currentUser.Id);

            return View("ViewAdHocRide", _adHocViewModel);

        }

        public ActionResult AddComment(int groupid, int RideId)
        {
            RideCommentModel _RideCommentModel = new RideCommentModel();
            _RideCommentModel.RideId = RideId;
            _RideCommentModel.GroupId = groupid;
            
            _RideCommentModel.Ride = repository.GetRideByID(RideId);

            return View(_RideCommentModel);

        }

        [HttpPost]
        public ActionResult AddComment(RideCommentModel RideComment)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            repository.AddRideComment(RideComment.Comment, RideComment.RideId, currentUser.UserName);
            repository.Save();

            RideModelIndex RideModel = new RideModelIndex();

            RideModelHelper _RideHelper = new RideModelHelper(repository);

            RideModel = _RideHelper.PopulateRideModel(RideComment.RideId, RideComment.GroupId, currentUser.Id, false);

            return View("Index", RideModel);

        }

        public ActionResult AddCommentNext(int groupid, int RideId, int PreviousRideID)
        {
            RideCommentModel _RideCommentModel = new RideCommentModel();
            _RideCommentModel.RideId = RideId;
            _RideCommentModel.GroupId = groupid;
            _RideCommentModel.PreviousRide = repository.GetRideByID(PreviousRideID);

            _RideCommentModel.Ride = repository.GetRideByID(RideId);

            return View("AddCommentNext",_RideCommentModel);

        }

        [HttpPost]
        public ActionResult AddCommentNext(RideCommentModel RideComment)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            repository.AddRideComment(RideComment.Comment, RideComment.RideId, currentUser.UserName);
            repository.Save();

            RideModelIndex RideModel = new RideModelIndex();

            RideModelHelper _RideHelper = new RideModelHelper(repository);

            RideModel = _RideHelper.PopulateRideModel(RideComment.RideId, RideComment.GroupId, currentUser.Id, true);

            return View("NextRide", RideModel);

        }

        public ActionResult Attend(int RideId, string Commitment, int Groupid)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Ride _Ride = new Ride();
            Group _Group = new Group();

            _Ride = repository.GetRideByID(RideId);
            _Group = repository.GetGroupByID(Groupid);

            Rider _Rider = new Rider { userId = currentUser.Id, Name = currentUser.UserName, Ride = _Ride, LeaveTime = DateTime.UtcNow, PercentKeen = Commitment };
                
            repository.AddRider(_Rider, _Group);
            repository.Save();

            RideModelIndex RideModel = new RideModelIndex();

            RideModelHelper _RideHelper = new RideModelHelper(repository);

            RideModel = _RideHelper.PopulateRideModel(RideId, Groupid, currentUser.Id, false);

            return View("Index", RideModel);
        }

        public ActionResult AttendNext(int RideId, string Commitment, int Groupid, int PreviousRideID)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Ride _Ride = new Ride();
            Group _Group = new Group();

            _Ride = repository.GetRideByID(RideId);
            _Group = repository.GetGroupByID(Groupid);

            Rider _Rider = new Rider { userId = currentUser.Id, Name = currentUser.UserName, Ride = _Ride, LeaveTime = DateTime.UtcNow, PercentKeen = Commitment };

            repository.AddRider(_Rider, _Group);
            repository.Save();

            RideModelIndex RideModel = new RideModelIndex();

            RideModelHelper _RideHelper = new RideModelHelper(repository);

            RideModel = _RideHelper.PopulateRideModel(RideId, Groupid, currentUser.Id, true);

            return View("NextRide", RideModel);
        }

        public ActionResult AttendAdHocRider(int adhocrideid, string Commitment)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Ad_HocRide _Ride = new Ad_HocRide();
            
            _Ride = repository.GetAdHocRideByID(adhocrideid);

            AdHocRider _Rider = new AdHocRider { userId = currentUser.Id, Name = currentUser.UserName, AdHocRide = _Ride, LeaveTime = DateTime.UtcNow, PercentKeen = Commitment };

            repository.AddAdHocRider(_Rider, _Ride);
            repository.Save();

            AdHocViewModel _adHocViewModel = new AdHocViewModel();

            RideModelHelper _AdHocHelper = new RideModelHelper(repository);

            _adHocViewModel = _AdHocHelper.PopulateAdHocModel(adhocrideid, currentUser.Id);

            return View("ViewAdHocRide", _adHocViewModel);
        }
      
	}
}