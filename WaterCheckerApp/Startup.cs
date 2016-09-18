using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WaterCheckerApp.Startup))]
namespace WaterCheckerApp
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
