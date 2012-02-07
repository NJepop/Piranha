using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

using Piranha.Models;

namespace Piranha.WebPages.RequestHandlers
{
	/// <summary>
	/// Handler for login/logout/create accout.
	/// </summary>
	public class AccountHandler : IRequestHandler
	{
		/// <summary>
		/// Handles the current request.
		/// </summary>
		/// <param name="context">The current context</param>
		/// <param name="args">Optional url arguments passed to the handler</param>
		public void HandleRequest(HttpContext context, params string[] args) {
			if (args != null && args.Length > 0) {
				if (args[0].ToLower() == "login") {
					string login  = context.Request["login"] ;
					string passwd = context.Request["password"] ;
					string returl = context.Request["returnurl"] ;
					bool persist  = context.Request["remeberme"] == "1" ;

					SysUser.LoginUser(login, passwd, persist) ;
					if (!String.IsNullOrEmpty(returl))
						context.Response.Redirect(returl) ;
					context.Response.Redirect("~/") ;
				}
			}
		}
	}
}
