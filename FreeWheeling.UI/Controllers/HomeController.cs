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
using FreeWheeling.UI.Filters;
using NodaTime.TimeZones;

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

        [AllowAnonymous]
        [Compress]
        public ActionResult Feedback()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult Feedback(FeedBackModel _FeedBackModel)
        {
            if (_FeedBackModel.SumValue == 7)
            {
                var result = new { Success = true, Message = "Feedback Sent, Thanks!" };
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

        //[Compress]
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
                    //Don't forget to update http post method to match model updates.
                    if (_HomeIndexModel.Locations.Any(l => l.id == currentUser.LocationID))
                    {
                        TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
                        Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();
                        _HomeIndexModel.FavouriteBunches = repository.GetFavouriteGroupsByLocation(_Location.id, currentUser.Id).ToList();
                        Session["Culture"] = _CultureHelper.GetCulture(Convert.ToInt32(currentUser.LocationID));
                        _HomeIndexModel.LocationsId = _Location.id;
                        _HomeIndexModel.CurrentUserLocation = _Location.Name;
                        _HomeIndexModel.BunchCount = repository.GetGroupCount(currentUser.LocationID);
                        _HomeIndexModel.Followingcount = repository.GetFollowingCount(currentUser.Id);
                        _HomeIndexModel.HomePageRide = repository.GetHomePageRideByUserID(currentUser.Id);
                        if (_HomeIndexModel.HomePageRide != null)
	                    {
                            _HomeIndexModel.IsOnWay = repository.IsOnWay(_HomeIndexModel.HomePageRide.id,currentUser.Id);
                            _HomeIndexModel.IsIn = repository.IsIn(_HomeIndexModel.HomePageRide.id, currentUser.Id);
                            _HomeIndexModel.IsOut = repository.IsOut(_HomeIndexModel.HomePageRide.id, currentUser.Id);
                            _HomeIndexModel.Keencount = repository.GetKeenCountForRide(_HomeIndexModel.HomePageRide.id);
	                    }                        
                    }
            }
            else
            {
                Task T = new Task(() =>
                {
                    repository.PopulateInitialExpandValues(currentUser.Id);
                });

                T.Start();
                _HomeIndexModel.CurrentUserLocation = "Please set a Location";
            }
            return View(_HomeIndexModel);
        }

        [HttpPost]
        [Compress]
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
                CultureHelper _CultureHelper = new CultureHelper(repository);
                TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
                Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();
                _HomeIndexModel.FavouriteBunches = repository.GetFavouriteGroupsByLocation(_Location.id, currentUser.Id).ToList();
                Session["Culture"] = _CultureHelper.GetCulture(Convert.ToInt32(currentUser.LocationID));
                _HomeIndexModel.LocationsId = _Location.id;
                _HomeIndexModel.CurrentUserLocation = _Location.Name;
                _HomeIndexModel.BunchCount = repository.GetGroupCount(currentUser.LocationID);
                _HomeIndexModel.Followingcount = repository.GetFollowingCount(currentUser.Id);
                _HomeIndexModel.HomePageRide = repository.GetHomePageRideByUserID(currentUser.Id);
                if (_HomeIndexModel.HomePageRide != null)
                {
                    _HomeIndexModel.IsOnWay = repository.IsOnWay(_HomeIndexModel.HomePageRide.id, currentUser.Id);
                    _HomeIndexModel.IsIn = repository.IsIn(_HomeIndexModel.HomePageRide.id, currentUser.Id);
                    _HomeIndexModel.IsOut = repository.IsOut(_HomeIndexModel.HomePageRide.id, currentUser.Id);
                    _HomeIndexModel.Keencount = repository.GetKeenCountForRide(_HomeIndexModel.HomePageRide.id);
                }               
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

        public ActionResult LocationCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LocationCreate(LocationCreate _LocationCreate)
        {
            //Check that Location does not already exist
            Location _LocationCheck = repository.GetLocations().Where(o => o.Name.ToLower().Equals(_LocationCreate.Name.ToLower())).FirstOrDefault();
            CultureHelper _CultureHelper = new CultureHelper(repository);
            HomeIndexModel _HomeIndexModel = new HomeIndexModel();

            if (_LocationCheck != null)
            {
                ModelState.AddModelError(string.Empty, "It looks like this location has already been created.");
                return View(_LocationCreate);
            }
            else
            {
                if (_LocationCreate.GoogletzTimeZone == null)
                {
                   ModelState.AddModelError(string.Empty, "Unable to lookup time zone for location please try again.");
                   return View(_LocationCreate); 
                }

                _LocationCreate.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(_CultureHelper.IanaToWindows(_LocationCreate.GoogletzTimeZone));

                if (_LocationCreate.TimeZone == null)
                {
                    ModelState.AddModelError(string.Empty, "Unable to lookup time zone for location please try again.");
                    return View(_LocationCreate);
                }

                Location NewLocation = new Location { Name = _LocationCreate.Name,
                                                      TimeZoneInfo = _LocationCreate.TimeZoneId,
                                                      Lat = _LocationCreate.lat,
                                                      Lng = _LocationCreate.lng,
                                                      CurrentGoogleUTC = _LocationCreate.CurrentGoogleUTC,
                                                      dstOffset = _LocationCreate.dstOffset,
                                                      Google_ErrorMessage = _LocationCreate.Google_ErrorMessage,
                                                      GoogleStatus = _LocationCreate.GoogleStatus,
                                                      GoogletimeZoneName = _LocationCreate.GoogletimeZoneName,
                                                      GoogletzTimeZone = _LocationCreate.GoogletzTimeZone,
                                                      rawOffset = _LocationCreate.rawOffset};
                repository.AddLocation(NewLocation);
                var currentUser = idb.Users.Find(User.Identity.GetUserId());
                currentUser.LocationID = NewLocation.id;
                idb.SaveChanges();
                _HomeIndexModel.Locations = repository.GetLocations().ToList();

                if (_HomeIndexModel.Locations.Any(l => l.id == currentUser.LocationID))
                {
                    TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
                    Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();
                    _HomeIndexModel.FavouriteBunches = repository.GetFavouriteGroupsByLocation(_Location.id, currentUser.Id).ToList();
                    Session["Culture"] = _CultureHelper.GetCulture(Convert.ToInt32(currentUser.LocationID));
                    _HomeIndexModel.LocationsId = _Location.id;
                    _HomeIndexModel.CurrentUserLocation = _Location.Name;
                    _HomeIndexModel.BunchCount = repository.GetGroupCount(currentUser.LocationID);
                    _HomeIndexModel.Followingcount = repository.GetFollowingCount(currentUser.Id);
                    _HomeIndexModel.HomePageRide = repository.GetHomePageRideByUserID(currentUser.Id);
                    if (_HomeIndexModel.HomePageRide != null)
                    {
                        _HomeIndexModel.IsOnWay = repository.IsOnWay(_HomeIndexModel.HomePageRide.id, currentUser.Id);
                        _HomeIndexModel.IsIn = repository.IsIn(_HomeIndexModel.HomePageRide.id, currentUser.Id);
                        _HomeIndexModel.IsOut = repository.IsOut(_HomeIndexModel.HomePageRide.id, currentUser.Id);
                        _HomeIndexModel.Keencount = repository.GetKeenCountForRide(_HomeIndexModel.HomePageRide.id);
                    }
                }
                else
                {
                    Task T = new Task(() =>
                    {
                        repository.PopulateInitialExpandValues(currentUser.Id);
                    });

                    T.Start();
                    _HomeIndexModel.CurrentUserLocation = "Please set a Location";
                }

                return View("Index",_HomeIndexModel);
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
            }
            else
            {
                TimeZoneInfo TZone = _CultureHelper.GetTimeZoneInfo(currentUser.LocationID);
                Location _Location = repository.GetLocations().Where(l => l.id == currentUser.LocationID).FirstOrDefault();
                _HomeIndexModel.LocationsId = _Location.id;
                _HomeIndexModel.CurrentUserLocation = _Location.Name;
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