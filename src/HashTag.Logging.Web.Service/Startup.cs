using HashTag.Diagnostics;
using HashTag.Logging.Connector.MSSQL;
using HashTag.Logging.Web.Library;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.Optimization;
using System.Web.Routing;

namespace HashTag.Logging.Web.Service
{
    public class Startup
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/LogServerScripts").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/knockout-3.3.0.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/moment.js",
                        "~/Scripts/app/eventVm.js",
                        "~/Scripts/app/detailsVm.js")
                        );
        }
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                //TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            };
            settings.Converters.Add(new NameValueCollectionConverter());
            JsonConvert.DefaultSettings = () => settings;
        }
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html")); //force JSON if format HTML request header is not specified

            var jsonResolver = new DefaultContractResolver(); //allow [Serializable] attribute on classes (for ELMAH compatiability)
            jsonResolver.IgnoreSerializableAttribute = true;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = jsonResolver;
            var jsonFormatter = config.Formatters.JsonFormatter;

            var settings = jsonFormatter.SerializerSettings;
            settings.Converters.Add(new NameValueCollectionConverter());

            config.Formatters.XmlFormatter.UseXmlSerializer = true;

            ODataModelBuilder builder = new ODataConventionModelBuilder();

            builder.EntitySet<Event>("4").EntityType.HasKey(p => p.UUID);
            builder.EntitySet<EventProperty>("Properties").EntityType.HasKey(p => p.UUID);
            config.MapODataServiceRoute(
                routeName: "ODataRoute",
                routePrefix: "events/0/0/O",
                model: builder.GetEdmModel());

            config.EnsureInitialized(); //is this necessary?
        }
    }
}