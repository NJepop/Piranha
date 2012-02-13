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
		public virtual void HandleRequest(HttpContext context, params string[] args) {
			HandleRequest(context, false, args) ;
		}
	
		/// <summary>
		/// Handles the current request.
		/// </summary>
		/// <param name="context">The current context</param>
		/// <param name="draft">Weather to view the draft</param>
		/// <param name="args">Optional url arguments passed to the handler</param>
		protected virtual void HandleRequest(HttpContext context, bool draft, params string[] args) {
			if (args != null && args.Length > 0) {
				Permalink perm = Permalink.GetByName(args[0]) ;

				if (perm != null) {
					if (perm.Type == Permalink.PermalinkType.PAGE) {
						Page page = Page.GetSingle(perm.ParentId, draft) ;

						if (!String.IsNullOrEmpty(page.Controller)) {
							context.RewritePath("~/templates/" + page.Controller + "/" + perm.Name + 
								(draft ? "?draft=true" : ""), false) ;
						} else {
							context.RewritePath("~/page/" + perm.Name + (draft ? "?draft=true" : "")) ;
						}
					} else {
						context.RewritePath("~/post/" + perm.Name + (draft ? "?draft=true" : "")) ;
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
