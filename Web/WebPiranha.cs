using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

using Piranha.Models;

namespace Piranha.Web
{
	public static class WebPiranha
	{
		/// <summary>
		/// Initializes the webb app.
		/// </summary>
		public static void Init() {
			// Register virtual path provider for the manager area
			HostingEnvironment.RegisterVirtualPathProvider(new ResourcePathProvider()) ;

			AreaRegistration.RegisterAllAreas() ;
			RegisterGlobalFilters(GlobalFilters.Filters) ;
			RegisterRoutes(RouteTable.Routes) ;
			RegisterBinders() ;
		}

		/// <summary>
		/// Handles the URL Rewriting for the application
		/// </summary>
		/// <param name="context">Http context</param>
		public static void BeginRequest(HttpContext context) {
			string path = context.Request.Path.Substring(context.Request.ApplicationPath.Length > 1 ? 
				context.Request.ApplicationPath.Length : 0) ;
        
			// If this is a call to "hem" then URL rewrite
			if (path.StartsWith("/hem/")) {
				Permalink perm = Permalink.GetByName(path.Substring(5)) ;
				if (perm != null) {
					if (perm.Type == Permalink.PermalinkType.PAGE) {
						Page page = Page.GetSingle(perm.ParentId) ;
						if (!String.IsNullOrEmpty(page.Controller)) {
							context.RewritePath("~/" + page.Controller + "/" + perm.Name) ;
						} else {
							context.RewritePath("~/page/" + perm.Name) ;
						}
					} else {
						context.RewritePath("~/post/" + perm.Name) ;
					}
				}
			} else if (path == "/") {
				//
				// Rewrite to current startpage
				//
				Page page = Page.GetStartpage() ;

				if (!String.IsNullOrEmpty(page.Controller))
					context.RewritePath("~/" + page.Controller) ;
				else context.RewritePath("~/page") ;
			}
		}

		#region Private methods
		/// <summary>
		/// Registers all routes.
		/// </summary>
		/// <param name="routes">The current route collection</param>
		private static void RegisterRoutes(RouteCollection routes) {
			/*routes.MapRoute("Manager",
				"Manager.aspx/{controller}/{action}/{id}",
				new { controller = "Page", action = "Index", id = UrlParameter.Optional }) ;*/
		}

		/// <summary>
		/// Registers all global filters.
		/// </summary>
		/// <param name="filters">The current filter collection</param>
		private static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute());
		}

		/// <summary>
		/// Registers all custom binders.
		/// </summary>
		private static void RegisterBinders() {
			ModelBinders.Binders.Add(typeof(Piranha.Models.Manager.PageModels.EditModel), 
				new Piranha.Models.Manager.PageModels.EditModel.Binder()) ;
			ModelBinders.Binders.Add(typeof(Piranha.Models.Manager.PostModels.EditModel), 
				new Piranha.Models.Manager.PostModels.EditModel.Binder()) ;
			ModelBinders.Binders.Add(typeof(Piranha.Models.Manager.TemplateModels.PageEditModel),
				new Piranha.Models.Manager.TemplateModels.PageEditModel.Binder()) ;
		}
		#endregion
	}
}
