using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace Piranha.Web
{
	/// <summary>
	/// HttpApplication for a Pirahna mvc application.
	/// </summary>
	public class MvcPiranha : HttpApplication
	{
		/// <summary>
		/// Registers all global filters.
		/// </summary>
		/// <param name="filters">The current filter collection</param>
		public virtual void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute());
		}

		/// <summary>
		/// Registers all routes.
		/// </summary>
		/// <param name="routes">The current route collection</param>
		public virtual void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			//
			// Swedish routing
			//
			routes.MapRoute("Default", "", new { controller = "Home", action = "Index" }) ;
			routes.MapRoute("Page", "hem.aspx/{permalink}", new { controller = "Home", action = "Permalink" }) ;
			routes.MapRoute("Custom", "hem.aspx/sidor/{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = ""});
			routes.MapRoute("Post", "hem.aspx/artiklar/{permalink}", new { controller = "Home", action = "Index", id = ""});
			//routes.MapRoute("Archive", "hem.aspx/arkiv/{templatename}", new { controller = "Home", action = "Archive" }) ;
			//routes.MapRoute("ArchiveYear", "hem.aspx/arkiv/{templatename}/{year}", new { controller = "Home", action = "Archive" }) ;
			routes.MapRoute("ArchiveMonth", "hem.aspx/arkiv/{templatename}/{year}/{month}", new { controller = "Home", action = "Archive", year = UrlParameter.Optional, month = UrlParameter.Optional }) ;
		}

		/// <summary>
		/// Registers all custom binders.
		/// </summary>
		public virtual void RegisterBinders() {
			ModelBinders.Binders.Add(typeof(Piranha.Models.Manager.PageModels.EditModel), 
				new Piranha.Models.Manager.PageModels.EditModel.Binder()) ;
			ModelBinders.Binders.Add(typeof(Piranha.Models.Manager.PostModels.EditModel), 
				new Piranha.Models.Manager.PostModels.EditModel.Binder()) ;
			ModelBinders.Binders.Add(typeof(Piranha.Models.Manager.TemplateModels.PageEditModel),
				new Piranha.Models.Manager.TemplateModels.PageEditModel.Binder()) ;
		}

		/// <summary>
		/// Starts the application.
		/// </summary>
		protected virtual void Application_Start() {
			// Register areas
			AreaRegistration.RegisterAllAreas();

			// Register routes and binders
			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
			RegisterBinders() ;

			// Register virtual path provider for the manager area
            HostingEnvironment.RegisterVirtualPathProvider(new ResourcePathProvider()) ;
		}
	}
}
