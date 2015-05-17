using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Newtonsoft.Json.Serialization;
using HashTag.Logging.Web.Library;

namespace HashTag.MVC.Elmah.RestClientDemo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Formatting.Indented,                
            };
            settings.Converters.Add(new NameValueCollectionConverter());
            JsonConvert.DefaultSettings = () => settings;
        }
    }
}
