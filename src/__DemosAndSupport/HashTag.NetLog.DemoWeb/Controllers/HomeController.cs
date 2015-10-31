using HashTag.Diagnostics;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
            _log.Info.Write("Welcome to IEventLogger Demo!");
            return View();
        }

        public ActionResult About()
        {
            try
            {
                ViewBag.Message = "Your application description page.";

                using(var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                {
                    cn.Open();
                    cn.Close();
                }
                return View();
            }
            catch (SqlException sqlEx)
            {
                _log.Error.Write(new ApplicationException("this is a nested exception",sqlEx));
                throw;
            }
            catch(Exception ex)
            {
                var s = ex.ToString();
                _log.Error.Write(ex);
                throw;
            }
            
        }

        public ActionResult Contact()
        {
            _log.Info.Write("Starting controller action: {0}", "contact");
            _log.Info.Reference(14).Write("starting processing record");
            throw new NotImplementedException("demo error to elmah");
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}