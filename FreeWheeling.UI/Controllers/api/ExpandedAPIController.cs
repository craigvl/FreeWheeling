using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using FreeWheeling.Domain.Entities;
using FreeWheeling.Domain.Abstract;
using FreeWheeling.UI.DataContexts;
using System.Security.Principal;
using FreeWheeling.UI.Models;
using Microsoft.AspNet.Identity;

namespace FreeWheeling.UI.Controllers
{
    //[Authorize]
    public class ExpandedAPIController : ApiController
    {
        private ICycleRepository repository;
        private IdentityDb idb = new IdentityDb();
       
        public ExpandedAPIController(ICycleRepository repoParam)
        {
            repository = repoParam;
        }

        [HttpGet]
        public IEnumerable<String> Get()
        {
            return repository.GetGroups().Select(g => g.name);
        }

        //Post http://localhost:6049/api/Expanded/1
        [HttpPost]
        public IHttpActionResult PostExtend(UserExpandModel _UserExpandModel)
        {

            ApplicationUser currentUser = idb.Users.Where(u => u.UserName == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();

            _UserExpandModel.userid = currentUser.Id;

            UserExpandHelper _UserExpandHelper = new UserExpandHelper(repository);

            _UserExpandHelper.UpdateOrInsertUserExpand(_UserExpandModel);

            //db.UserExpands.Add(userexpand);
            //db.SaveChanges();

           
            //return NotFound();
            return Ok();
        }


        //[HttpGet]
        //public String GetUser()
        //{
        //    return System.Threading.Thread.CurrentPrincipal.Identity.Name;
        //    return System.Web.HttpContext.Current.User.Identity.Name;
        //}

    }  
}