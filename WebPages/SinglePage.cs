using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using Piranha.Models;

namespace Piranha.WebPages
{
	/// <summary>
	/// Standard page class for a single page.
	/// </summary>
	public abstract class SinglePage : SinglePage<PageModel> {}

	/// <summary>
	/// Page class for a single page where the model is of the generic type T.
	/// </summary>
	/// <typeparam name="T">The model type</typeparam>
	public abstract class SinglePage<T> : ContentPage<T> where T : PageModel
	{
		#region Members
		private Models.Page page = null ;
		#endregion

		/// <summary>
		/// Initializes the web page
		/// </summary>
		protected override void InitializePage() {
			string permalink = UrlData.Count > 0 ? UrlData[UrlData.Count - 1] : "" ;
			bool   cached = false ;
			
			// Load the current page
			if (!String.IsNullOrEmpty(permalink))
				page = Models.Page.GetByPermalink(permalink) ;
			else page = Models.Page.GetStartpage() ;

			// Check permissions
			if (page.GroupId != Guid.Empty) {
				if (!User.IsMember(page.GroupId)) {
					SysParam param = SysParam.GetByName("LOGIN_PAGE") ;
					if (param != null)
						Server.TransferRequest(param.Value) ;
					else Server.TransferRequest("~/") ;
				}
				Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache) ;
			} else {
				// Only cache public pages
				cached = HandleCache() ;
			}
			// Load the model if the page wasn't cached
			if (!cached)
				InitModel(PageModel.Get<T>(page)) ;
			base.InitializePage() ;
		}

		#region Private methods
		/// <summary>
		/// Initializes the instance from the given model.
		/// </summary>
		/// <param name="pm">The page model</param>
		protected virtual void InitModel(T pm) {
			Model = pm ;

			Page.Current = Model.Page ;
		}

		/// <summary>
		/// Generates the unique entity tag for the page.
		/// </summary>
		/// <param name="modified">Last modified date</param>
		/// <returns>The entity tag</returns>
		protected override string GenerateETag(DateTime modified) {
			UTF8Encoding encoder = new UTF8Encoding() ;
			MD5CryptoServiceProvider crypto = new MD5CryptoServiceProvider() ;

			string str = page.Id.ToString() + modified.ToLongTimeString() ;
			byte[] bts = crypto.ComputeHash(encoder.GetBytes(str)) ;
			return Convert.ToBase64String(bts, 0, bts.Length);
		}

		/// <summary>
		/// Gets the last modification date for the page.
		/// </summary>
		/// <returns>The modification date</returns>
		protected override DateTime GetLastModified() {
			return page.Updated ;
		}
		#endregion
	}
}
