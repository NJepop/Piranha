using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

using Piranha.Models;

namespace Piranha.WebPages
{
	public static class WebPiranha
	{
		/// <summary>
		/// Initializes the webb app.
		/// </summary>
		public static void Init() {
			// Register virtual path provider for the manager area
			HostingEnvironment.RegisterVirtualPathProvider(new Piranha.Web.ResourcePathProvider()) ;

			// This will trigger the manager area registration
			AreaRegistration.RegisterAllAreas() ;
		}

		/// <summary>
		/// Initializes the manager app.
		/// </summary>
		/// <param name="context"></param>
		public static void InitManager(AreaRegistrationContext context) {
			// Register manager routing
			context.MapRoute(
				"Manager",
				"manager/{controller}/{action}/{id}",
				new { controller = "Page", action = "Index", id = UrlParameter.Optional }
			) ;

			// Register filters & binders
			RegisterGlobalFilters(GlobalFilters.Filters) ;
			RegisterBinders() ;
		}

		/// <summary>
		/// Handles the URL Rewriting for the application
		/// </summary>
		/// <param name="context">Http context</param>
		public static void BeginRequest(HttpContext context) {
			string path = context.Request.Path.Substring(context.Request.ApplicationPath.Length > 1 ? 
				context.Request.ApplicationPath.Length : 0) ;

			// If this is a call to "hem" then URL rewrite
			if (path.StartsWith("/hem/")) {
				Permalink perm = Permalink.GetByName(path.Substring(5)) ;

				if (perm != null) {
					if (perm.Type == Permalink.PermalinkType.PAGE) {
						Page page = Page.GetSingle(perm.ParentId) ;

						if (!String.IsNullOrEmpty(page.Controller)) {
							context.RewritePath("~/templates/" + page.Controller + "/" + perm.Name, false) ;
						} else {
							context.RewritePath("~/page/" + perm.Name) ;
						}
					} else {
						context.RewritePath("~/post/" + perm.Name) ;
					}
				} else {
					string str = path.Substring(5).ToLower() ;
					if (str == "perm") {
						//
						// TODO: Generate RSS feed for all posts
						//
					}
				}
			} else if (path.StartsWith("/media/")) {
				//
				// Media content
				//
				string[] param = path.Substring(7).Split(new char[] { '/' }) ;
				Content content = Content.GetSingle(new Guid(param[0])) ;

				if (content != null) {
					int? width = null ;

					if (param.Length > 1)
						width = Convert.ToInt32(param[1]) ;
					content.GetMedia(context, width) ;
				}
			} else if (path.StartsWith("/thumb/")) {
				//
				// Thumbnail content
				//
				string[] param = path.Substring(7).Split(new char[] { '/' }) ;
				Content content = Content.GetSingle(new Guid(param[0])) ;

				if (content != null) {
					if (param.Length == 1)
						content.GetThumbnail(context) ;
					else content.GetThumbnail(context, Convert.ToInt32(param[1])) ;
				}
			} else if (path.StartsWith("/preview/")) {
				//
				// Http preview
				//
				Page page = Page.GetSingle(new Guid(path.Substring(9))) ;
				WebThumb.GetThumbnail(context.Response, page.Id, "http://" + context.Request.Url.DnsSafeHost + 
					VirtualPathUtility.ToAbsolute("~/hem/" + page.Permalink), 300, 225) ;
			} else if (path.StartsWith("/upload/")) {
				//
				// Uploaded content
				//
				string [] param = path.Substring(8).Split(new char[] { '/' }) ;
				Upload upload = Upload.GetSingle(new Guid(param[0])) ;

				if (upload != null)
					upload.GetFile(context.Response) ;
			} else if (path == "/") {
				//
				// Rewrite to current startpage
				//
				Page page = Page.GetStartpage() ;

				if (!String.IsNullOrEmpty(page.Controller))
					context.RewritePath("~/templates/" + page.Controller, false) ;
				else context.RewritePath("~/page") ;
			}
		}

