using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HashTag.Logging.Web.Service
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(Startup.Register);
            Startup.RegisterGlobalFilters(GlobalFilters.Filters);
            Startup.RegisterRoutes(RouteTable.Routes);
            Startup.RegisterBundles(BundleTable.Bundles);
        }
    }
}
