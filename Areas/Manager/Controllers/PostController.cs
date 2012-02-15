using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Piranha.Controllers;
using Piranha.Models.Manager.PostModels;

namespace Piranha.Areas.Manager.Controllers
{
    public class PostController : ManagerController
    {
		/// <summary>
		/// Default constructor. Gets the post list.
		/// </summary>
		/// <returns></returns>
	    public ActionResult Index() {
			return View("Index", ListModel.Get());
        }

		/// <summary>
		/// Creates a new post.
		/// </summary>
		/// <param name="im">The insert model</param>
		[HttpPost()]
		public ActionResult Insert(InsertModel im) {
			EditModel pm = EditModel.CreateByTemplate(im.TemplateId) ;

			ViewBag.Title = "Lägg till " + pm.Template.Name.Singular.ToLower() ;

			return View("Edit", pm) ;
		}

		/// <summary>
		/// Edits the post with the given id.
		/// </summary>
		/// <param name="id">The post id</param>
		public ActionResult Edit(string id) {
			EditModel m = EditModel.GetById(new Guid(id)) ;
			ViewBag.Title = "Ändra " + m.Template.Name.Singular.ToLower() ;

			return View(m) ;
		}

		/// <summary>
		/// Saves the model.
		/// </summary>
		/// <param name="m">The model</param>
		[HttpPost(), ValidateInput(false)]
		public ActionResult Edit(bool draft, EditModel m) {
			if (ModelState.IsValid) {
				if (m.SaveAll(draft)) {
					ModelState.Clear() ;
					if (!draft)
						ViewBag.Message = "Din artikel har publicerats." ;
					else ViewBag.Message = "Din artikel har sparats." ;
				} else ViewBag.Message = "Artikeln kunde inte sparas." ;
			}
			m.Refresh() ;

			if (m.Post.IsNew)
				ViewBag.Title = "Lägg till " + m.Template.Name.Singular.ToLower() ;
			else ViewBag.Title = "Ändra " + m.Template.Name.Singular.ToLower() ;

			return View("Edit", m) ;
		}

		/// <summary>
		/// Deletes the post.
		/// </summary>
		/// <param name="id">The post id</param>
		public ActionResult Delete(string id) {
			EditModel pm = EditModel.GetById(new Guid(id)) ;

			if (pm.DeleteAll())
				ViewBag.Message = "Din artikel har raderats." ;
			else ViewBag.Message = "Ett internt fel har uppstått och artikeln kunde inte raderas." ;

			return Index() ;
		}
    }
}
