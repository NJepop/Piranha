﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Data;

namespace Piranha.Models.Manager.PageModels
{
	/// <summary>
	/// Page edit model for the manager area.
	/// </summary>
	public class EditModel
	{
		#region Binder
		public class Binder : DefaultModelBinder
		{
			/// <summary>
			/// Extend the default binder so that html strings can be fetched from the post.
			/// </summary>
			/// <param name="controllerContext">Controller context</param>
			/// <param name="bindingContext">Binding context</param>
			/// <returns>The page edit model</returns>
			public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
				EditModel model = (EditModel)base.BindModel(controllerContext, bindingContext) ;

				model.PageRegions.Each<Region>((i,m) => {
					bindingContext.ModelState.Remove("PageRegions[" + i +"].Body") ;
					m.Body = new HtmlString(bindingContext.ValueProvider.GetUnvalidatedValue("PageRegions[" + i +"].Body").AttemptedValue) ; 
				}) ;

				return model ;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the page.
		/// </summary>
		public virtual Page Page { get ; set ; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		public Permalink Permalink { get ; set ; }

		/// <summary>
		/// Gets/sets the page regions.
		/// </summary>
		public virtual List<Region> PageRegions { get ; set ; }

		/// <summary>
		/// Gets/sets the Properties.
		/// </summary>
		public virtual List<Property> Properties { get ; set ; }

		/// <summary>
		/// Gets/sets the attachments.
		/// </summary>
		public virtual List<Content> Attachments { get ; set ; }

		/// <summary>
		/// Gets/sets the available content.
		/// </summary>
		public List<Content> Content { get ; set ; }

		/// <summary>
		/// Gets/sets the current template.
		/// </summary>
		public virtual PageTemplate Template { get ; private set ; }

		/// <summary>
		/// Gets/sets the groups.
		/// </summary>
		public virtual SelectList Groups { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor, creates a new model.
		/// </summary>
		public EditModel() {
			PageRegions = new List<Region>() ;
			Properties = new List<Property>() ;
			Attachments = new List<Piranha.Models.Content>() ;
			Content = Piranha.Models.Content.Get() ;

			List<SysGroup> groups = SysGroup.GetStructure().Flatten() ;
			groups.Insert(0, new SysGroup() { Name = "Alla" }) ;
			Groups = new SelectList(groups, "Id", "Name") ;
		}

		/// <summary>
		/// Gets the model for the page specified by the given id.
		/// </summary>
		/// <param name="id">The page id</param>
		/// <param name="draft">Weather to get the draft or not.</param>
		/// <returns>The model</returns>
		public static EditModel GetById(Guid id, bool draft = true) {
			EditModel m = new EditModel() ;
			
			m.Page = Piranha.Models.Page.GetSingle(id, draft) ;
			if (m.Page == null)
				m.Page = Piranha.Models.Page.GetSingle(id) ;

			if (m.Page != null) {
				m.GetRelated() ;
			} else throw new ArgumentException("Could not find page with id {" + id.ToString() + "}") ;

			return m ;
		}

		/// <summary>
		/// Creates a new page from the given template and return it 
		/// as a edit model.
		/// </summary>
		/// <param name="templateId">The template id</param>
		/// <returns>The edit model</returns>
		public static EditModel CreateByTemplate(Guid templateId) {
			EditModel m = new EditModel() ;

			m.Page = new Piranha.Models.Page() {
				Id = Guid.NewGuid(),
				TemplateId = templateId 
			} ;
			m.GetRelated() ;

			return m ;
		}

		/// <summary>
		/// Reverts to the latest published version.
		/// </summary>
		/// <param name="id">The page id</param>
		public static void Revert(Guid id) {
			EditModel m = EditModel.GetById(id, false) ;

			// Saving this baby will overwrite the current draft
			m.SaveAll() ;

			// Now we just have to "turn back time"
			Page.Execute("UPDATE page SET page_updated = page_last_published WHERE page_id = @0 AND page_draft = 1", null, id) ;
		}

		/// <summary>
		/// Saves the page and all of it's related regions.
		/// </summary>
		/// <param name="publish">Weather the page should be published</param>
		/// <returns>Weather the operation succeeded</returns>
		public virtual bool SaveAll(bool draft = true) {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				try {
					bool firstpublish = Page.IsNew || Page.Published == DateTime.MinValue ;

					// Save page
					if (draft)
						Page.Save(tx) ;
					else Page.SaveAndPublish(tx) ;

					// Save regions & properties
					PageRegions.ForEach(r => { 
						r.IsDraft = r.IsPageDraft = true ; 
						r.Save(tx) ;
						if (!draft) {
							if (firstpublish)
								r.IsNew = true ;
							r.IsDraft = r.IsPageDraft = false ; 
							r.Save(tx) ;
						}
					}) ;
					Properties.ForEach(p => { 
						p.IsDraft = p.IsPageDraft = true ; 
						p.Save(tx) ;
						if (!draft) {
							if (firstpublish)
								p.IsNew = true ;
							p.IsDraft = p.IsPageDraft = false ; 
							p.Save(tx) ;
						}
					}) ;

					// Save permalink
					if (Permalink.IsNew)
						Permalink.Name = Permalink.Generate(!String.IsNullOrEmpty(Page.NavigationTitle) ?
							Page.NavigationTitle : Page.Title) ;
					Permalink.Save(tx) ;

					tx.Commit() ;
				} catch { tx.Rollback() ; throw ; }
			}
			return true ;
		}

		/// <summary>
		/// Deletes the page and all of it's related regions.
		/// </summary>
		/// <returns></returns>
		public virtual bool DeleteAll() {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				// Since we can have multiple rows for all id's, get everything.
				List<Region> regions = Region.GetAllByPageId(Page.Id) ;
				List<Property> properties = Property.GetAllByParentId(Page.Id) ;
				List<Page> pages = Page.Get("page_id=@0", Page.Id) ;

				regions.ForEach(r => r.Delete(tx)) ;
				properties.ForEach(p => p.Delete(tx)) ;
				Permalink.Delete(tx) ;
				pages.ForEach(p => p.Delete(tx)) ;

				tx.Commit() ;

				try {
					// Delete page preview
					// WebPages.WebThumb.RemovePagePreview(Page.Id) ;
				} catch {}
			}
			return true ;
		}

