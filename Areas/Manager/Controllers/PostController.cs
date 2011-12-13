using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Piranha.Controllers;
using Piranha.Models.Manager.PostModels;

namespace byBrick.Areas.Manager.Controllers
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
			return View(EditModel.GetById(new Guid(id))) ;
		}

		/// <summary>
		/// Saves the model.
		/// </summary>
		/// <param name="m">The model</param>
		[HttpPost(), ValidateInput(false)]
		public ActionResult Edit(EditModel m) {
			if (m.SaveAll()) {
				m.Refresh() ;
				ViewBag.Title = "Ändra " + m.Post.TemplateName.Singular.ToLower() ;
				ViewBag.Message = "Din artikel har sparats." ;
			} else {
				m.Refresh() ;
				ViewBag.Title = "Lägg till " + m.Post.TemplateName.Singular.ToLower() ;
				ViewBag.Message = "Artikeln kunde inte sparas." ;
			}

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
