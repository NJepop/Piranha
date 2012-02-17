using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piranha.Models
{
	/// <summary>
	/// Sitemap record interface.
	/// </summary>
	public interface ISitemap
	{
		#region Fields
		/// <summary>
		/// Gets the id.
		/// </summary>
		Guid Id { get ; }

		/// <summary>
		/// Gets the group needed to view the page.
		/// </summary>
		Guid GroupId { get ; }

		/// <summary>
		/// Gets the parent id.
		/// </summary>
		Guid ParentId { get ; }

		/// <summary>
		/// Gets the seqno specifying the structural position.
		/// </summary>
		int Seqno { get ; }

		/// <summary>
		/// Gets the title.
		/// </summary>
		string Title { get ; }

		/// <summary>
		/// Gets the title.
		/// </summary>
		string NavigationTitle { get ; }

		/// <summary>
		/// Gets weather the page should be visible in menus or not.
		/// </summary>
		bool IsHidden { get ; }

		/// <summary>
		/// Gets the permalink.
		/// </summary>
		string Permalink { get ; }

		/// <summary>
		/// Gets the template name.
		/// </summary>
		ComplexName TemplateName { get ; }

		/// <summary>
		/// Gets the created date.
		/// </summary>
		DateTime Created { get ; }

		/// <summary>
		/// Gets the updated date.
		/// </summary>
		DateTime Updated { get ; }

		/// <summary>
		/// Gets the published date.
		/// </summary>
		DateTime Published { get ; }

		/// <summary>
		/// Gets the last published date.
		/// </summary>
		DateTime LastPublished { get ; }

		/// <summary>
		/// Gets the user id that created the record.
		/// </summary>
		Guid CreatedBy { get ; }

		/// <summary>
		/// Gets the user id that created the record.
		/// </summary>
		Guid UpdatedBy { get ; }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the controller for the page.
		/// </summary>
		string Controller { get ; }

		/// <summary>
		/// Gets the redirect for the page.
		/// </summary>
		string Redirect { get ; }

		/// <summary>
		/// Gets weather the page is the site startpage.
		/// </summary>
		bool IsStartpage { get ; }
		#endregion
	}
}
