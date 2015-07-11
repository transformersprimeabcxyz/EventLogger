using HashTag.NetLog.DemoWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HashTag.NetLog.DemoWeb.Controllers
{
    public class InjectedController : Controller
    {
        IInjectedService _svc;
        public InjectedController(IInjectedService svc)
        {
            _svc = svc;
        }
        // GET: Injected
        public ActionResult Index()
        {
            _svc.SaveRecords(new List<int>() { 1, 2, 3, 4, 5 });
            return View();
        }
    }
}