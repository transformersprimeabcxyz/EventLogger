using Newtonsoft.Json;
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

            var dic = new Dictionary<string, string>();
            dic.Add("key", "value");
            dic.Add("key2", "value");
            
            var js = JsonConvert.SerializeObject(dic,Formatting.Indented);
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