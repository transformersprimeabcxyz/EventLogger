using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HashTag.Logging.Web.Service.Controllers
{
    public class EventSummaryController : Controller
    {
        [Route("EventSummary"),HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [Route("Event/{id:guid}"), HttpGet]
        public ActionResult Index(Guid id)
        {
            return View("Details",id);
        }
    }
}