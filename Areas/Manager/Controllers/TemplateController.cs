using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Piranha.Models.Manager.TemplateModels;

namespace Piranha.Areas.Manager.Controllers
{
    public class TemplateController : ManagerController
    {
		/// <summary>
		/// Opens the insert or edit view for the template depending on
		/// weather a template id was passed to the action.
		/// </summary>
		/// <param name="id">The template id</param>
		public ActionResult Page(string id = "") {
			PageEditModel m = new PageEditModel() ; 
			
			if (id != "") {
				m = PageEditModel.GetById(new Guid(id)) ;
				ViewBag.Title = "Ändra sidmall" ;
			} else {
				ViewBag.Title = "Lägg till ny sidmall" ;
			}
			return View("PageEdit", m) ;
		}

		/// <summary>
		/// Saves the current template.
		/// </summary>
		/// <param name="m">The model</param>
		[HttpPost(), ValidateInput(false)]
		public ActionResult Page(PageEditModel m) {
			//if (ModelState.IsValid) {
				if (m.SaveAll()) {
					ModelState.Clear() ;
					ViewBag.Message = "Mallen har sparats" ;
				}
			//}
			return View("PageEdit", m) ;
		}

		/// <summary>
		/// Opens the insert or edit view for the template depending on
		/// weather a template id was passed to the action.
		/// </summary>
		/// <param name="id">The template id</param>
		public ActionResult Post(string id = "") {
			PostEditModel m = new PostEditModel() ; 
			
			if (id != "") {
				m = PostEditModel.GetById(new Guid(id)) ;
				ViewBag.Title = "Ändra artikeltyp" ;
			} else {
				ViewBag.Title = "Lägg till ny artikeltyp" ;
			}
			return View("PostEdit", m) ;
		}

		/// <summary>
		/// Saves the current template.
		/// </summary>
		/// <param name="m">The model</param>
		[HttpPost(), ValidateInput(false)]
		public ActionResult Post(PostEditModel m) {
			ViewBag.Title = "Lägg till ny artikeltyp" ;

			if (ModelState.IsValid) {
				if (m.SaveAll()) {
					ModelState.Clear() ;
					ViewBag.Title = "Ändra artikeltyp" ;
					ViewBag.Message = "Artikeltypen har sparats" ;
				} else ViewBag.Message = "Det gick inte att spara artikeltypen" ;
			}
			return View("PostEdit", m) ;
		}

		/// <summary>
		/// Deletes the specified page template.
		/// </summary>
		/// <param name="id">The template id</param>
		public ActionResult DeletePage(string id) {
			PageEditModel pm = PageEditModel.GetById(new Guid(id)) ;

			if (pm.DeleteAll())
				ViewBag.Message = "Din sidmall har raderats." ;
			else ViewBag.Message = "Ett internt fel har uppstått och din sidmall kunde inte raderas." ;
			return RedirectToAction("Index", "Page") ;
		}

		/// <summary>
		/// Deletes the specified post template.
		/// </summary>
		/// <param name="id">The template id</param>
		public ActionResult DeletePost(string id) {
			PostEditModel pm = PostEditModel.GetById(new Guid(id)) ;

			if (pm.DeleteAll())
				ViewBag.Message = "Din artikeltyp har raderats." ;
			else ViewBag.Message = "Ett internt fel har uppstått och din artikeltyp kunde inte raderas." ;
			return RedirectToAction("Index", "Post") ;
		}
    }
}
