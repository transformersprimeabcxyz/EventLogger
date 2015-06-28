
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

using NLog;

namespace HashTag.MVC.Elmah.RestClientDemo.Controllers
{
    public class HomeController : Controller
    {
        //IEventLogger _log = LoggerFactory.NewLogger<HomeController>();
        ILogger _log = LogManager.GetLogger(typeof(HomeController).FullName);
        public ActionResult Index()
        {
            //_log.Fatal("this is a string with param: {0}", DateTime.Now);
            var tgt = new SplunkTarget();
            var ctx = HttpContext;
            var ctxCurrent = HttpContext.Request;
            var x = 1000;
            try
            {
                while (--x > -1)
                {
                    var y = x / x;
                }
            }
            catch(Exception ex)
            {
                for (x = 0; x < 1000; x++)
                {
                    ex.Data["interation"] = x;
                    _log.Error(ex, ex.Message);
                }
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

}