		/// <summary>
		/// Checks request headers against the given etag and last modification data and
		/// sets the correct response headers. Returns weather the file is client cached 
		/// or should be loaded/rendered.
		/// </summary>
		/// <param name="context">The current context</param>
		/// <param name="etag">The entity tag</param>
		/// <param name="modified">Last nodification</param>
		/// <returns>If the file is cached</returns>
		public static bool HandleClientCache(HttpContext context, string etag, DateTime modified, bool noexpire = false) {
#if !DEBUG
			if (!context.Request.IsLocal) {
				try {
					DateTime siteDt = DateTime.Parse(SysParam.GetByName("SITE_LAST_MODIFIED").Value) ;
					modified = modified > siteDt ? modified : siteDt ;
				} catch {}
				context.Response.Cache.SetETag(etag) ;
				context.Response.Cache.SetLastModified(modified <= DateTime.Now ? modified : DateTime.Now) ;	
				context.Response.Cache.SetCacheability(System.Web.HttpCacheability.ServerAndPrivate) ;
				if (!noexpire) {
					context.Response.Cache.SetExpires(DateTime.Now.AddMinutes(Convert.ToInt32(SysParam.GetByName("CACHE_PUBLIC_EXPIRES").Value))) ;
					context.Response.Cache.SetMaxAge(new TimeSpan(0, Convert.ToInt32(SysParam.GetByName("CACHE_PUBLIC_MAXAGE").Value), 0)) ;
				} else {
					context.Response.Cache.SetExpires(DateTime.Now) ;
				}
				if (IsCached(context, modified, etag)) {
					context.Response.StatusCode = 304 ;
					context.Response.SuppressContent = true ;
					context.Response.End() ;
					return true ;
				}
			} else {
				context.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache) ;
			}
#else
			context.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache) ;
#endif
			return false ;
		}

		/// <summary>
		/// Generates an unique entity tag.
		/// </summary>
		/// <param name="name">Object name</param>
		/// <param name="modified">Last modified date</param>
		/// <returns>The entity tag</returns>
		public static string GenerateETag(string name, DateTime modified) {
			UTF8Encoding encoder = new UTF8Encoding() ;
			MD5CryptoServiceProvider crypto = new MD5CryptoServiceProvider() ;

			try {
				DateTime siteDt = DateTime.Parse(SysParam.GetByName("SITE_LAST_MODIFIED").Value) ;
				modified = modified > siteDt ? modified : siteDt ;
			} catch {}

			string str = name + modified.ToLongTimeString() ;
			byte[] bts = crypto.ComputeHash(encoder.GetBytes(str)) ;
			return Convert.ToBase64String(bts, 0, bts.Length);
		}

		/// <summary>
		/// Check if the page is client cached.
		/// </summary>
		/// <param name="modified">Last modification date</param>
		/// <param name="entitytag">Entity tag</param>
		private static bool IsCached(HttpContext context, DateTime modified, string entitytag) {
			// Check If-None-Match
			string etag = context.Request.Headers["If-None-Match"] ;
			if (!String.IsNullOrEmpty(etag))
				if (etag == entitytag)
					return true ;

			// Check If-Modified-Since
			string mod = context.Request.Headers["If-Modified-Since"] ;
			if (!String.IsNullOrEmpty(mod))
				try {
					DateTime since ;
					if (DateTime.TryParse(mod, out since))
						return since >= modified ;
				} catch {}
			return false ;
		}


		#region Private methods
		/// <summary>
		/// Registers all global filters.
		/// </summary>
		/// <param name="filters">The current filter collection</param>
		private static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute());
		}

		/// <summary>
		/// Registers all custom binders.
		/// </summary>
		private static void RegisterBinders() {
			ModelBinders.Binders.Add(typeof(Piranha.Models.Manager.PageModels.EditModel), 
				new Piranha.Models.Manager.PageModels.EditModel.Binder()) ;
			ModelBinders.Binders.Add(typeof(Piranha.Models.Manager.PostModels.EditModel), 
				new Piranha.Models.Manager.PostModels.EditModel.Binder()) ;
			ModelBinders.Binders.Add(typeof(Piranha.Models.Manager.TemplateModels.PageEditModel),
				new Piranha.Models.Manager.TemplateModels.PageEditModel.Binder()) ;
		}
		#endregion
	}
}
