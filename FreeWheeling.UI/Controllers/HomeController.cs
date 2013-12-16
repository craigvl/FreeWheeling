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
            HomeIndexModel _HomeIndexModel = new HomeIndexModel();
            _HomeIndexModel.Locations = repository.GetLocations().ToList();

            var currentUser = idb.Users.Find(User.Identity.GetUserId());
            Location _Location = repository.GetMemberLocation(currentUser.Id);
            
            if (_Location != null)
            {


            _HomeIndexModel.LocationsId = _Location.id ;
            _HomeIndexModel.CurrentUserLocation = _Location.Name;

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

                repository.SetMemberLocation(currentUser.Id, _HomeIndexModel.LocationsId);
                repository.Save();

            }
            else
            {

                _HomeIndexModel.LocationsId = 0;

            }

            Location _Location = repository.GetMemberLocation(currentUser.Id);

            if (_Location != null)
            {
                _HomeIndexModel.CurrentUserLocation = _Location.Name;
            }
            else
            {

                _HomeIndexModel.CurrentUserLocation = "Please set a Location";

            }
            return View(_HomeIndexModel);
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