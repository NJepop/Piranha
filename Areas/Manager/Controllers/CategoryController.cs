using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Piranha.Models.Manager.CategoryModels;

namespace Piranha.Areas.Manager.Controllers
{
	/// <summary>
	/// Manager area controller for the category entity.
	/// </summary>
	public class CategoryController : ManagerController
	{
		/// <summary>
		/// Edits or inserts a new category.
		/// </summary>
		/// <param name="id">The category id</param>
		public ActionResult Edit(string id = "") {
			EditModel m = new EditModel() ;

			if (id != "") {
				m = EditModel.GetById(new Guid(id)) ;
				ViewBag.Title = "Ändra kategori" ;
			} else {
				ViewBag.Title = "Lägg till ny kategori" ;
			}
			return View("Edit", m) ;
		}

		/// <summary>
		/// Saves the given model.
		/// </summary>
		/// <param name="m">The model</param>
		/// <returns></returns>
		[HttpPost()]
		public ActionResult Edit(EditModel m) {
			if (ModelState.IsValid) {
				if (m.SaveAll()) {
					ViewBag.Title = "Ändra kategori" ;
					ViewBag.Message = "Din kategori har sparats." ;
					ModelState.Clear() ;
				} else {
					ViewBag.Title = "Lägg till kategori" ;
					ViewBag.Message = "Din kategori kunde inte sparas." ;
				}
			}
			return View("Edit", m) ;
		}

		/// <summary>
		/// Deletes the category with the given id.
		/// </summary>
		/// <param name="id">The category id</param>
		public ActionResult Delete(string id) {
			EditModel m = EditModel.GetById(new Guid(id)) ;

			if (m.DeleteAll())
				ViewBag.Message = "Kategorin har raderats." ;
			else ViewBag.Message = "Ett internt fel har uppstått och kategorin kunde inte raderas." ;
			return  RedirectToAction("Index", "Post") ;
		}
	}
}
