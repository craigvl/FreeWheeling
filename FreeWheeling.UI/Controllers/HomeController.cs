using FreeWheeling.Domain.Abstract;
using FreeWheeling.UI.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FreeWheeling.UI.Models;


namespace FreeWheeling.UI.Controllers
{
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
            _HomeIndexModel.LocationsId = "4";

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