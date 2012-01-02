using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Piranha.Data;

namespace Piranha.Models.Manager.TemplateModels
{
	/// <summary>
	/// Post template edit model for the manager area.
	/// </summary>
	public class PostEditModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the post template.
		/// </summary>
		public PostTemplate Template { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new model.
		/// </summary>
		public PostEditModel() {
			Template = new PostTemplate() ;
		}

		/// <summary>
		/// Gets the model for the template specified by the given id.
		/// </summary>
		/// <param name="id">The template id</param>
		/// <returns>The model</returns>
		public static PostEditModel GetById(Guid id) {
			PostEditModel m = new PostEditModel() ;
			m.Template = PostTemplate.GetSingle(id) ;

			return m ;
		}

		/// <summary>
		/// Saves the model.
		/// </summary>
		/// <returns>Weather the operation succeeded</returns>
		public bool SaveAll() {
			try {
				return Template.Save() ;
			} catch { return false ; }
		}

		/// <summary>
		/// Deletes the post template and all posts associated with it.
		/// </summary>
		/// <returns>Weather the operation succeeded</returns>
		public bool DeleteAll() {
			List<Post> posts = Post.Get("post_template_id = @0", Template.Id) ;

			using (IDbTransaction tx = Database.OpenTransaction()) {
				try {
					foreach (Post post in posts) {
						post.Delete(tx) ;
					}
					Template.Delete(tx) ;
					tx.Commit() ;
				} catch { tx.Rollback() ; return false ; }
			}
			return true ;
		}
	}
}
