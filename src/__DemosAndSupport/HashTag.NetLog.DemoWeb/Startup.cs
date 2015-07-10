using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HashTag.NetLog.DemoWeb.Startup))]
namespace HashTag.NetLog.DemoWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
