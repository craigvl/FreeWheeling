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
using System.Threading.Tasks;


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

        public ActionResult Feedback()
        {

            return View();
        }

        [HttpPost]
        public JsonResult Feedback(FeedBackModel _FeedBackModel)
        {
            if (_FeedBackModel.SumValue == 7)
            {
                var result = new { Success = true, Message = "Feed Back Sent, Thanks!" };
                Task T = new Task(() =>
                {
                    UserHelper _UserHelp = new UserHelper();
                    _UserHelp.SendFeedBack(_FeedBackModel.Name, _FeedBackModel.Message);
                });

                T.Start();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = new { Success = false, Message = "Please enter the correct sum" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            CultureHelper _CultureHelper = new CultureHelper(repository);           
            HomeIndexModel _HomeIndexModel = new HomeIndexModel();
            _HomeIndexModel.Locations = repository.GetLocations().ToList();
            if (currentUser != null)
            {
                Member _CurrentMember = repository.GetMemberByUserID(currentUser.Id);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
                       
            if (currentUser.LocationID != null)
            {
                    //Check that user ID is a current location ID
                    if (_HomeIndexModel.Locations.Any(l => l.id == currentUser.LocationID))
                    {
                        TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
                        Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();
                        _HomeIndexModel.FavouriteBunches = repository.GetFavouriteGroupsByLocation(_Location.id, currentUser.Id).ToList();
                        Session["Culture"] = _CultureHelper.GetCulture(Convert.ToInt32(currentUser.LocationID));
                        _HomeIndexModel.LocationsId = _Location.id;
                        _HomeIndexModel.CurrentUserLocation = _Location.Name;
                        _HomeIndexModel.UpCommingAd_HocCount = repository.GetUpCommingAd_HocCount(repository.GetLocations()
                            .Where(o => o.id == currentUser.LocationID).FirstOrDefault(), TZone);
                        _HomeIndexModel.UpCommingAd_HocCount = _HomeIndexModel.UpCommingAd_HocCount + repository.GetPrivateAdHocRideByUserID(currentUser.Id
                            , _Location, TZone).Count();
                        _HomeIndexModel.HomePageRide = repository.GetHomePageRideByUserID(currentUser.Id);
                    }
            }
            else
            {
                _HomeIndexModel.CurrentUserLocation = "Please set a Location";
            }
            return View(_HomeIndexModel);
        }

        [HttpPost]
        public ActionResult Index(HomeIndexModel _HomeIndexModel, string returnUrl)
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

            if (returnUrl != null)
            {
               return  RedirectToLocal(returnUrl);
            }
            else
            {
                return View(_HomeIndexModel);
            }
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