using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Piranha.Models
{
	/// <summary>
	/// Client API for the post.
	/// </summary>
	public interface IPost {
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		Guid Id { get ; }

		/// <summary>
		/// Gets/sets the title.
		/// </summary>
		string Title { get ; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		string Permalink { get ; }

		/// <summary>
		/// Gets/sets the excerpt.
		/// </summary>
		string Excerpt { get ; }

		/// <summary>
		/// Gets/sets the body.
		/// </summary>
		HtmlString Body { get ; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		DateTime Created { get ; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		DateTime Updated { get ; }

		/// <summary>
		/// Gets/sets the published date.
		/// </summary>
		DateTime Published { get ; }
	}
}
