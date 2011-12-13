using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Archive model for the cms application.
	/// </summary>
	public class ArchiveModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the posts in the current archive.
		/// </summary>
		public List<Post> Archive { get ; set ; }

		/// <summary>
		/// Gets/sets the template for the current archive.
		/// </summary>
		public PostTemplate Template { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new archive model.
		/// </summary>
		public ArchiveModel() {
			Archive = new List<Post>() ;
		}

		/// <summary>
		/// Gets the archive model for the given name.
		/// </summary>
		/// <param name="archivename">The template archive name</param>
		/// <returns>The model.</returns>
		public static ArchiveModel GetByArchiveName(string archivename) {
			ArchiveModel am = new ArchiveModel() ;

			am.Template = PostTemplate.GetSingle("posttemplate_archive_name = @0", archivename) ;
			if (am.Template != null)
				am.Archive = Post.Get("post_template_id = @0", am.Template.Id, new Params() { OrderBy = "post_created DESC" }) ;
			return am ;
		}
	}
}
