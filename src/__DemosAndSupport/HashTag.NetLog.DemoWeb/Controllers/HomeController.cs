using HashTag.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HashTag.NetLog.DemoWeb.Controllers
{
    public class HomeController : Controller
    {
        IEventLogger _log = EventLogger.GetLogger(typeof(HomeController));
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            _log.Info.Write("Starting controller action");
            _log.Info.Reference(14).Write("starting processing record");
         //   throw new NotImplementedException("you don't need no stink'n phone number");
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}