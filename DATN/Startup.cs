using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WH.Startup))]
namespace WH
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
