using FreeWheeling.UI.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace FreeWheeling.UI
{
    class WebApiConfig
    {

       public static void Register(HttpConfiguration configuration)
        {

            configuration.Routes.MapHttpRoute(
               name: "API",
               routeTemplate: "api/Expanded/{id}",
               defaults: new
               {
                   controller = "ExpandedAPI",
                   id = RouteParameter.Optional
               });


            configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            //configuration.DependencyResolver = new ResolveController();
        }

    }
}