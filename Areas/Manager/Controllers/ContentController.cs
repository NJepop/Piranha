using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Piranha.Controllers;
using Piranha.Models.Manager.ContentModels;

namespace byBrick.Areas.Manager.Controllers
{
    public class ContentController : ManagerController
    {
		/// <summary>
		/// Gets the list view.
		/// </summary>
        public ActionResult Index() {
            return View("Index", ListModel.Get());
        }

		/// <summary>
		/// Edits or inserts a new content model.
		/// </summary>
		/// <param name="id">The id of the content</param>
		public ActionResult Edit(string id) {
			if (!String.IsNullOrEmpty(id)) {
				ViewBag.Title = "Ändra bild eller dokument" ;
				return View("Edit", EditModel.GetById(new Guid(id))) ;
			} else {
				ViewBag.Title = "Lägg till ny bild eller dokument" ;
				return View("Edit", new EditModel()) ;
			}
		}

		/// <summary>
		/// Saves the current edit model.
		/// </summary>
		/// <param name="m">The model</param>
		[HttpPost()]
		public ActionResult Edit(EditModel m) {
			if (m.SaveAll()) {
				ViewBag.Title = "Ändra bild eller dokument" ;
				ViewBag.Message = "Din bild eller ditt dokument har sparats." ;
				return View("Edit", m) ;
			} else {
				ViewBag.Title = "Lägg till bild eller dokument" ;
				ViewBag.Message = "Din bild eller ditt dokument kunde inte sparas." ;
				return View("Edit", m) ;
			}
		}

		/// <summary>
		/// Deletes the specified content record.
		/// </summary>
		/// <param name="id">The content id</param>
		public ActionResult Delete(string id) {
			EditModel m = EditModel.GetById(new Guid(id)) ;

			if (m.DeleteAll()) {
				ViewBag.Message = "Din bild eller ditt dokument har raderats." ;
			} else {
				ViewBag.Message = "Din bild eller ditt dokument kunde inte raderas." ;
			} 
			return Index() ;
		}
    }
}
