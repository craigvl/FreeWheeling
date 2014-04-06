using FreeWheeling.Domain.Abstract;
using FreeWheeling.UI.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FreeWheeling.UI.Models;
using Microsoft.AspNet.Identity;
using FreeWheeling.Domain.Entities;
using FreeWheeling.UI.Infrastructure;


namespace FreeWheeling.UI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private IdentityDb idb = new IdentityDb(); 

        private ICycleRepository repository;

        public HomeController(ICycleRepository repoParam)
        {
            repository = repoParam;
        }

        public ActionResult Index()
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            CultureHelper _CultureHelper = new CultureHelper(repository);           
            HomeIndexModel _HomeIndexModel = new HomeIndexModel();
            _HomeIndexModel.Locations = repository.GetLocations().ToList();
            Member _CurrentMember = repository.GetMemberByUserID(currentUser.Id);
            
            if (currentUser.LocationID != null)
            {
                //Check that user ID is a current location ID
                if (_HomeIndexModel.Locations.Any(l => l.id == currentUser.LocationID))
                {
                    TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
                    Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();
                    _HomeIndexModel.FavouriteBunches = repository.GetFavouriteGroupsByLocation(_Location.id).Where(u => u.Members.Any(m => m.userId == currentUser.Id)).ToList();
                    Session["Culture"] = _CultureHelper.GetCulture(Convert.ToInt32(currentUser.LocationID));
                    _HomeIndexModel.LocationsId = _Location.id;
                    _HomeIndexModel.CurrentUserLocation = _Location.Name;
                    _HomeIndexModel.UpCommingAd_HocCount = repository.GetUpCommingAd_HocCount(repository.GetLocations().Where(o => o.id == currentUser.LocationID).FirstOrDefault(), TZone);
                }
            }
            else
            {
                _HomeIndexModel.CurrentUserLocation = "Please set a Location";
            }

            return View(_HomeIndexModel);
        }

        [HttpPost]
        public ActionResult Index(HomeIndexModel _HomeIndexModel)
        {
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            if (_HomeIndexModel.LocationsId != null)
            {
                _HomeIndexModel.Locations = repository.GetLocations().ToList();
                currentUser.LocationID = repository.GetLocations().Where(l => l.id == _HomeIndexModel.LocationsId ).Select(o => o.id).FirstOrDefault();
                idb.SaveChanges();
            }
            else
            {
                _HomeIndexModel.LocationsId = 0;
            }

            if (currentUser.LocationID != null)
            {
                _HomeIndexModel.CurrentUserLocation = repository.GetLocations().Where(i => i.id == currentUser.LocationID).Select(o => o.Name).FirstOrDefault();
            }
            else
            {
                _HomeIndexModel.CurrentUserLocation = "Please set a Location";
            }
            return View(_HomeIndexModel);
        }

        public ActionResult LocationChange()
        {
            LocationChangeModel _LocationChangeModel = new LocationChangeModel();
            _LocationChangeModel.Locations = repository.GetLocations().ToList();
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();
            _LocationChangeModel.LocationsId = _Location.id;
            _LocationChangeModel.CurrentUserLocation = _Location.Name;
            return View(_LocationChangeModel);
        }

        [HttpPost]
        public ActionResult LocationChange(LocationChangeModel _LocationChangeModel)
        {
            HomeIndexModel _HomeIndexModel = new HomeIndexModel();          
            _HomeIndexModel.Locations = repository.GetLocations().ToList();
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            CultureHelper _CultureHelper = new CultureHelper(repository);

            //Check that user ID is a current location ID
            if (_HomeIndexModel.Locations.Any(l => l.id == _LocationChangeModel.LocationsId))
            {
                currentUser.LocationID = repository.GetLocations().Where(l => l.id == _LocationChangeModel.LocationsId).Select(o => o.id).FirstOrDefault();
                idb.SaveChanges();
                Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();
                TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
                _HomeIndexModel.LocationsId = _Location.id;
                _HomeIndexModel.CurrentUserLocation = _Location.Name;
                _HomeIndexModel.UpCommingAd_HocCount = repository.GetUpCommingAd_HocCount(repository.GetLocations().Where(o => o.id == currentUser.LocationID).FirstOrDefault(), TZone);
            }
            else
            {
                TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
                Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();
                _HomeIndexModel.LocationsId = _Location.id;
                _HomeIndexModel.CurrentUserLocation = _Location.Name;
                _HomeIndexModel.UpCommingAd_HocCount = repository.GetUpCommingAd_HocCount(repository.GetLocations().Where(o => o.id == currentUser.LocationID).FirstOrDefault(), TZone);
            }

            return View("Index",_HomeIndexModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}