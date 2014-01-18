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

        public ActionResult Index(int groupid, int rideid = -1)
        {
            //var TimeZone = TimeZoneInfo.Local.Id;
            
            RideModelIndex RideModel = new RideModelIndex();
            var currentUser = idb.Users.Find(User.Identity.GetUserId());


            Group _Group = repository.GetGroupByID(groupid);

            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
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
                RideModel.Group = _Group;
                RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id,TZone);
                RideModel.Comments = repository.GetTop2CommentsForRide(RideModel.Ride.id);
                RideModel.CommentCount = repository.GetCommentCountForRide(RideModel.Ride.id);
                RideModel.IsOwner = repository.IsGroupCreator(_Group.id, currentUser.Id);

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
            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
            _AdHocRidesModel._Ad_HocRide = repository.GetAdHocRides(repository.GetLocations().Where(o => o.id == currentUser.LocationID).FirstOrDefault(), TZone).OrderBy(c => c.RideDate).ToList();
            return View(_AdHocRidesModel);
        }

        public ActionResult ViewAdHocRide(int adhocrideid)
        {
            Ad_HocRide Ah = repository.GetAdHocRideByID(adhocrideid);
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            AdHocViewModel adHocViewModel = new AdHocViewModel { Ride = Ah, RideDate = Ah.RideDate, RideTime = Ah.RideTime };
            adHocViewModel.CommentCount = repository.GetCommentCountForAdHocRide(adhocrideid);
            adHocViewModel.IsOwner = repository.IsAdHocCreator(adhocrideid, currentUser.Id);
            adHocViewModel.MapUrl =
            string.Concat("<iframe id=mapmyfitness_route src=https://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=http://veloroutes.org/k/%3Fr%3D", Ah.MapUrl, "&output=embed height=300px width=300px frameborder=0></iframe>");

                //https://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=http://veloroutes.org/k/%3Fr%3D108681

            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
            adHocViewModel.Riders = repository.GetRidersForAdHocRide(adhocrideid,TZone);
           
            adHocViewModel.Comments = repository.GetTop2CommentsForAdHocRide(adhocrideid);

            return View(adHocViewModel);
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

        public ActionResult NextRide(int RideId, int Groupid, int PreviousRideID)
        {

            RideModelIndex RideModel = new RideModelIndex();

            Group _Group = repository.GetGroupByID(Groupid);

            RideModel.PreviousRide = repository.GetRideByID(PreviousRideID);

            RideModel.Ride = repository.GetRideByID(RideId);

            if (RideModel.Ride != null)
            {

                RideModel.Group = _Group;

                TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
                RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id,TZone);
                RideModel.Comments = repository.GetTop2CommentsForRide(RideModel.Ride.id);

            }
            else
            {

                GroupModel GroupModel = new GroupModel();
                GroupModel._Groups = repository.GetGroups().ToList();
                var currentUser = idb.Users.Find(User.Identity.GetUserId());
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

                TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
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
                    MapUrl = CurrentRide.MapUrl
                };

                return View(_EditAdHocRideModel);
            }
        }

        [HttpPost]
        public ActionResult EditAdHocRide(EditAdHocRideModel _EditAdHocRideModel)
        {
            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
            DateTime LocalNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TZone);
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Location _Location = repository.GetLocations().Where(l => l.id == _EditAdHocRideModel.LocationsId).FirstOrDefault();

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
                MapUrl = _EditAdHocRideModel.MapUrl
            };

            repository.UpdateAdHocRide(adhoc);
            repository.Save();

            Ad_HocRide Ah = repository.GetAdHocRideByID(_EditAdHocRideModel.adhocrideid);
            AdHocViewModel adHocViewModel = new AdHocViewModel { Ride = Ah,
                                                                 RideDate = Ah.RideDate,
                                                                 RideTime = Ah.RideTime,
                                                                 MapUrl = string.Concat("<iframe id=mapmyfitness_route src=https://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=http://veloroutes.org/k/%3Fr%3D", Ah.MapUrl, "&output=embed height=300px width=300px frameborder=0></iframe>")};

            adHocViewModel.Comments = repository.GetTop2CommentsForAdHocRide(_EditAdHocRideModel.adhocrideid);
            adHocViewModel.CommentCount = repository.GetCommentCountForAdHocRide(_EditAdHocRideModel.adhocrideid);
            adHocViewModel.Riders = repository.GetRidersForAdHocRide(_EditAdHocRideModel.adhocrideid, TZone);
            adHocViewModel.IsOwner = repository.IsAdHocCreator(_EditAdHocRideModel.adhocrideid, currentUser.Id);

            return View("ViewAdHocRide", adHocViewModel);

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

            Ad_HocRide Ah = repository.GetAdHocRideByID(RideComment.adhocrideid);
            AdHocViewModel adHocViewModel = new AdHocViewModel { Ride = Ah, RideDate = Ah.RideDate, RideTime = Ah.RideTime, };

            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
            adHocViewModel.Riders = repository.GetRidersForAdHocRide(RideComment.adhocrideid,TZone);
        
            adHocViewModel.Comments = repository.GetTop2CommentsForAdHocRide(RideComment.adhocrideid);
            adHocViewModel.CommentCount = repository.GetCommentCountForAdHocRide(RideComment.adhocrideid);

            return View("ViewAdHocRide", adHocViewModel);

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
            RideModel.Ride = repository.GetRideByID(RideComment.RideId);
            RideModel.RideDate = RideModel.Ride.RideDate;
            RideModel.Group = repository.GetGroupByID(RideComment.GroupId);
            RideModel.NextRide = RideModel.Group.Rides.Where(u => u.RideDate > RideModel.Ride.RideDate).OrderBy(i => i.RideDate).FirstOrDefault();

            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
            RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id,TZone);

            RideModel.Comments = repository.GetTop2CommentsForRide(RideModel.Ride.id);
            RideModel.CommentCount = repository.GetCommentCountForRide(RideModel.Ride.id);

            return View("Index",RideModel);

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
            RideModel.Ride = repository.GetRideByID(RideComment.RideId);
            RideModel.Group = repository.GetGroupByID(RideComment.GroupId);

            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
            RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id,TZone);

            RideModel.Comments = repository.GetTop2CommentsForRide(RideModel.Ride.id);
            RideModel.PreviousRide = repository.GetPreviousRideForGroup(RideModel.Group);

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
           
            RideModel.Ride = _Ride;
            RideModel.RideDate = RideModel.Ride.RideDate;
            RideModel.NextRide = _Group.Rides.Where(u => u.RideDate > RideModel.Ride.RideDate).OrderBy(i => i.RideDate).FirstOrDefault();
            RideModel.Group = _Group;
            RideModel.Comments = repository.GetTop2CommentsForRide(RideModel.Ride.id);

            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");

            RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id, TZone);

            return View("Index", RideModel);
        }

        public ActionResult AttendAdHocRider(int adhocrideid, string Commitment)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Ad_HocRide _Ride = new Ad_HocRide();
            Group _Group = new Group();


            _Ride = repository.GetAdHocRideByID(adhocrideid);

            AdHocRider _Rider = new AdHocRider { userId = currentUser.Id, Name = currentUser.UserName, AdHocRide = _Ride, LeaveTime = DateTime.UtcNow, PercentKeen = Commitment };

            repository.AddAdHocRider(_Rider, _Ride);
            repository.Save();

            Ad_HocRide Ah = repository.GetAdHocRideByID(adhocrideid);
            AdHocViewModel adHocViewModel = new AdHocViewModel { Ride = Ah, RideDate = Ah.RideDate, RideTime = Ah.RideTime, };

            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");
            adHocViewModel.Riders = repository.GetRidersForAdHocRide(adhocrideid, TZone);

            adHocViewModel.Comments = repository.GetTop2CommentsForAdHocRide(adhocrideid);

            return View("ViewAdHocRide", adHocViewModel);
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
            RideModel.Ride = _Ride;
            RideModel.Group = _Group;

            TimeZoneInfo TZone = TimeZoneInfo.FindSystemTimeZoneById("E. Australia Standard Time");

            RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id,TZone);

            RideModel.Comments = repository.GetTop2CommentsForRide(RideModel.Ride.id);
            RideModel.PreviousRide = repository.GetRideByID(PreviousRideID);
            

            return View("NextRide", RideModel);
        }

       
	}
}