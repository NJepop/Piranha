using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piranha.Models;

namespace Piranha.Web
{
	public abstract class SinglePage : PiranhaPage<PageModel>
	{
		/// <summary>
		/// Initializes the web page
		/// </summary>
		protected override void InitializePage() {
			string permalink = UrlData.Count > 0 ? UrlData[UrlData.Count - 1] : "" ;

			// Load the current page model
			if (!String.IsNullOrEmpty(permalink))
				Init(PageModel.GetByPermalink(permalink)) ;
			else Init(PageModel.GetByStartpage()) ;

			// Check for basic permissions
			if (Model.Page.GroupId != Guid.Empty)
				if (!User.IsMember(Model.Page.GroupId)) {
					SysParam param = SysParam.GetByName("LOGIN_PAGE") ;
					if (param != null)
						Server.TransferRequest(param.Value) ;
					else Server.TransferRequest("~/") ;
				}
			base.InitializePage() ;
		}

		#region Private methods
		/// <summary>
		/// Initializes the instance from the given model.
		/// </summary>
		/// <param name="pm">The page model</param>
		private void Init(PageModel pm) {
			Model = pm ;
		}
		#endregion
	}
}
