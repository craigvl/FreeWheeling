using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using FreeWheeling.UI.Controllers;
using FreeWheeling.Domain.Abstract;
using FreeWheeling.Domain.Concrete;

namespace FreeWheeling.UI.App_Start
{
    public class ResolveController : IDependencyResolver
    {
        private static readonly ICycleRepository CycleStore = new CycleRepository();

        public object GetService(Type serviceType)
        {
            return serviceType == typeof(ExpandedAPIController) ? new ExpandedAPIController(CycleStore) : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>();
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {

        }
    }
}