using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Piranha.Web
{
	/// <summary>
	/// Transfers execution to the supplied action & controller.
	/// </summary>
	public class TransferResult : ActionResult
	{
		#region Properties
		/// <summary>
		/// Gets/sets the controller to route the request to.
		/// </summary>
		public string RouteController { get ; set ; }

		/// <summary>
		/// Gets/sets the action to route the request to.
		/// </summary>
		public string RouteAction { get ; set ; }

		/// <summary>
		/// Gets/sets the routes values.
		/// </summary>
		public object RouteValues { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public TransferResult() : base() {
			RouteController = "Home" ;
			RouteAction = "Index" ;
		}

		/// <summary>
		/// Executes the result
		/// </summary>
		/// <param name="context"></param>
		public override void ExecuteResult(ControllerContext context) {
			if (context != null) {
				UrlHelper urlHelper = new UrlHelper(context.RequestContext);
				string url = urlHelper.Action(RouteAction, RouteController, RouteValues) ;

				HttpContext httpContext = HttpContext.Current;

				// MVC 3 running on IIS 7+
				if (HttpRuntime.UsingIntegratedPipeline) {
					httpContext.Server.TransferRequest(url, true);
				} else {
					// Pre MVC 3
					httpContext.RewritePath(url, false);

					IHttpHandler httpHandler = new MvcHttpHandler();
					httpHandler.ProcessRequest(httpContext);
				}
			} else throw new ArgumentNullException("No context supplied");

		}
	}
}
