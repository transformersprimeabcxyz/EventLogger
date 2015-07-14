using HashTag.Diagnostics;
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
        IEventLogger _log;
        IInjectedService _svc;
        public InjectedController(IInjectedService svc,IEventLogger log)
        {
            _svc = svc;
            _log = log;
        }
        // GET: Injected
        public ActionResult Index()
        {
            _log.Verbose.Write("this is a verbose message from controller");
            _log.Info.Write("this is a info message from injected controller");
            _svc.SaveRecords(new List<int>() { 1, 2, 3, 4, 5 });
            return View();
        }
    }
}