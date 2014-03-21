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

            currentUser.LocationID = repository.GetLocations().Where(l => l.id == _SettingsModel.LocationsId).Select(o => o.id).FirstOrDefault();
            currentUser.ReceiveEmails = _SettingsModel.ReceiveEmails;
            idb.SaveChanges();

            return Json(new
            {
                success = true,
                message = "Settings Saved"
            }, JsonRequestBehavior.AllowGet);

        }
	}
}