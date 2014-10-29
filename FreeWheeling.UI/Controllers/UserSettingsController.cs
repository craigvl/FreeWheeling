using FreeWheeling.Domain.Abstract;
using FreeWheeling.UI.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FreeWheeling.UI.Models;
using FreeWheeling.Domain.Entities;
using System.Threading.Tasks;
using FreeWheeling.UI.Infrastructure;

namespace FreeWheeling.UI.Controllers
{
    [Authorize]
    public class UserSettingsController : Controller
    {
        private IdentityDb idb = new IdentityDb();
        private ICycleRepository repository;

        public UserSettingsController(ICycleRepository repoParam)
        {
            repository = repoParam;
        }

        public ActionResult Index()
        {
            UserSettingsModel _SettingsModel = new UserSettingsModel();
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            _SettingsModel.Locations = repository.GetLocations().ToList();
            Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();
            _SettingsModel.LocationsId = _Location.id;
            _SettingsModel.CurrentUserLocation = _Location.Name;
            _SettingsModel.ReceiveEmails = currentUser.ReceiveEmails;
            _SettingsModel.FirstName = currentUser.FirstName;
            _SettingsModel.LastName = currentUser.LastName;

            if (currentUser.LocationID != null)
            {
            }
            else
            {
                return RedirectToAction("Index","Home");
            }

            return View(_SettingsModel);
        }

        [HttpPost]
        public JsonResult Index(UserSettingsModel _SettingsModel)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            _SettingsModel.Locations = repository.GetLocations().ToList();

            //Check that user ID is a current location ID
            if (_SettingsModel.Locations.Any(l => l.id == _SettingsModel.LocationsId))
            {
                currentUser.LocationID = repository.GetLocations().Where(l => l.id == _SettingsModel.LocationsId).Select(o => o.id).FirstOrDefault();
                currentUser.ReceiveEmails = _SettingsModel.ReceiveEmails;
                currentUser.FirstName = _SettingsModel.FirstName;
                currentUser.LastName = _SettingsModel.LastName;
                idb.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = "Settings Saved"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Please Select a Location"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Follow()
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            List<FollowingModel> _FollowingList = new List<FollowingModel>();
            List<ApplicationUser> _Users = idb.Users.Where(l => l.LocationID == currentUser.LocationID && l.Id != currentUser.Id).ToList();

            foreach (ApplicationUser item in _Users)
            {
                FollowingModel _Fmodel = new FollowingModel { FirstName = item.FirstName,
                                                              LastName = item.LastName,
                                                              UserID = item.Id,
                                                              UserName = item.UserName,
                                                              following = repository.IsFollowing(currentUser.Id, item.Id) };
                _FollowingList.Add(_Fmodel);
            }

            return View(_FollowingList);
        }

        public JsonResult FollowAddJSON(string Id)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            repository.AddFollowingUser(currentUser.Id, Id);
            List<FollowingModel> _FollowingList = new List<FollowingModel>();
            List<ApplicationUser> _Users = idb.Users.Where(l => l.LocationID == currentUser.LocationID && l.Id != currentUser.Id).ToList();

            foreach (ApplicationUser item in _Users)
            {
                FollowingModel _Fmodel = new FollowingModel
                {
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    UserID = item.Id,
                    UserName = item.UserName,
                    following = repository.IsFollowing(currentUser.Id, item.Id)
                };
                _FollowingList.Add(_Fmodel);
            }

            Task E = new Task(() =>
            {
                UserHelper _UserHelp = new UserHelper();
                string email = _UserHelp.GetUserEmailViaUserId(Id);
                ApplicationUser _u = idb.Users.Find(currentUser.Id);
                string Username = _u.UserName + " (" + _u.FirstName + " " + _u.LastName + ")";
                _UserHelp.SendFollowing(Username, email);
            });

            E.Start();

            return Json(new
            {
                success = true              
            }, JsonRequestBehavior.AllowGet);  
        }

        public JsonResult UnfollowJSON(string Id)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            repository.DeleteFollowingUser(currentUser.Id, Id);
            List<FollowingModel> _FollowingList = new List<FollowingModel>();
            List<ApplicationUser> _Users = idb.Users.Where(l => l.LocationID == currentUser.LocationID && l.Id != currentUser.Id).ToList();

            foreach (ApplicationUser item in _Users)
            {
                FollowingModel _Fmodel = new FollowingModel
                {
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    UserID = item.Id,
                    UserName = item.UserName,
                    following = repository.IsFollowing(currentUser.Id, item.Id)
                };
                _FollowingList.Add(_Fmodel);
            }

            return Json(new
            {
                success = true
            }, JsonRequestBehavior.AllowGet);
        }
	}
}