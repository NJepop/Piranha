using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Piranha.Controllers;
using Piranha.Models.Manager.PageModels;

namespace byBrick.Areas.Manager.Controllers
{
    public class PageController : ManagerController
    {
		/// <summary>
		/// Default controller. Gets the page list.
		/// </summary>
        public ActionResult Index() {
			return View("Index", ListModel.Get());
        }

		/// <summary>
		/// Opens the edit view for the selected page.
		/// </summary>
		/// <param name="id">The page id</param>
		public ActionResult Edit(string id) {
			EditModel pm = EditModel.GetById(new Guid(id)) ;

			ViewBag.Title = "Ändra sida" ;

			return View("Edit", pm) ;
		}

		/// <summary>
		/// Saves the currently edited page.
		/// </summary>
		/// <param name="pm">The page model</param>
		[HttpPost(), ValidateInput(false)]
		public ActionResult Edit(EditModel pm) {
			try {
				if (pm.SaveAll())
					ViewBag.Message = "Din sida har sparats" ;
				else ViewBag.Message = "Det gick inte att spara sidan" ;
			} catch (Exception e) {
				ViewBag.Message = e.ToString() ;
			}
			pm.Refresh();

			if (pm.Page.IsNew)
				ViewBag.Title = "Lägg till " + pm.Template.Name.Singular.ToLower() ;
			else ViewBag.Title = "Ändra sida" ;

			return View("Edit", pm) ;
		}

		/// <summary>
		/// Creates a new page.
		/// </summary>
		/// <param name="im">The insert model</param>
		[HttpPost()]
		public ActionResult Insert(InsertModel im) {
			EditModel pm = EditModel.CreateByTemplate(im.TemplateId) ;
			pm.Page.TemplateId = im.TemplateId ;
			pm.Page.ParentId = im.ParentId ;
			pm.Page.Seqno = im.Seqno ;

			ViewBag.Title = "Lägg till " + pm.Template.Name.Singular.ToLower() ;

			return View("Edit", pm) ;
		}

		/// <summary>
		/// Deletes the page specified by the given id.
		/// </summary>
		/// <param name="id">The page id</param>
		public ActionResult Delete(string id) {
			EditModel pm = EditModel.GetById(new Guid(id)) ;

			if (pm.DeleteAll())
				ViewBag.Message = "Din sida har raderats." ;
			else ViewBag.Message = "Ett internt fel har uppstått och sidan kunde inte raderas." ;

			return Index() ;
		}
    }
}