		/// <summary>
		/// Refreshes the model from the database.
		/// </summary>
		public virtual void Refresh() {
			if (Page != null) {
				if (!Page.IsNew) { // Page.Id != Guid.Empty) {
					Page = Page.GetSingle(Page.Id, true) ;
					GetRelated() ;
				} else {
					Template = PageTemplate.GetSingle("pagetemplate_id = @0", Page.TemplateId) ;
				}
			}
		}

		#region Private methods
		private void GetRelated() {
			// Clear related
			PageRegions.Clear() ;
			Properties.Clear() ;
			Attachments.Clear() ;

			// Get template & permalink
			Template  = PageTemplate.GetSingle("pagetemplate_id = @0", Page.TemplateId) ;
			Permalink = Permalink.GetByParentId(Page.Id) ;
			if (Permalink == null)
				Permalink = new Permalink() { ParentId = Page.Id, Type = Permalink.PermalinkType.PAGE } ;

			if (Template != null) {
				// Get page regions
				foreach (string name in Template.PageRegions) {
					Region reg = Region.GetSingle("region_name = @0 AND region_page_id = @1 AND region_draft = @2", 
						name, Page.Id, Page.IsDraft) ;
					if (reg != null)
						PageRegions.Add(reg) ;
					else PageRegions.Add(new Region() { Name = name, PageId = Page.Id, IsDraft = Page.IsDraft, IsPageDraft = Page.IsDraft }) ;
				} 

				// Get Properties
				foreach (string name in Template.Properties) {
					Property prp = Property.GetSingle("property_name = @0 AND property_page_id = @1 AND property_draft = @2", 
						name, Page.Id, Page.IsDraft) ;
					if (prp != null)
						Properties.Add(prp) ;
					else Properties.Add(new Property() { Name = name, PageId = Page.Id, IsDraft = Page.IsDraft, IsPageDraft = Page.IsDraft }) ;
				}
			} else throw new ArgumentException("Could not find page template for page {" + Page.Id.ToString() + "}") ;

			// Get attachments
			Attachments = Piranha.Models.Content.GetByParentId(Page.Id, true) ;
		}
		#endregion
	}
}
