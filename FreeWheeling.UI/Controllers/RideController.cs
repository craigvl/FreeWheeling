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

namespace FreeWheeling.UI.Controllers
{
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

            RideModelIndex RideModel = new RideModelIndex();

            Group _Group = repository.GetGroupByID(groupid);

            if (rideid == -1)
            {
                RideModel.Ride = _Group.Rides.Where(u => u.RideDate >= DateTime.Now).OrderBy(i => i.RideDate).FirstOrDefault();
                RideModel.NextRide = _Group.Rides.Where(u => u.RideDate > RideModel.Ride.RideDate).OrderBy(i => i.RideDate).FirstOrDefault(); 
            }
            else
            {
                int _rideid = rideid;
                RideModel.Ride = repository.GetRideByID(rideid);
                RideModel.NextRide = _Group.Rides.Where(u => u.RideDate > RideModel.Ride.RideDate).OrderBy(i => i.RideDate).FirstOrDefault(); 
            }

            if (RideModel.Ride != null)
            {
                RideModel.Group = _Group;
                RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id);
                RideModel.Comments = repository.GetCommentsForRide(RideModel.Ride.id);
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

        public ActionResult NextRide(int RideId, int Groupid, int PreviousRideID)
        {

            RideModelIndex RideModel = new RideModelIndex();

            Group _Group = repository.GetGroupByID(Groupid);

            RideModel.PreviousRide = repository.GetRideByID(PreviousRideID);

            RideModel.Ride = repository.GetRideByID(RideId);

            if (RideModel.Ride != null)
            {

                RideModel.Group = _Group;
                RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id);

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

        public ActionResult AddComment(int groupid, int RideId)
        {
            RideCommentModel _RideCommentModel = new RideCommentModel();
            _RideCommentModel.RideId = RideId;
            _RideCommentModel.GroupId = groupid;
            _RideCommentModel.NextRide = 

            _RideCommentModel.Ride = repository.GetRideByID(RideId);

            return View(_RideCommentModel);

        }

        [HttpPost]
        public ActionResult AddComment(RideCommentModel RideComment)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            repository.AddRideComment(RideComment.Comment, RideComment.RideId, currentUser.Id);
            repository.Save();

            RideModelIndex RideModel = new RideModelIndex();
            RideModel.Ride = repository.GetRideByID(RideComment.RideId);
            RideModel.Group = repository.GetGroupByID(RideComment.GroupId);
            RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id);

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

        public ActionResult Attend(int RideId, string Commitment, int Groupid)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Ride _Ride = new Ride();
            Group _Group = new Group();


            _Ride = repository.GetRideByID(RideId);
            _Group = repository.GetGroupByID(Groupid);

            Rider _Rider = new Rider { userId = currentUser.Id, Name = currentUser.UserName, Ride = _Ride, LeaveTime = DateTime.Now.ToShortTimeString(), PercentKeen = Commitment };

            repository.AddRider(_Rider, _Group);
            repository.Save();

            RideModelIndex RideModel = new RideModelIndex();
            RideModel.Ride = _Ride;
            RideModel.NextRide = _Group.Rides.Where(u => u.RideDate > RideModel.Ride.RideDate).OrderBy(i => i.RideDate).FirstOrDefault();
            RideModel.Group = _Group;
            RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id);

            return View("Index", RideModel);
        }

        public ActionResult AttendNext(int RideId, string Commitment, int Groupid, int PreviousRideID)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Ride _Ride = new Ride();
            Group _Group = new Group();

            _Ride = repository.GetRideByID(RideId);
            _Group = repository.GetGroupByID(Groupid);

            Rider _Rider = new Rider { userId = currentUser.Id, Name = currentUser.UserName, Ride = _Ride, LeaveTime = DateTime.Now.ToShortTimeString(), PercentKeen = Commitment };

            repository.AddRider(_Rider, _Group);
            repository.Save();

            RideModelIndex RideModel = new RideModelIndex();
            RideModel.Ride = _Ride;
            RideModel.Group = _Group;
            RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id);
            RideModel.PreviousRide = repository.GetRideByID(PreviousRideID);

            return View("NextRide", RideModel);
        }

       
	}
}