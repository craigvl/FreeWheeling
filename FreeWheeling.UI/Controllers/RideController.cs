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
using PusherServer;
using System.Threading.Tasks;

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
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            RideModelIndex RideModel = new RideModelIndex();
            
            Group _Group = repository.GetGroupByID(groupid);
            RideModelHelper _RideHelper = new RideModelHelper(repository);
            RideModel = _RideHelper.PopulateRideModel(rideid, groupid, currentUser.Id, true, FromFavPage);

            if (RideModel.Ride != null)
            {              
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
                DateTime da = DateTime.ParseExact(_EditAdHocRideModel.DateString, "dd/MM/yyyy", null);
                DateTime _RideDate = da.Date.Add(new TimeSpan(_EditAdHocRideModel.RideHour, _EditAdHocRideModel.RideMinute, 0));

                if (_RideDate < LocalNow)
                {

                    ModelState.AddModelError(string.Empty, "Please select date and time that is greater than current date and time");
                    _EditAdHocRideModel.Locations = repository.GetLocations().ToList();
                    _EditAdHocRideModel.LocationsId = _Location.id;
                    return View(_EditAdHocRideModel);

                }

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
        public ActionResult AddAdHocComment(int adhocrideid, string CommentString)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            if (CommentString != string.Empty)
            {

                repository.AddAdHocRideComment(CommentString, adhocrideid, currentUser.UserName);
                repository.Save();

                AdHocViewModel _adHocViewModel = new AdHocViewModel();
                RideModelHelper _AdHocHelper = new RideModelHelper(repository);
                _adHocViewModel = _AdHocHelper.PopulateAdHocModel(adhocrideid, currentUser.Id);

                Task T = new Task(() =>
                {
                    int CommentCount = repository.GetCommentCountForAdHocRide(adhocrideid);
                    var pusher = new Pusher("65360", "dba777635636cbc16582", "5205ac0b6d4b64b0ecee");
                    var result = pusher.Trigger("BunchyRide" + adhocrideid, "New-CommentsAdHoc", new
                    {
                        rideid = adhocrideid,
                        message = CommentString,
                        commentcount = CommentCount,
                        username = User.Identity.Name
                    });
                });

                T.Start();

                return View("ViewAdHocRide", _adHocViewModel);
            }
            else
            {
                AdHocViewModel _adHocViewModel = new AdHocViewModel();
                RideModelHelper _AdHocHelper = new RideModelHelper(repository);
                _adHocViewModel = _AdHocHelper.PopulateAdHocModel(adhocrideid, currentUser.Id);
                return View("ViewAdHocRide", _adHocViewModel);
            }
        }

        [HttpPost]
        public ActionResult AddComment(int groupid, int rideid, string CommentString, bool FromFavPage, int ParentRideID)
        {          
            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            if(CommentString != string.Empty)
            {            
                repository.AddRideComment(CommentString, rideid, currentUser.UserName);
                repository.Save();

                RideModelIndex RideModel = new RideModelIndex();
                RideModelHelper _RideHelper = new RideModelHelper(repository);
                RideModel = _RideHelper.PopulateRideModel(ParentRideID, groupid, currentUser.Id, false, FromFavPage);

                Task T = new Task(() =>
                {
                    int CommentCount = repository.GetCommentCountForRide(rideid);
                    var pusher = new Pusher("65360", "dba777635636cbc16582", "5205ac0b6d4b64b0ecee");
                    var result = pusher.Trigger("BunchyRide" + ParentRideID, "New-Comments", new { rideid = rideid,
                        message = CommentString,
                        commentcount = CommentCount,
                        username = User.Identity.Name
                    });
                });

                T.Start();

                return View("Index", RideModel);

            }
            else
            {
                RideModelIndex RideModel = new RideModelIndex();
                RideModelHelper _RideHelper = new RideModelHelper(repository);
                RideModel = _RideHelper.PopulateRideModel(rideid, groupid, currentUser.Id, false, FromFavPage);
                return View("Index", RideModel);
            }

        }

        public ActionResult Attend(int RideId, string Commitment, int Groupid, bool FromFavPage, int ParentRideID)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Ride _Ride = new Ride();
            Group _Group = new Group();

            _Ride = repository.GetRideByID(RideId);
            _Group = repository.GetGroupByID(Groupid);

            Rider _Rider = new Rider { userId = currentUser.Id,
                                       Name = currentUser.UserName,
                                       Ride = _Ride,
                                       LeaveTime = DateTime.UtcNow,
                                       PercentKeen = Commitment };
                
            repository.AddRider(_Rider, _Group);
            repository.Save();

            RideModelIndex RideModel = new RideModelIndex();
            RideModelHelper _RideHelper = new RideModelHelper(repository);
            RideModel = _RideHelper.PopulateRideModel(ParentRideID, Groupid, currentUser.Id, false, FromFavPage);

            Task T = new Task(() =>
               {
                   var pusher = new Pusher("65360", "dba777635636cbc16582", "5205ac0b6d4b64b0ecee");

                   if (Commitment == "In")
                   {
                       var result = pusher.Trigger("BunchyRide" + ParentRideID, "You-In", new
                       {
                           rideid = RideId,
                           message = Commitment,
                           keencount = (repository.GetKeenCountForRide(RideId)),
                           username = User.Identity.GetUserName()
                       });
                   }

                   if (Commitment == "Out")
                   {
                       var result = pusher.Trigger("BunchyRide" + ParentRideID, "You-In", new
                       {
                           rideid = RideId,
                           message = Commitment,
                           keencount = (repository.GetKeenCountForRide(RideId)),
                           username = User.Identity.GetUserName()
                       });
                   }

                   if (Commitment == "OnWay")
                   {
                       var result = pusher.Trigger("BunchyRide" + ParentRideID, "You-In", new
                       {
                           rideid = RideId,
                           message = Commitment,
                           keencount = (repository.GetKeenCountForRide(RideId)),
                           username = User.Identity.GetUserName(),
                           leavetime = DateTime.UtcNow
                       });
                   }
               });

            T.Start();
            return View("Index", RideModel);
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