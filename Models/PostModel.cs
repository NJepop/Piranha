using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piranha.Data;

namespace Piranha.Models
{
	public class PostModel : IModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the post.
		/// </summary>
		public Post Post { get ; set ; }

		/// <summary>
		/// Gets/sets the archive.
		/// </summary>
		public List<Post> Archive { get ; set ; }

		/// <summary>
		/// Gets the current page.
		/// </summary>
		public Page Page { get { return null ; } }
		#endregion

		/// <summary>
		/// Gets the post model for the given id.
		/// </summary>
		/// <param name="id">The post id</param>
		/// <returns>The model</returns>
		public static PostModel GetById(Guid id) {
			PostModel m = new PostModel() {
				Post = Post.GetSingle(id)
			} ;
			m.Archive = Post.Get("post_template_id = @0", m.Post.TemplateId,
				new Params() { OrderBy = "post_created DESC" }) ;
			return m ;
		}

		/// <summary>
		/// Gets the post model for the given permalink.
		/// </summary>
		/// <param name="permalink">The permalink</param>
		/// <returns>The model</returns>
		public static PostModel GetByPermalink(string permalink) {
			PostModel m = new PostModel() {
				Post = Post.GetByPermalink(permalink)
			};
			m.Archive = Post.Get("post_template_id = @0", m.Post.TemplateId,
				new Params() { OrderBy = "post_created DESC" }) ;
			return m ;
		}
	}
}
