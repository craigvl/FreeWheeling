using FreeWheeling.Domain.Abstract;
using FreeWheeling.UI.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FreeWheeling.UI.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private IdentityDb idb = new IdentityDb();

        private ICycleRepository repository;

        public SettingsController(ICycleRepository repoParam)
        {
            repository = repoParam;
        }

        public ActionResult Index()
        {

            return View();
        }
	}
}