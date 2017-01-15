using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SampleWebApp.Providers.Startup))]
namespace SampleWebApp.Providers
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
