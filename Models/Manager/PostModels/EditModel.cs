using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Data;

namespace Piranha.Models.Manager.PostModels
{
	/// <summary>
	/// Edit model for a post in the manager area.
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

				bindingContext.ModelState.Remove("Post.Body") ;

				model.Post.Body = 
					new HtmlString(bindingContext.ValueProvider.GetUnvalidatedValue("Post.Body").AttemptedValue) ;

				return model ;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the post.
		/// </summary>
		public Post Post { get ; set ; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		public Permalink Permalink { get ; set ; }

		/// <summary>
		/// Gets/sets the post template.
		/// </summary>
		public PostTemplate Template { get ; set ; }

		/// <summary>
		/// Gets/sets the categories associated with the post.
		/// </summary>
		public List<Category> PostCategories { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public EditModel() {
			Post = new Post() ;
		}

		/// <summary>
		/// Creates a new post from the given template and return it 
		/// as a edit model.
		/// </summary>
		/// <param name="templateId">The template id</param>
		/// <returns>The edit model</returns>
		public static EditModel CreateByTemplate(Guid templateId) {
			EditModel m = new EditModel() ;

			m.Post = new Piranha.Models.Post() {
				Id = Guid.NewGuid(),
				TemplateId = templateId 
			} ;
			m.Template = PostTemplate.GetSingle(templateId) ;
			m.Permalink = new Permalink() { 
				ParentId = m.Post.Id, Type = Permalink.PermalinkType.POST } ;
			m.PostCategories = new List<Category>() ;

			return m ;
		}

		/// <summary>
		/// Gets the model for the post
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public static EditModel GetById(Guid id) {
			EditModel m = new EditModel() ;
			m.Post = Piranha.Models.Post.GetSingle(id) ;
			m.Template = PostTemplate.GetSingle(m.Post.TemplateId) ;
			m.Permalink = Permalink.GetSingle("permalink_parent_id = @0", m.Post.Id) ;
			if (m.Permalink == null)
				m.Permalink = new Permalink() { 
					ParentId = m.Post.Id, Type = Permalink.PermalinkType.POST } ;
			m.PostCategories = Category.GetByPostId(m.Post.Id) ;

			return m ;
		}

		/// <summary>
		/// Saves the model.
		/// </summary>
		/// <returns>Weather the action was successful</returns>
		public bool SaveAll() {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				try {
					Post.Save(tx) ;
					if (Permalink.IsNew)
						Permalink.Name = Permalink.Generate(Post.Title) ;
					Permalink.Save(tx) ;
					tx.Commit() ;
				} catch { tx.Rollback() ; throw ; }
			}
			return true ;
		}

		/// <summary>
		/// Deletes the post.
		/// </summary>
		/// <returns></returns>
		public virtual bool DeleteAll() {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				try {
					Permalink.Delete(tx) ;
					Post.Delete(tx) ;
					tx.Commit() ;
				} catch { tx.Rollback() ; return false ; }
			}
			return true ;
		}

		/// <summary>
		/// Refreshes the current model.
		/// </summary>
		public void Refresh() {
			if (Post != null) {
				if (!Post.IsNew) {
					Post = Piranha.Models.Post.GetSingle(Post.Id) ;
					Permalink = Permalink.GetSingle("permalink_parent_id = @0", Post.Id) ;
				}
				Template = PostTemplate.GetSingle(Post.TemplateId) ;
			}
		}
	}
}
