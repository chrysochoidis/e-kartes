using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ekartes.Startup))]
namespace ekartes
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
