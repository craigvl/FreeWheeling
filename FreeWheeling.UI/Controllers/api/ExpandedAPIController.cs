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

namespace FreeWheeling.UI.Controllers
{
    [Authorize]
    public class ExpandedAPIController : ApiController
    {
        private ICycleRepository repository;

        public ExpandedAPIController(ICycleRepository repoParam)
        {
            repository = repoParam;
        }

        [HttpGet]
        public IEnumerable<String> Get()
        {
            return repository.GetGroups().Select(g => g.name);
        }

    }

   
}