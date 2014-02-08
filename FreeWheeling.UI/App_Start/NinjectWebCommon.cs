[assembly: WebActivator.PreApplicationStartMethod(typeof(FreeWheeling.UI.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(FreeWheeling.UI.App_Start.NinjectWebCommon), "Stop")]

namespace FreeWheeling.UI.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using FreeWheeling.Domain.Abstract;
    using FreeWheeling.Domain.Concrete;
    using Ninject.Web.Mvc.FilterBindingSyntax;
    using FreeWheeling.UI.Filters;
    using System.Web.Mvc;
    using System.Web.Http;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);

            // Install our Ninject-based IDependencyResolver into the Web API config
            GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);

            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {

            kernel.Bind<ICycleRepository>().To<CycleRepository>();

        //Can you the below to control user access to action methods.
        //         kernel.BindFilter<RoleAttribute>(FilterScope.Action, 0).When(
        // (controllerContext, actionDescriptor) => actionDescriptor.ActionName == "EditAdHocRide");

        //         kernel.BindFilter<RoleAttribute>(FilterScope.Action, 0).When(
        //(controllerContext, actionDescriptor) => actionDescriptor.ActionName == "EditGroup");

        }        
    }
}
