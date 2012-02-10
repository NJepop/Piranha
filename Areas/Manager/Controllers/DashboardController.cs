using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Piranha.Controllers;

namespace Piranha.Areas.Manager.Controllers
{
    public class DashboardController : ManagerController
    {
        public ActionResult Index() {
            return View();
        }

		public ActionResult Test() {
			return View();
		}
    }
}
