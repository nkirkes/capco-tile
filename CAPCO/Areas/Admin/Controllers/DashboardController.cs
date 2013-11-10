using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAPCO.Controllers;

namespace CAPCO.Areas.Admin.Controllers
{
    public class DashboardController : BaseAdminController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
