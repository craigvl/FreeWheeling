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
            var errMsg = TempData["ErrorMessage"] as string;
            GroupModel _GroupModel = new GroupModel();
            _GroupModel._Groups = repository.GetGroups().ToList();

            foreach (Group item in _GroupModel._Groups)
            {

                item.Rides = item.Rides.Where(t => t.RideDate >= DateTime.Now).ToList();

            }

            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            _GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);

            return View(_GroupModel);
        }

        //// GET: /Group/Details/5
        public ActionResult Join(int id)
        {
           
            Group group = repository.GetGroupByID(id);
            GroupModel GroupModel = new GroupModel();
            GroupModel._Groups = repository.GetGroups().ToList();

            if (group == null)
            {
                return HttpNotFound();
            }

            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            repository.AddMember(currentUser.Id, group);
            repository.Save();

            GroupModel.CurrentGroupMembership = repository.CurrentGroupsForUser(currentUser.Id);

            return View("Index", GroupModel);
        }

        public ViewResult MyGroups()
        {

            MyGroupsModel GroupModel = new MyGroupsModel();

            var currentUser = idb.Users.Find(User.Identity.GetUserId());

            GroupModel.CycleGroups = repository.GetGroups().Where(u => u.Members.Any(m => m.userId == currentUser.Id)).ToList();

            return View(GroupModel);


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
