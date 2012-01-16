using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Piranha.Models;

namespace Piranha.WebPages
{
	/// <summary>
	/// Base class for all content pages.
	/// </summary>
	/// <typeparam name="T">The model type</typeparam>
	public abstract class ContentPage<T> : BasePage
	{
		#region Properties
		/// <summary>
		/// Gets/sets the content model.
		/// </summary>
		public new T Model { get ; protected set ; }
		#endregion

		/// <summary>
		/// Initializes the web page.
		/// </summary>
		protected override void InitializePage() {
			base.InitializePage() ;
	
			if (IsPost) {
				if (Request.Form.AllKeys.Contains("piranha_form_action")) {
					MethodInfo m = GetType().GetMethod(Request.Form["piranha_form_action"],
						BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.IgnoreCase);
					if (m != null) {
						List<object> args = new List<object>() ;
						foreach (var param in m.GetParameters())
							args.Add(ModelBinder.BindModel(param.ParameterType)) ;
						m.Invoke(this, args.ToArray()) ;
					}
				}
			}
		}

		#region Cache methods
		/// <summary>
		/// Handle HTTP client caching.
		/// </summary>
		/// <returns>If the page is client cached or not</returns>
		protected bool HandleCache() {
#if !DEBUG
			DateTime mod = GetLastModified() ;
			string etag = GenerateETag(mod) ;

			Response.Cache.SetETag(etag) ;
			Response.Cache.SetLastModified(mod) ;	
			Response.Cache.SetCacheability(System.Web.HttpCacheability.ServerAndPrivate) ;
			Response.Cache.SetExpires(DateTime.Now.AddMinutes(Convert.ToInt32(SysParam.GetByName("CACHE_PUBLIC_EXPIRES").Value))) ;
			Response.Cache.SetMaxAge(new TimeSpan(0, Convert.ToInt32(SysParam.GetByName("CACHE_PUBLIC_MAXAGE").Value), 0)) ;

			if (IsCached(mod, etag)) {
				Response.StatusCode = 304 ;
				Response.SuppressContent = true ;
				Response.End() ;
				return true ;
			}
			return false ;
#else
			// Don't cache when we're in DEBUG
			Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache) ;
			return false ;
#endif
		}

		/// <summary>
		/// Check if the page is client cached.
		/// </summary>
		/// <param name="modified">Last modification date</param>
		/// <param name="entitytag">Entity tag</param>
		protected bool IsCached(DateTime modified, string entitytag) {
			// Check If-None-Match
			string etag = Request.Headers["If-None-Match"] ;
			if (!String.IsNullOrEmpty(etag))
				if (etag == entitytag)
					return true ;

			// Check If-Modified-Since
			string mod = Request.Headers["If-Modified-Since"] ;
			if (!String.IsNullOrEmpty(mod))
				try {
					DateTime since ;
					if (DateTime.TryParse(mod, out since))
						return since >= modified ;
				} catch {}
			return false ;
		}

		/// <summary>
		/// Generates the unique entity tag for the page.
		/// </summary>
		/// <param name="modified">Last modified date</param>
		/// <returns>The entity tag</returns>
		protected abstract string GenerateETag(DateTime modified) ;

		/// <summary>
		/// Gets the last modification date for the page.
		/// </summary>
		/// <returns>The modification date</returns>
		protected abstract DateTime GetLastModified();
		#endregion
	}
}
