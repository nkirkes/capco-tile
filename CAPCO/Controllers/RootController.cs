using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CAPCO.Controllers
{
    public class RootController : ApplicationController
    {
        public ActionResult Index()
        {
            return RedirectToAction("index", "home", new { area = "" });
        }
    }
}
