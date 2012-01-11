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
		/// <returns>The posts</returns>
		public List<Post> GetPostsByType(string name) {
			return Post.Get("posttemplate_name LIKE @0", "%," + name, new Params() { OrderBy = "post_created DESC" }) ;
		}
	}
}
