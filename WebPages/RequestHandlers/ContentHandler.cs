using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Models;

namespace Piranha.WebPages.RequestHandlers
{
	/// <summary>
	/// Request handler for content.
	/// </summary>
	public class ContentHandler : IRequestHandler
	{
		/// <summary>
		/// Handles the current request.
		/// </summary>
		/// <param name="context">The current context</param>
		/// <param name="args">Optional url arguments passed to the handler</param>
		public void HandleRequest(HttpContext context, params string[] args) {
			if (args != null && args.Length > 0) {
				Content content = Content.GetSingle(new Guid(args[0])) ;

				if (content != null) {
					int? width = null ;

					if (args.Length > 1)
						width = Convert.ToInt32(args[1]) ;
					content.GetMedia(context, width) ;
				}
			}
		}
	}
}
