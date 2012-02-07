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
using Piranha.WebPages.RequestHandlers;

namespace Piranha.WebPages
{
	public static class WebPiranha
	{
		#region Members
		/// <summary>
		/// The different request handlers
		/// </summary>
		private static Dictionary<string, RequestHandlerRegistration> Handlers = new Dictionary<string, RequestHandlerRegistration>() ;
		#endregion

		/// <summary>
		/// Registers the given 
		/// </summary>
		/// <param name="urlprefix">The url prefix</param>
		/// <param name="id">The handler id</param>
		/// <param name="handler">The actual handler</param>
		public static void RegisterHandler(string urlprefix, string id, IRequestHandler handler) {
			Handlers.Add(id.ToUpper(), new RequestHandlerRegistration() { UrlPrefix = urlprefix, Id = id, Handler = handler }) ;
		}

		/// <summary>
		/// Gets the current url prefix used for the given handler id.
		/// </summary>
		/// <param name="id">The handler id</param>
		/// <returns>The url prefix</returns>
		public static string GetUrlPrefixForHandlerId(string id) {
			if (Handlers.ContainsKey(id.ToUpper()))
				return Handlers[id.ToUpper()].UrlPrefix ;
			return "" ;
		}

		/// <summary>
		/// Clears all of the currently registered handlers.
		/// </summary>
		public static void ResetHandlers() {
			Handlers.Clear() ;
		}

		/// <summary>
		/// Registers all of the default request handlers.
		/// </summary>
		public static void RegisterDefaultHandlers() {
			RegisterHandler("", "STARTPAGE", new PermalinkHandler()) ;
			RegisterHandler("home", "PERMALINK", new PermalinkHandler()) ;
			RegisterHandler("media", "CONTENT", new ContentHandler()) ;
			RegisterHandler("thumb", "THUMBNAIL", new ThumbnailHandler()) ;
			RegisterHandler("preview", "PREVIEW", new PreviewHandler()) ;
			RegisterHandler("upload", "UPLOAD", new UploadHandler()) ;
			RegisterHandler("account", "ACCOUNT", new AccountHandler()) ;
		}

		/// <summary>
		/// Initializes the webb app.
		/// </summary>
		public static void Init() {
			// Register virtual path provider for the manager area
			HostingEnvironment.RegisterVirtualPathProvider(new Piranha.Web.ResourcePathProvider()) ;

			// This will trigger the manager area registration
			AreaRegistration.RegisterAllAreas() ;

			// Register handlers
			RegisterDefaultHandlers() ;
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

			string[] args = path.Split(new char[] {'/'}).Subset(1) ;

			foreach (RequestHandlerRegistration hr in Handlers.Values) {
				if (hr.UrlPrefix.ToLower() == args[0].ToLower()) {
					hr.Handler.HandleRequest(context, args.Subset(1)) ;
					break ;
				}
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
