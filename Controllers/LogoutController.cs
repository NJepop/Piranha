using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Piranha.Controllers
{
	/// <summary>
	/// Logout controller.
	/// </summary>
	public class LogoutController : Controller
	{
		/// <summary>
		/// Performs a logout and redirects the user.
		/// </summary>
		/// <param name="id">Optional return url</param>
		public ActionResult Index(string id) {
			FormsAuthentication.SignOut() ;
			Session.Clear() ;
			Session.Abandon() ;

			if (!String.IsNullOrEmpty(id))
				return RedirectToRoute(id) ;
			return RedirectToAction("Index", "Home") ;
		}
	}
}
