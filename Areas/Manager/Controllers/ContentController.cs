using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Piranha.Models.Manager.ContentModels;

namespace Piranha.Areas.Manager.Controllers
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
				EditModel m = EditModel.GetById(new Guid(id)) ;
				if (m.Content.IsImage)
					ViewBag.Title = "Ändra bild" ;
				else ViewBag.Title = "Ändra dokument" ;

				return View("Edit", m) ;
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
				if (m.Content.IsImage) {
					ViewBag.Title = "Ändra bild" ;
					ViewBag.Message = "Din bild har sparats." ;
				} else {
					ViewBag.Title = "Ändra dokument" ;
					ViewBag.Message = "Ditt dokument har sparats." ;
				}
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
