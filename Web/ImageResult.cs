using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Piranha.Web
{
	/// <summary>
	/// Abstract base class for all image results.
	/// </summary>
	public abstract class ImageResult : ActionResult
	{
		#region Properties
		/// <summary>
		/// Gets/sets the base path for the image result.
		/// </summary>
		protected string BasePath { get ; set ; }

		/// <summary>
		/// Gets/sets the file path for the image result.
		/// </summary>
		public string Filename { get ; set ; }

		/// <summary>
		/// Gets/sets the file name for the image result.
		/// </summary>
		public string ContentType { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="basepath">The base path</param>
		/// <param name="file">The file path</param>
		protected ImageResult(string basepath, string filename, string contenttype) {
			BasePath = basepath ;
			Filename = filename ;
			ContentType = contenttype ;
		}

		/// <summary>
		/// Execute 
		/// </summary>
		/// <param name="context"></param>
		public override void ExecuteResult(ControllerContext context) {
			var res = context.HttpContext.Response;

            res.Clear() ;

			if (File.Exists(context.HttpContext.Server.MapPath(BasePath + (!BasePath.EndsWith("/") ? "/" : "") + Filename))) {
				res.Cache.SetCacheability(HttpCacheability.NoCache) ;
				res.ContentType = ContentType ;
				res.TransmitFile(context.HttpContext.Server.MapPath(BasePath + 
					(!BasePath.EndsWith("/") ? "/" : "") + Filename)) ;
			} else {
				res.StatusCode = 404 ;
				res.Close() ;
			}
		}
	}
}
