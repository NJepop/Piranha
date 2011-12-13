using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha;
using Piranha.Web;
using Piranha.Models;

namespace Piranha.Controllers
{
	/// <summary>
	/// Base class for all piranha controllers.
	/// </summary>
	public abstract class PiranhaController : Controller
	{
		/// <summary>
		/// Checks for access rules for the current action
		/// </summary>
		/// <param name="filterContext">The context</param>
		protected override void OnActionExecuting(ActionExecutingContext filterContext) {
			// Get methodinfo for current action.
			MethodInfo m = null ;

			try {
				m = this.GetType().GetMethod(filterContext.ActionDescriptor.ActionName) ;
			} catch {
				// If this fails we have multiple actions with the same name. We'll have to try and
				// match it on FormMethod.
				this.GetType().GetMethods().Each<MethodInfo>((i, method) => {
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
					if (!User.HasAccess(attr.Function)) {
						SysParam param = SysParam.GetSingle("sysparam_name = @0", "LOGIN_PAGE") ;
						if (param != null && !String.IsNullOrEmpty(param.Value))
							filterContext.Result = new TransferResult() { RouteController = param.Value } ;
						else filterContext.Result = new TransferResult() { RouteController = "Home" } ;
					}
				}
			}
			base.OnActionExecuting(filterContext) ;
		}
	}
}
