using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;

using Piranha.WebPages;

namespace Piranha.Web
{
	public class ResourceHandler : IHttpHandler
	{
		/// <summary>
		/// Gets weather this handler is reusable or not.
		/// </summary>
		public bool IsReusable { 
			get { return false ; } 
		}

		/// <summary>
		/// Processes the http request.
		/// </summary>
		/// <param name="context">The current context</param>
		public void ProcessRequest(HttpContext context) {
			ResourcePathProvider res = new ResourcePathProvider() ;

			if (res.FileExists(context.Request.Path)) {
				var file = res.GetFile(context.Request.Path) ;

				DateTime mod = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime ;
				string etag = WebPiranha.GenerateETag(file.VirtualPath, mod) ;

				if (!WebPiranha.HandleClientCache(context, etag, mod)) {
					if (file.Name.EndsWith(".js")) {
						context.Response.ContentType = "text/javascript" ;
					} else if (file.Name.EndsWith(".css")) {
						context.Response.ContentType = "text/css" ;
					}
					var stream = file.Open() ;
					byte[] bytes = new byte[stream.Length] ;

					stream.Read(bytes, 0, Convert.ToInt32(stream.Length)) ;
					context.Response.BinaryWrite(bytes) ;
					stream.Close() ;
				}
			}
		}
	}
}
