using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Piranha.Web;
using Piranha.Models;

namespace Piranha.Controllers
{
	public class UploadController : Controller
	{
		/// <summary>
		/// Gets the image or document associated with the id.
		/// </summary>
		/// <param name="id">Content id</param>
		public ActionResult Get(string id) {
			Upload ur = Upload.GetSingle(new Guid(id)) ;
			return new UploadResult(ur) ;
		}

		/// <summary>
		/// Gets the upload record by its parent id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ActionResult GetByParent(string id) {
			Upload ur = Upload.GetSingleByParentId(new Guid(id)) ;
			return new UploadResult(ur) ;
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
