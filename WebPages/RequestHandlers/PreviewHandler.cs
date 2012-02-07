using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Models;

namespace Piranha.WebPages.RequestHandlers
{
	public class PreviewHandler : IRequestHandler
	{
		/// <summary>
		/// Handles the current request.
		/// </summary>
		/// <param name="context">The current context</param>
		/// <param name="args">Optional url arguments passed to the handler</param>
		public virtual void HandleRequest(HttpContext context, params string[] args) {
			if (args != null && args.Length > 0) {
				//
				// Http preview
				//
				Page page = Page.GetSingle(new Guid(args[0])) ;
				WebThumb.GetThumbnail(context.Response, page.Id, "http://" + context.Request.Url.DnsSafeHost + 
					VirtualPathUtility.ToAbsolute("~/hem/" + page.Permalink), 300, 225) ;
			}
		}
	}
}
