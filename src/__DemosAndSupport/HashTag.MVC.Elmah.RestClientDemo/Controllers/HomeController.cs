using HashTag.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using HashTag.Web.Http;

namespace HashTag.MVC.Elmah.RestClientDemo.Controllers
{
    public class HomeController : Controller
    {
        IEventLogger _log = LogEventLoggerFactory.NewLogger<HomeController>();

        public ActionResult Index()
        {
            
            try
            {

                

            }
            catch(Exception ex)
            {              
                throw;
            }
            
            return View();
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
    public class TestFilter :ILogEventFilter
    {

        public bool Matches(LogEvent logEvent)
        {
            return true;
        }

        public void Initialize(object config)
        {
            
        }

        public void Initialize(IDictionary<string, string> config)
        {
            throw new NotImplementedException();
        }
    }
}