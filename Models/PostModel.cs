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
		/// Gets/sets the post categories.
		/// </summary>
		public List<Category> Categories { get ; set ; }

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
			m.GetRelated() ;
			return m ;
		}

		/// <summary>
		/// Gets the post model for the given permalink.
		/// </summary>
		/// <param name="permalink">The permalink</param>
		/// <returns>The model</returns>
		public static PostModel GetByPermalink(string permalink) {
			return GetByPermalink<PostModel>(permalink) ;
		}

		/// <summary>
		/// Gets the post model for the given permalink.
		/// </summary>
		/// <param name="permalink">The permalink</param>
		/// <typeparam name="T">The model type</typeparam>
		/// <returns>The model</returns>
		public static T GetByPermalink<T>(string permalink) where T : PostModel {
			T m = Activator.CreateInstance<T>() ;

			m.Post = Post.GetByPermalink(permalink) ;
			m.GetRelated() ;
			return m ;
		}

		/// <summary>
		/// Gets the related information for the post.
		/// </summary>
		private void GetRelated() {
			// Get categories
			Categories = Category.GetByPostId(Post.Id) ;

			// Get archive
			Archive = Post.Get("post_template_id = @0", Post.TemplateId,
				new Params() { OrderBy = "post_created DESC" }) ;
		}
	}
}
