using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Piranha.WebPages.RequestHandlers
{
	/// <summary>
	/// A request handler is a chunk of code that is inserted into BeginRequest,
	/// such as the default handler for permalinks.
	/// </summary>
	public interface IRequestHandler
	{
		/// <summary>
		/// Handles the current request.
		/// </summary>
		/// <param name="context">The current context</param>
		/// <param name="args">Optional url arguments passed to the handler</param>
		void HandleRequest(HttpContext context, params string[] args) ;
	}
}
