using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Models;
using Piranha.Web;

namespace Piranha.Areas.Manager.Controllers
{
	/// <summary>
	/// Controller for the manager area with built in access control.
	/// </summary>
	public abstract class ManagerController : Controller
	{
		/// <summary>
		/// Do additional security checks for the manager area.
		/// </summary>
		/// <param name="filterContext"></param>
		protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext) {
			if (User.Identity.IsAuthenticated && User.HasAccess("ADMIN")) {
				// Get methodinfo for current action.
				MethodInfo m = null ;

				try {
					m = this.GetType().GetMethod(filterContext.ActionDescriptor.ActionName) ;
				} catch {
					// If this fails we have multiple actions with the same name. We'll have to try and
					// match it on FormMethod.
					this.GetType().GetMethods().Each((i, method) => {
						if (method.Name == filterContext.ActionDescriptor.ActionName) {
							if (Request.HttpMethod == "POST") {
								if (method.GetCustomAttribute<HttpPostAttribute>(true) != null) {
									m = method ;
								}
							} else if (Request.HttpMethod == "GET") {
								if (method.GetCustomAttribute<HttpGetAttribute>(true) != null ||
									method.GetCustomAttribute<HttpPostAttribute>(true) == null) {
									m = method ;
								}
							}
						}
					}) ;
				}

				if (m != null) {
					AccessAttribute attr = m.GetCustomAttribute<AccessAttribute>(true) ;
					if (attr != null) {
						if (!User.HasAccess(attr.Function))
							filterContext.Result = RedirectToAction("Index", "Account") ;
					}
				}
				base.OnActionExecuting(filterContext) ;
			} else {
				filterContext.Result = RedirectToAction("Index", "Account") ;
			}
		}
	}
}
