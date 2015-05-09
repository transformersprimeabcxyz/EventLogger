using HashTag.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace HashTag.MVC.Elmah.RestClientDemo.Controllers
{
    public class HomeController : Controller
    {
        ILog _log = Log.NewLog(typeof(HomeController));
        public ActionResult Index()
        {
            try
            {
                using(var cn = new SqlConnection("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=dbLog;Data Source=powell-pc\\sqlexpressS"))
                {
                    cn.Open();
                }

            }
            catch(Exception ex)
            {
                ex.Data["test data"] = "test message";

                var msg = _log.Error.Catch(ex).Message("something really, really bad happened at {0}", DateTime.Now);
                msg.Properties.Add("form fields","other stuff");
                var js = JsonConvert.SerializeObject(msg,Formatting.Indented);
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