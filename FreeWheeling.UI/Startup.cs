using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FreeWheeling.UI.Startup))]
namespace FreeWheeling.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
