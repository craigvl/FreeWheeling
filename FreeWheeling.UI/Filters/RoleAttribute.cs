using FreeWheeling.Domain.Abstract;
using FreeWheeling.UI.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FreeWheeling.Domain.Entities;
using Ninject;


namespace FreeWheeling.UI.Filters
{
    public class RoleAttribute : AuthorizeAttribute
    {
        private ICycleRepository repository;

        public RoleAttribute(ICycleRepository Repo)
        {
            repository = Repo;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            if (!httpContext.Request.IsAuthenticated)
                return false;

            var groupid = (httpContext.Request.RequestContext.RouteData.Values["groupId"] as string)
             ??
             (httpContext.Request["groupId"] as string);

            if (groupid != null)
            {

                if (repository.IsGroupCreator(Convert.ToInt32(groupid), httpContext.User.Identity.GetUserId().ToString()))
                {

                    return true;

                }
                else
                {

                    return false;

                }


            }

            return false;

        }

    }
}