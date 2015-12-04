using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TechTree.Startup))]
namespace TechTree
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
