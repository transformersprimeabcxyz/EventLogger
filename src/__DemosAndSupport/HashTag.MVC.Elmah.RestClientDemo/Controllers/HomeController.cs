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
                
             throw new NotImplementedException("hello isn't working today...so leave already");

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
}