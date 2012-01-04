using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piranha.Data;

namespace Piranha.Models.Manager.PostModels
{
	/// <summary>
	/// Page list model for the manager area.
	/// </summary>
	public class ListModel 
	{
		#region Properties
		/// <summary>
		/// Gets/sets the pages.
		/// </summary>
		public List<Post> Posts { get ; set ; }

		/// <summary>
		/// Gets/sets the categories.
		/// </summary>
		public List<Category> Categories { get ; set ; }

		/// <summary>
		/// Gets/sets the page templates.
		/// </summary>
		public List<PostTemplate> Templates { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor, creates a new model.
		/// </summary>
		public ListModel() {
			Posts = new List<Post>() ;
			Categories = Category.GetStructure().Flatten() ;
			Templates = PostTemplate.Get(new Params() { OrderBy = "posttemplate_name ASC" }) ;
		}

		/// <summary>
		/// Gets the list model for all available pages.
		/// </summary>
		/// <returns>The model.</returns>
		public static ListModel Get() {
			ListModel m = new ListModel() ;
			m.Posts = Post.Get(new Params() { OrderBy = "post_title ASC" }) ;

			return m ;
		}
	}
}
