﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HashTag.Logging.Web.Service.Controllers
{
    public class HomeController : Controller
    {
        [Route("~/")]
        public ActionResult Index()
        {
            return View();
        }
    }
}