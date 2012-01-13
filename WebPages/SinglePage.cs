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
	public abstract class SinglePage<T> : ContentPage<T> where T : PageModel {
		/// <summary>
		/// Initializes the web page
		/// </summary>
		protected override void InitializePage() {
			string permalink = UrlData.Count > 0 ? UrlData[UrlData.Count - 1] : "" ;
			
			// Load the current page model
			if (!String.IsNullOrEmpty(permalink))
				InitModel(PageModel.GetByPermalink<T>(permalink)) ;
			else InitModel(PageModel.GetByStartpage<T>()) ;

			// Check for basic permissions
			if (Model.Page.GroupId != Guid.Empty) {
				if (!User.IsMember(Model.Page.GroupId)) {
					SysParam param = SysParam.GetByName("LOGIN_PAGE") ;
					if (param != null)
						Server.TransferRequest(param.Value) ;
					else Server.TransferRequest("~/") ;
				}
				// Don't cache authenticated pages
				Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache) ;
			} else {
				// We only cache public pages
#if !DEBUG
				Response.Cache.SetETag(GenerateETag()) ;
				Response.Cache.SetLastModified(Model.Page.Updated) ;	
				if (IsCached()) {
					Response.StatusCode = 304 ;
					Response.SuppressContent = true ;
					Response.End() ;
				} else {
					Response.Cache.SetCacheability(System.Web.HttpCacheability.ServerAndPrivate) ;
					Response.Cache.SetExpires(DateTime.Now.AddMinutes(30)) ;
					Response.Cache.SetMaxAge(new TimeSpan(0, 30, 0)) ;
				}
#else
				// Don't cache when we're in DEBUG
				Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache) ;
#endif
			}
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
		/// Check if the page is cached on the client.
		/// </summary>
		private bool IsCached() {
			// Check If-None-Match
			string etag = Request.Headers["If-None-Match"] ;
			if (!String.IsNullOrEmpty(etag))
				if (etag == GenerateETag())
					return true ;

			// Check If-Modified-Since
			string mod = Request.Headers["If-Modified-Since"] ;
			if (!String.IsNullOrEmpty(mod))
				try {
					DateTime since ;
					if (DateTime.TryParse(mod, out since))
						return since >= Model.Page.Updated ;
				} catch {}
			return false ;
		}

		/// <summary>
		/// Generates the page ETag from the id and updated time.
		/// </summary>
		/// <returns>The ETag</returns>
		private string GenerateETag() {
			UTF8Encoding encoder = new UTF8Encoding() ;
			MD5CryptoServiceProvider crypto = new MD5CryptoServiceProvider() ;
			string str = Model.Page.Id.ToString() + Model.Page.Updated.ToLongTimeString() ;
			byte[] bts = crypto.ComputeHash(encoder.GetBytes(str)) ;
			return Convert.ToBase64String(bts, 0, bts.Length);
		}
		#endregion
	}
}
