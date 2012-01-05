using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Models;

namespace Piranha.WebPages
{
	/// <summary>
	/// Caches objects from the database.
	/// </summary>
	public static class PiranhaCache
	{
		#region Inner classes
		/// <summary>
		/// The different types och cacheable objects.
		/// </summary>
		public enum CacheObject { PERMALINK, PAGE, CONTENT }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the permalink cache object.
		/// </summary>
		private static Dictionary<string, Permalink> PermalinkCache {
			get {
				if (HttpContext.Current.Cache[CacheObject.PERMALINK.ToString()] == null)
					HttpContext.Current.Cache[CacheObject.PERMALINK.ToString()] = new Dictionary<string, Permalink>() ;
				return (Dictionary<string, Permalink>)HttpContext.Current.Cache[CacheObject.PERMALINK.ToString()] ;
			}
		}

		/// <summary>
		/// Gets the page cache object.
		/// </summary>
		private static Dictionary<Guid, Page> PageCache {
			get {
				if (HttpContext.Current.Cache[CacheObject.PAGE.ToString()] == null)
					HttpContext.Current.Cache[CacheObject.PAGE.ToString()] = new Dictionary<Guid, Page>() ;
				return (Dictionary<Guid, Page>)HttpContext.Current.Cache[CacheObject.PAGE.ToString()] ;
			}
		}

		/// <summary>
		/// Gets the page cache object.
		/// </summary>
		private static Dictionary<Guid, Content> ContentCache {
			get {
				if (HttpContext.Current.Cache[CacheObject.CONTENT.ToString()] == null)
					HttpContext.Current.Cache[CacheObject.CONTENT.ToString()] = new Dictionary<Guid, Content>() ;
				return (Dictionary<Guid, Content>)HttpContext.Current.Cache[CacheObject.CONTENT.ToString()] ;
			}
		}
		#endregion

		/// <summary>
		/// Invalidates the entire piranha application cache.
		/// </summary>
		public static void InvalidateCache() {
			InvalidateCache(CacheObject.CONTENT) ;
			InvalidateCache(CacheObject.PAGE) ;
			InvalidateCache(CacheObject.PERMALINK) ;
		}

		/// <summary>
		/// Invalidates the cache for the given cache object.
		/// </summary>
		/// <param name="obj">The cache object</param>
		public static void InvalidateCache(CacheObject obj) {
			HttpContext.Current.Cache[obj.ToString()] = null ;
		}

		/// <summary>
		/// Gets the permalink with given name from the cache.
		/// </summary>
		/// <param name="name">The permalink name</param>
		/// <returns>The permalink</returns>
		public static Permalink GetPermalinkByName(string name) {
			if (!PermalinkCache.ContainsKey(name))
				PermalinkCache[name] = Permalink.GetByName(name) ;
			return PermalinkCache[name] ;
		}

		/// <summary>
		/// Gets the page with the given id from the cache.
		/// </summary>
		/// <param name="id">The page id</param>
		/// <returns>The page</returns>
		public static Page GetPage(Guid id) {
			if (!PageCache.ContainsKey(id))
				PageCache[id] = Page.GetSingle(id) ;
			return PageCache[id] ;
		}

		/// <summary>
		/// Gets the startpage from the cache.
		/// </summary>
		/// <returns>The startpage</returns>
		public static Page GetStartpage() {
			if (!PageCache.ContainsKey(Guid.Empty))
				PageCache[Guid.Empty] = Page.GetStartpage() ;
			return PageCache[Guid.Empty] ;
		}

		/// <summary>
		/// Gets the content with the given id from the cache.
		/// </summary>
		/// <param name="id">The content id</param>
		/// <returns>The content</returns>
		public static Content GetContent(Guid id) {
			if (!ContentCache.ContainsKey(id))
				ContentCache[id] = Content.GetSingle(id) ;
			return ContentCache[id] ;
		}
	}
}
