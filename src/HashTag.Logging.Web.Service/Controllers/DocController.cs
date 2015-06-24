using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HashTag.Logging.Web.Service.Controllers
{
    public class DocController : Controller
    {
        // GET: Doc
        [Route("doc"),HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}