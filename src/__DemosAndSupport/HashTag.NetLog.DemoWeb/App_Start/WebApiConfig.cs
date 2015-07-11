using HashTag.NetLog.DemoWeb.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace HashTag.NetLog.DemoWeb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.EnsureInitialized();
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

        }
    }

    public class ElmahExceptionLogger : ExceptionLogger
    {
        private const string HttpContextBaseKey = "MS_HttpContext";

        public override void Log(ExceptionLoggerContext context)
        {
            HttpContext httpContext = GetHttpContext(context.Request);

            if (httpContext == null)
            {
                return;
            }

            // Wrap the exception in an HttpUnhandledException so that ELMAH can capture the original error page.
            Exception exceptionToRaise = new HttpUnhandledException(message: null, innerException: context.Exception);

            // Send the exception to ELMAH (for logging, mailing, filtering, etc.).
            Elmah.ErrorSignal signal = Elmah.ErrorSignal.FromContext(httpContext);
            signal.Raise(exceptionToRaise, httpContext);
        }

        private static HttpContext GetHttpContext(HttpRequestMessage request)
        {
            HttpContextBase contextBase = GetHttpContextBase(request);

            if (contextBase == null)
            {
                return null;
            }

            return ToHttpContext(contextBase);
        }

        private static HttpContextBase GetHttpContextBase(HttpRequestMessage request)
        {
            if (request == null)
            {
                return null;
            }

            object value;

            if (!request.Properties.TryGetValue(HttpContextBaseKey, out value))
            {
                return null;
            }

            return value as HttpContextBase;
        }

        private static HttpContext ToHttpContext(HttpContextBase contextBase)
        {
            return contextBase.ApplicationInstance.Context;
        }
    }

}