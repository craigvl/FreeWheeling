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

            foreach (Group item in _GroupModel._Groups)
            {

                item.Rides = item.Rides.Where(t => t.RideDate >= DateTime.Now).ToList();
                Ride NextRide = repository.GetNextRideForGroup(item);
                
                if (NextRide != null)
                {
                    _GroupModel._NextRideDetails.Add(new NextRideDetails { Date = NextRide.RideDate, GroupId = item.id, NumberofRiders = NextRide.Riders.Where(i => i.PercentKeen == "100") .Count() });    
                }
                
            }

            _GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);

            return View(_GroupModel);
        }

        public ActionResult RemoveFromFavouriteList(int id, string title)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Member _Member = repository.GetMemberByUserID(currentUser.Id);
            Group group = repository.GetGroupByID(id);

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
                Ride NextRide = repository.GetNextRideForGroup(item);

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
                    Ride NextRide = repository.GetNextRideForGroup(item);

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
                Ride NextRide = repository.GetNextRideForGroup(item);

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

            GroupModel _GroupModel = new GroupModel();

            _GroupModel._Groups = repository.GetGroupsByLocation(currentUser.LocationID).Where(u => u.Members.Any(m => m.userId == currentUser.Id)).ToList();
            _GroupModel.title = "Favourite Groups";

            _GroupModel._NextRideDetails = new List<NextRideDetails>();
            _GroupModel.UserLocation = repository.GetLocationName(currentUser.LocationID);

            foreach (Group item in _GroupModel._Groups)
            {

                item.Rides = item.Rides.Where(t => t.RideDate >= DateTime.Now).ToList();
                Ride NextRide = repository.GetNextRideForGroup(item);

                if (NextRide != null)
                {
                    _GroupModel._NextRideDetails.Add(new NextRideDetails { Date = NextRide.RideDate, GroupId = item.id, NumberofRiders = NextRide.Riders.Where(i => i.PercentKeen == "100").Count() });
                }

            }


            _GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);

            return View("Index",_GroupModel);

        }

        //// GET: /Group/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Group group = db.Groups.Find(id);
        //    if (group == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(group);
        //}

        //// GET: /Group/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: /Group/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include="id,name,IsPrivate")] Group group)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Groups.Add(group);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(group);
        //}

        //// GET: /Group/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Group group = db.Groups.Find(id);
        //    if (group == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(group);
        //}

        //// POST: /Group/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="id,name,IsPrivate")] Group group)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(group).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(group);
        //}

        //// GET: /Group/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Group group = db.Groups.Find(id);
        //    if (group == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(group);
        //}

        //// POST: /Group/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Group group = db.Groups.Find(id);
        //    db.Groups.Remove(group);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
