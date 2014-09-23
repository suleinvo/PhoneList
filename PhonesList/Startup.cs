using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PhonesList.Startup))]
namespace PhonesList
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
