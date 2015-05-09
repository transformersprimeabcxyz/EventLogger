using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace HashTag.MVC.Elmah.RestClientDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            var asm = Assembly.Load("HashTag.Elmah.RestProxy");
            throw new NotImplementedException("something really went bad!");

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