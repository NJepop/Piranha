using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Data;

namespace Piranha.Models.Manager.TemplateModels
{
	/// <summary>
	/// Page template edit model for the manager area.
	/// </summary>
	public class PageEditModel
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
				PageEditModel model = (PageEditModel)base.BindModel(controllerContext, bindingContext) ;

				model.Template.Preview =
					new HtmlString(bindingContext.ValueProvider.GetUnvalidatedValue("Template.Preview").AttemptedValue) ;
				return model ;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the page template
		/// </summary>
		public virtual PageTemplate Template { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor, creates a new model.
		/// </summary>
		public PageEditModel() {
			Template = new PageTemplate() ;
		}

		/// <summary>
		/// Gets the model for the template specified by the given id.
		/// </summary>
		/// <param name="id">The template id</param>
		/// <returns>The model</returns>
		public static PageEditModel GetById(Guid id) {
			PageEditModel m = new PageEditModel() ;
			m.Template = PageTemplate.GetSingle(id) ;

			return m ;
		}

		/// <summary>
		/// Saves the model.
		/// </summary>
		/// <returns>Weather the operation succeeded</returns>
		public virtual bool SaveAll() {
			try {
				return Template.Save() ;
			} catch { return false ; }
		}

		/// <summary>
		/// Deletes the page template and all pages associated with it.
		/// </summary>
		/// <returns>Weather the operation succeeded</returns>
		public virtual bool DeleteAll() {
			List<Piranha.Models.Page> pages = Piranha.Models.Page.Get("page_template_id = @0", Template.Id) ;

			using (IDbTransaction tx = Database.OpenTransaction()) {
				try {
					foreach (Piranha.Models.Page page in pages) {
						Region.Get("region_page_id = @0", page.Id).ForEach((r) => r.Delete(tx)) ;
						page.Delete(tx) ;
					}
					Template.Delete(tx) ;
					tx.Commit() ;
				} catch { tx.Rollback() ; return false ; }
			}
			return true ;
		}
	}
}
