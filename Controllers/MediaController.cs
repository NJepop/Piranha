using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Models;
using Piranha.Web;

namespace Piranha.Controllers
{
	/// <summary>
	/// Controller responsible for returning content to the application
	/// </summary>
	public class MediaController : Controller
	{
		/// <summary>
		/// Gets the image or document associated with the content id.
		/// </summary>
		/// <param name="id">Content id</param>
		public ActionResult Get(string id) {
			Content cr = Piranha.Models.Content.GetSingle(new Guid(id)) ;
			return new PiranhaImageResult(cr) ;
		}

		/// <summary>
		/// Gets a thumbnail for the content with the given id.
		/// </summary>
		/// <param name="id">Content id</param>
		/// <param name="width">Optional width</param>
		/// <param name="height">Optional height</param>
		public ActionResult Thumbnail(string id, string width, string height) {
			return null ;
		}
	}
}
