using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piranha.Data;
using Piranha.Models;

namespace Piranha.WebPages
{
	/// <summary>
	/// Client API for accessing the models.
	/// </summary>
	public class PiranhaFacade
	{
		/// <summary>
		/// Gets the available posts for the given post type.
		/// </summary>
		/// <param name="name">The post type name.</param>
		/// <returns>A list of posts</returns>
		public List<Post> GetPostsByType(string name) {
			return Post.Get("posttemplate_name LIKE @0", "%," + name, new Params() { OrderBy = "post_created DESC" }) ;
		}

		/// <summary>
		/// Gets the posts for the given category name.
		/// </summary>
		/// <param name="name">The category name</param>
		/// <returns>A list of posts</returns>
		public List<Post> GetPostByCategoryName(string name) {
			return Post.Get("post_id IN (" +
				"SELECT relation_data_id FROM relation WHERE relation_type = @0 AND relation_related_id = (" +
				"SELECT category_id FROM category WHERE category_name = @1))", Relation.RelationType.POSTCATEGORY, name) ;
		}

		/// <summary>
		/// Gets the posts for the given category id.
		/// </summary>
		/// <param name="id">The category id</param>
		/// <returns>A list of posts</returns>
		public List<Post> GetPostByCategoryId(Guid id) {
			return Post.Get("post_id IN (" +
				"SELECT relation_data_id FROM relation WHERE relation_type = @0 AND relation_related_id = @1)",
				Relation.RelationType.POSTCATEGORY, id) ;
		}
	}
}
