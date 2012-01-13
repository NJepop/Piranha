using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace Piranha.Models
{
	/// <summary>
	/// Page model for the from cms application.
	/// </summary>
	public class PageModel
	{
		#region Properties
		/// <summary>
		/// Gets the page.
		/// </summary>
		public IPage Page { get ; set ; }

		/// <summary>
		/// Gets the available html regions for the page.
		/// </summary>
		public dynamic Regions { get ; private set ; }

		/// <summary>
		/// Gets the available Properties.
		/// </summary>
		public dynamic Properties { get ; private set ; }

		/// <summary>
		/// Gets the available attachments.
		/// </summary>
		public List<Content> Attachments { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new empty page model.
		/// </summary>
		public PageModel() {
			Regions       = new ExpandoObject() ;
			Properties    = new ExpandoObject() ;
			Attachments   = new List<Content>() ;
		}

		#region Static accessors
		/// <summary>
		/// Gets the page model for the given page.
		/// </summary>
		/// <param name="p">The page record</param>
		/// <returns>The model</returns>
		public static PageModel Get(Page p) {
			PageModel m = new PageModel() {
				Page = p
			} ;
			m.Init() ;
			return m ;
		}

		/// <summary>
		/// Gets the page model for the startpage.
		/// </summary>
		/// <returns>The model</returns>
		public static PageModel GetByStartpage() {
			return GetByStartpage<PageModel>() ;
		}

		/// <summary>
		/// Gets tne page model for the startpage.
		/// </summary>
		/// <typeparam name="T">The model type</typeparam>
		/// <returns>The model</returns>
		public static T GetByStartpage<T>() where T : PageModel {
			T m = Activator.CreateInstance<T>() ;

			m.Page = Models.Page.GetStartpage() ;
			m.Init() ;
			return m ;
		}

		/// <summary>
		/// Gets the page model for the specified permalink.
		/// </summary>
		/// <param name="permalink">The permalink</param>
		/// <returns>The model</returns>
		public static PageModel GetByPermalink(string permalink) {
			return GetByPermalink<PageModel>(permalink) ;
		}

		/// <summary>
		/// Gets the page model for the specified permalink.
		/// </summary>
		/// <typeparam name="T">The model type</typeparam>
		/// <param name="permalink">The permalink</param>
		/// <returns>The model</returns>
		public static T GetByPermalink<T>(string permalink) where T : PageModel {
			T m = Activator.CreateInstance<T>() ;

			m.Page = Models.Page.GetByPermalink(permalink) ;
			m.Init() ;
			return m ;
		}
		
		/// <summary>
		/// Gets the page model for the specified page id.
		/// </summary>
		/// <param name="id">The page id</param>
		/// <returns>The model</returns>
		public static PageModel GetById(Guid id) {
			PageModel m = new PageModel() {
				Page = Models.Page.GetSingle(id)
			} ;
			m.Init() ;
			return m ;
		}

		/// <summary>
		/// Gets the page model for the current route. This method is only for MVC use.
		/// </summary>
		/// <typeparam name="T">The page model type</typeparam>
		/// <param name="route">Optional route. Overrides RouteData if provided</param>
		/// <returns>The model</returns>
		public static T GetByRoute<T>(string route = "") where T : PageModel {
			RouteData rd = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current)) ;

			string controller = (string)rd.Values["controller"] ;
			string action     = (string)rd.Values["action"] ;

			if (route == "")
				route = controller + (action.ToLower() != "index" ? "/" + action : "") ;

			if (controller.ToLower() != "home") {
				T m = Activator.CreateInstance<T>() ;
				m.Page = Models.Page.GetSingle("page_controller = @0 OR (page_controller is NULL AND pagetemplate_controller = @0)", route) ;

				if (m.Page.GroupId != Guid.Empty) {
					if (!HttpContext.Current.User.Identity.IsAuthenticated || !HttpContext.Current.User.IsMember(m.Page.GroupId))
						throw new UnauthorizedAccessException("The current user doesn't have access to the requested page.") ;
				}

				m.Init() ;
				return m ;
			}
			throw new InvalidOperationException("GetByRoute() is only applicable for custom controllers.") ;
		}
		#endregion

		/// <summary>
		/// Gets the associated regions for the current page
		/// </summary>
		protected void Init() {
			PageTemplate pt = PageTemplate.GetSingle(((Page)Page).TemplateId) ;

			// Page regions
			foreach (string str in pt.PageRegions) {
				Region pr = Region.GetSingle("region_page_id = @0 AND region_name = @1", Page.Id, str) ;
				((IDictionary<string, object>)Regions).Add(str, pr != null ? pr.Body : new HtmlString("")) ;
			}
			// Properties
			foreach (string str in pt.Properties) {
				Property pr = Property.GetSingle("property_page_id = @0 AND property_name = @1", Page.Id, str) ;
				((IDictionary<string, object>)Properties).Add(str, pr != null ? pr.Value : "") ;
			}
		}
	}
}
