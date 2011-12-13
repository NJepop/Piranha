using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using Piranha.Models;

namespace Piranha.Controllers
{
	/// <summary>
	/// Login controller.
	/// </summary>
	public class LoginController : Controller
	{
		/// <summary>
		/// Performs a logon with the given information.
		/// </summary>
		/// <param name="m">The model</param>
		[HttpPost()]
		public ActionResult Index(LoginModel m) {
			// Authenticate the user
			if (ModelState.IsValid) {
				SysUser user = SysUser.Authenticate(m.Login, m.Password) ;
				if (user != null) {
					FormsAuthentication.SetAuthCookie(user.Id.ToString(), m.RememberMe) ;
					HttpContext.Session[PiranhaApp.USER] = user ;
				}
			}
			// Redirect after logon
			if (!String.IsNullOrEmpty(m.ReturnPermalink))
				return RedirectToAction("Permalink", "Home", new { @permalink = m.ReturnPermalink }) ;
			if (!String.IsNullOrEmpty(m.ReturnController))
				return RedirectToAction(!String.IsNullOrEmpty(m.ReturnAction) ?
					m.ReturnAction : "Index", m.ReturnController) ;
			return RedirectToAction("Index", "Home") ;
		}
	}
}
