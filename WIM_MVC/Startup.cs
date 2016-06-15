using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WIM_MVC.Startup))]
namespace WIM_MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
