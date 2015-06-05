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
        ILog _log = LogFactory.Create.NewLog(typeof(HomeController));

        public ActionResult Index()
        {
            
            try
            {

                LogEventProcessorSettings settings = new LogEventProcessorSettings();
               // settings.Pipeline.Add(new TestWriter());
                settings.Processor.ForceFlushFilters = new ILogEventFilter[] { new TestFilter() };
                settings.ShouldLogEventFilters.Add(new TestFilter());
                settings.ShouldLogEventFilters.Add(new TestFilter());
                var s = JsonConvert.SerializeObject(settings,new JsonSerializerSettings()
                    {
                         TypeNameHandling = TypeNameHandling.Objects
                    });


                Log.Configure(); 
             //throw new NotImplementedException("hello isn't working today...so leave already");

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