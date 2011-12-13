using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

using Yahoo.Yui.Compressor;

namespace Piranha.Areas.Manager.Content.Css
{
	/// <summary>
	/// Summary description for Css
	/// </summary>
	public class Style : IHttpHandler
	{
		#region Properties
		public bool IsReusable {
			get { return false ; }
		}
		#endregion

		/// <summary>
		/// Process the request
		/// </summary>
		public void ProcessRequest(HttpContext context) {
			StreamReader io = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(
				"Piranha.Areas.Manager.Content.Css.Style.css")) ;

			context.Response.ContentType = "text/css" ;
#if DEBUG
			context.Response.Write(io.ReadToEnd()) ;
#else
			context.Response.Write(CssCompressor.Compress(io.ReadToEnd()).Replace("\n","")) ;
#endif
			io.Close() ;

			// Now check for application specific styles
			if (File.Exists(context.Server.MapPath("~/Areas/Manager/Content/Css/Style.css"))) {
				io = new StreamReader(context.Server.MapPath("~/Areas/Manager/Content/Css/Style.css")) ;
#if DEBUG
				context.Response.Write(io.ReadToEnd()) ;
#else
				context.Response.Write(CssCompressor.Compress(io.ReadToEnd()).Replace("\n","")) ;
#endif
				io.Close() ;
			}
		}
	}
}