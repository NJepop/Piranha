using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Models;

namespace Piranha.WebPages.RequestHandlers
{
	/// <summary>
	/// Request handler for permalinks.
	/// </summary>
	public class PermalinkHandler : IRequestHandler
	{
		/// <summary>
		/// Handles the current request.
		/// </summary>
		/// <param name="context">The current context</param>
		/// <param name="args">Optional url arguments passed to the handler</param>
		public void HandleRequest(HttpContext context, params string[] args) {
			if (args != null && args.Length > 0) {
				Permalink perm = Permalink.GetByName(args[0]) ;

				if (perm != null) {
					if (perm.Type == Permalink.PermalinkType.PAGE) {
						Page page = Page.GetSingle(perm.ParentId) ;

						if (!String.IsNullOrEmpty(page.Controller)) {
							context.RewritePath("~/templates/" + page.Controller + "/" + perm.Name, false) ;
						} else {
							context.RewritePath("~/page/" + perm.Name) ;
						}
					} else {
						context.RewritePath("~/post/" + perm.Name) ;
					}
				}
			} else {
				//
				// Rewrite to current startpage
				//
				Page page = Page.GetStartpage() ;

				if (!String.IsNullOrEmpty(page.Controller))
					context.RewritePath("~/templates/" + page.Controller, false) ;
				else context.RewritePath("~/page") ;
			}
		}
	}
}
