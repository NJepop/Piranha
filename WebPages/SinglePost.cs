using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piranha.Models;

namespace Piranha.WebPages
{
	/// <summary>
	/// Standard page class for a single page.
	/// </summary>
	public abstract class SinglePost : SinglePost<PostModel> {}

	/// <summary>
	/// Page class for a single page where the model is of the generic type T.
	/// </summary>
	/// <typeparam name="T">The model type</typeparam>
	public abstract class SinglePost<T> : ContentPage<T> where T : PostModel {
		/// <summary>
		/// Initializes the web page
		/// </summary>
		protected override void InitializePage() {
			string permalink = UrlData.Count > 0 ? UrlData[UrlData.Count - 1] : "" ;

			// Load the current page model
			if (!String.IsNullOrEmpty(permalink))
				InitModel(PostModel.GetByPermalink<T>(permalink)) ;

			// Check for basic permissions
			//if (Model.Page.GroupId != Guid.Empty)
			//	if (!User.IsMember(Model.Page.GroupId)) {
			//		SysParam param = SysParam.GetByName("LOGIN_PAGE") ;
			//		if (param != null)
			//			Server.TransferRequest(param.Value) ;
			//		else Server.TransferRequest("~/") ;
			//	}
			base.InitializePage() ;
		}

		#region Private methods
		/// <summary>
		/// Initializes the instance from the given model.
		/// </summary>
		/// <param name="pm">The page model</param>
		protected virtual void InitModel(T pm) {
			Model = pm ;

			Page.Current = null ;
		}
		#endregion
	}
}
