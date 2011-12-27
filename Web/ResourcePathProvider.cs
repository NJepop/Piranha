using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Piranha.Web
{
	public class ResourcePathProvider : VirtualPathProvider
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public ResourcePathProvider() {}

		/// <summary>
		/// Checks if the given virtual file exists in the assembly
		/// </summary>
		/// <param name="virtualpath">The virtual path</param>
		/// <returns>If the path exists</returns>
		private bool IsResourceFile(string virtualpath) {
			ManifestResourceInfo info = Assembly.GetExecutingAssembly().GetManifestResourceInfo(
				VirtualPathUtility.ToAppRelative(virtualpath).Replace("/", ".").Replace("~", "Piranha").Replace("res.ashx.", "")) ;
			return !File.Exists(HttpContext.Current.Server.MapPath(virtualpath)) && info != null ;
		}

		/// <summary>
		/// Checks if the file located at the given path exists.
		/// </summary>
		/// <param name="virtualpath">Theh virtual path</param>
		/// <returns>If the file exists</returns>
        public override bool FileExists(string virtualpath) {
            return (IsResourceFile(virtualpath) || base.FileExists(virtualpath)) ;
        }

		/// <summary>
		/// Gets the resource file with at the given virtual path.
		/// </summary>
		/// <param name="virtualpath">The virtual file</param>
		/// <returns>The virtual file</returns>
        public override VirtualFile GetFile(string virtualpath) {
            if (IsResourceFile(virtualpath))
                return new ResourceVirtualFile(virtualpath) ;
            return base.GetFile(virtualpath) ;
        }

		/// <summary>
		/// Gets the cache dependency
		/// </summary>
		/// <param name="virtualpath">The virtual path</param>
		/// <param name="dependencies">The dependencies</param>
		/// <param name="start">Start date</param>
		/// <returns>The dependency</returns>
        public override CacheDependency GetCacheDependency(string virtualpath, IEnumerable dependencies, DateTime start) {
            if (IsResourceFile(virtualpath))
                return null ;
            return base.GetCacheDependency(virtualpath, dependencies, start) ;
        }
	}

	/// <summary>
	/// Class for defining a virtual file embedded as a resource.
	/// </summary>
    public class ResourceVirtualFile : VirtualFile
	{
		#region Members
		private string path ;
		#endregion

		/// <summary>
		/// Default constructor. Creates a new resource file for the given
		/// virtual path.
		/// </summary>
		/// <param name="virtualpath">The virtual path</param>
		public ResourceVirtualFile(string virtualpath) : base(virtualpath) {
            path = VirtualPathUtility.ToAppRelative(virtualpath);
        }

		/// <summary>
		/// Opens the stream for the current resource.
		/// </summary>
		/// <returns>The stream</returns>
        public override Stream Open() {
			string res = path.Replace("/", ".").Replace("~", "Piranha").Replace("res.ashx.", "") ;

			return Assembly.GetExecutingAssembly().GetManifestResourceStream(res) ;
        }
    }
}
