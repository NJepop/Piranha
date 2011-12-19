using System;
using System.Collections.Generic;
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
	public class PageModel : IModel
	{
		#region Properties
		/// <summary>
		/// Gets the page.
		/// </summary>
		public Page Page { get ; set ; }

		/// <summary>
		/// Gets the available page regions.
		/// </summary>
		public Dictionary<string, Region> PageRegions { get ; set ; }

		/// <summary>
		/// Gets the available global regions.
		/// </summary>
		public Dictionary<string, Region> GlobalRegions { get ; set ; }

		/// <summary>
		/// Gets the available Properties.
		/// </summary>
		public Dictionary<string, Property> Properties { get ; set ; }

		/// <summary>
		/// Gets the available attachments.
		/// </summary>
		public List<Content> Attachments { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new empty page model.
		/// </summary>
		public PageModel() {
			// Initialize the regions
			PageRegions   = new Dictionary<string, Region>() ;
			GlobalRegions = new Dictionary<string, Region>() ;
			Properties    = new Dictionary<string, Property>() ;
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
		/// <returns></returns>
		public static PageModel GetByStartpage() {
			PageModel m = new PageModel() {
				Page = Page.GetStartpage()
			} ;
			m.Init() ;
			return m ;
		}

		/// <summary>
		/// Gets the page model for the specified permalink.
		/// </summary>
		/// <param name="permalink">The permalink</param>
		/// <returns>The model</returns>
		public static PageModel GetByPermalink(string permalink) {
			PageModel m = new PageModel() {
				Page = Page.GetByPermalink(permalink)
			} ;
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
				Page = Page.GetSingle(id)
			} ;
			m.Init() ;
			return m ;
		}

		/// <summary>
		/// Gets the page model for the current route.
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
				m.Page = Page.GetSingle("page_controller = @0 OR (page_controller is NULL AND pagetemplate_controller = @0)", route) ;
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
			PageTemplate pt = PageTemplate.GetSingle(this.Page.TemplateId) ;

			// Page regions
			foreach (string str in pt.PageRegions) {
				Region pr = Region.GetSingle("region_page_id = @0 AND region_name = @1",
					this.Page.Id, str) ;
				this.PageRegions.Add(str, pr != null ? pr : new Region()) ;
			}
			// Global regions
			foreach (string str in pt.GlobalRegions) {
				Region gr = Region.GetSingle("region_page_id IS NULL AND region_name = @0", str) ;
				this.GlobalRegions.Add(str, gr != null ? gr : new Region()) ;
			}
			// Properties
			foreach (string str in pt.Properties) {
				Property pr = Property.GetSingle("property_page_id = @0 AND property_name = @1", 
					this.Page.Id, str) ;
				this.Properties.Add(str, pr != null ? pr : new Property()) ;
			}
		}
	}
}
