using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.WebPages;
using System.Web.WebPages.Html;

using Piranha.Models;

namespace Piranha.Web
{
	/// <summary>
	/// Base class for Razor syntax web pages.
	/// </summary>
	public abstract class PiranhaPage : WebPage
	{
		#region Inner classes
		/// <summary>
		/// View helper class.
		/// </summary>
		public class PiranhaHelper
		{
			#region Members
			private HtmlHelper Html ;
			#endregion

			/// <summary>
			/// Default constructor.
			/// </summary>
			/// <param name="html"></param>
			public PiranhaHelper(HtmlHelper html) {
				Html = html ;
			}


		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the helper for the piranha methods
		/// </summary>
		public PiranhaHelper Piranha { get ; private set ; }

		/// <summary>
		/// Gets the page model.
		/// </summary>
		public PageModel PageModel { get ; private set ; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new page.
		/// </summary>
		public PiranhaPage() : base() {
			Piranha = new PiranhaHelper(Html) ;
		}

		/// <summary>
		/// Initializes the web page
		/// </summary>
		protected override void InitializePage() {
			string permalink = UrlData[0] ;

			// Load the current page model
			if (!String.IsNullOrEmpty(permalink))
				PageModel = PageModel.GetByPermalink(permalink) ;
			else PageModel = PageModel.GetByStartpage() ;

			// Check for custom controller
			if (!String.IsNullOrEmpty(PageModel.Page.Controller))
				if (PageModel.Page.Controller != TemplateInfo.VirtualPath)
					Response.Redirect(PageModel.Page.Controller) ;

			// Check for basic permissions
			if (PageModel.Page.GroupId != Guid.Empty)
				if (!User.IsMember(PageModel.Page.GroupId)) {
					SysParam param = SysParam.GetSingle("sysparam_name = @0", "LOGIN_PAGE") ;
					if (param != null)
						Response.RedirectToRoute(param.Value) ;
					else Response.Redirect("~/") ;
				}

			base.InitializePage() ;
		}
	}
}
