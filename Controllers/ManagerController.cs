using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piranha.Web;

namespace Piranha.Controllers
{
	public abstract class ManagerController : PiranhaController
	{
		/// <summary>
		/// Do additional security checks for the manager area.
		/// </summary>
		/// <param name="filterContext"></param>
		protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext) {
			if (User.Identity.IsAuthenticated && User.HasAccess("ADMIN")) {
				// Check access control
				base.OnActionExecuting(filterContext);
			} else {
				filterContext.Result = new TransferResult() { RouteController = "Account" } ; 
			}
		}
	}
}
