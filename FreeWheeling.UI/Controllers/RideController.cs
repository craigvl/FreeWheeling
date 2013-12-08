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

        public ActionResult Index(int groupid)
        {

            RideModelIndex RideModel = new RideModelIndex();

            Group _Group = repository.GetGroupByID(groupid);

            RideModel.Ride = _Group.Rides.Where(u => u.RideDate >= DateTime.Now).OrderBy(i => i.RideDate).FirstOrDefault();

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

        public ActionResult Attend(int RideId, string Commitment, int Groupid)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Ride _Ride = new Ride();
            Group _Group = new Group();


            _Ride = repository.GetRideByID(RideId);
            _Group = repository.GetGroupByID(Groupid);

            repository.AddRider(currentUser.Id, currentUser.UserName, _Ride, _Group, Commitment);
            repository.Save();

            RideModelIndex RideModel = new RideModelIndex();
            RideModel.Ride = _Ride;
            RideModel.Group = _Group;
            RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id);

            return View("Index", RideModel);
        }

        public ActionResult AttendNext(int RideId, string Commitment, int Groupid)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Ride _Ride = new Ride();
            Group _Group = new Group();


            _Ride = repository.GetRideByID(RideId);
            _Group = repository.GetGroupByID(Groupid);

            repository.AddRider(currentUser.Id, currentUser.UserName, _Ride, _Group, Commitment);
            repository.Save();

            RideModelIndex RideModel = new RideModelIndex();
            RideModel.Ride = _Ride;
            RideModel.Group = _Group;
            RideModel.Riders = repository.GetRidersForRide(RideModel.Ride.id);

            return View("NextRide", RideModel);
        }

        public ActionResult NextRide(int RideId, int Groupid)
        {

            RideModelIndex RideModel = new RideModelIndex();

            Group _Group = repository.GetGroupByID(Groupid);

            DateTime PreviousRide = _Group.Rides.Where(r => r.id == RideId).Select(u => u.RideDate).FirstOrDefault();

            RideModel.Ride = _Group.Rides.Where(u => u.RideDate > PreviousRide).OrderBy(i => i.RideDate).FirstOrDefault();

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
	}
}