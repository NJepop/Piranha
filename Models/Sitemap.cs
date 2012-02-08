using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Active record for a sitemap.
	/// </summary>
	[Table(Name="page")]
	[Join(TableName="pagetemplate", ForeignKey="page_template_id", PrimaryKey="pagetemplate_id")]
	[Join(TableName="permalink", ForeignKey="page_id", PrimaryKey="permalink_parent_id")]
	public class Sitemap : PiranhaRecord<Sitemap>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="page_id")]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets the group needed to view the page.
		/// </summary>
		[Column(Name="page_group_id")]
		public Guid GroupId { get ; set ; }

		/// <summary>
		/// Gets/sets the parent id.
		/// </summary>
		[Column(Name="page_parent_id")]
		public Guid ParentId { get ; set ; }

		/// <summary>
		/// Gets/sets the seqno specifying the structural position.
		/// </summary>
		[Column(Name="page_seqno")]
		public int Seqno { get ; set ; }

		/// <summary>
		/// Gets/sets the title.
		/// </summary>
		[Column(Name="page_title")]
		public string Title { get ; set ; }

		/// <summary>
		/// Gets/sets the title.
		/// </summary>
		[Column(Name="page_navigation_title")]
		public string NavigationTitle { get ; set ; }


		/// <summary>
		/// Gets/sets weather the page should be visible in menus or not.
		/// </summary>
		[Column(Name="page_is_hidden")]
		public bool IsHidden { get ; set ; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		//[Column(Name="page_permalink")]
		[Column(Name="permalink_name", ReadOnly=true)]
		public string Permalink { get ; set ; }

		/// <summary>
		/// Gets/sets the template name.
		/// </summary>
		[Column(Name="pagetemplate_name", ReadOnly=true, OnLoad="OnNameLoad", OnSave="OnNameSave")]
		public ComplexName TemplateName { get ; set ; }

		/// <summary>
		/// Gets/sets the page controller.
		/// </summary>
		[Column(Name="page_controller")]
		private string PageController { get ; set ; }

		/// <summary>
		/// Gets/sets the template controller.
		/// </summary>
		[Column(Name="pagetemplate_controller", ReadOnly=true)]
		private string TemplateController { get ; set ; }

		/// <summary>
		/// Gets/sets the custom view.
		/// </summary>
		[Column(Name="page_view")]
		public string PageView { get ; set ; }

		/// <summary>
		/// Gets/sets the custom view.
		/// </summary>
		[Column(Name="pagetemplate_view", ReadOnly=true)]
		private string TemplateView { get ; set ; }

		/// <summary>
		/// Gets/sets the custom redirect.
		/// </summary>
		[Column(Name="page_redirect")]
		public string PageRedirect { get ; set ; }

		/// <summary>
		/// Gets/sets the custom controller.
		/// </summary>
		[Column(Name="pagetemplate_redirect", ReadOnly=true)]
		private string TemplateRedirect { get ; set ; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name="page_created")]
		public override DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name="page_updated")]
		public override DateTime Updated { get ; set ; }

		/// <summary>
		/// Gets/sets the published date.
		/// </summary>
		[Column(Name="page_published")]
		public DateTime Published { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="page_created_by")]
		public override Guid CreatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="page_updated_by")]
		public override Guid UpdatedBy { get ; set ; }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the controller for the page.
		/// </summary>
		public string Controller { 
			get { return !String.IsNullOrEmpty(PageController) ? PageController : TemplateController ; }
		}

		/// <summary>
		/// Gets the view for the page.
		/// </summary>
		public string View { 
			get { return !String.IsNullOrEmpty(PageView) ? PageView : TemplateView ; }
		}

		/// <summary>
		/// Gets the redirect for the page.
		/// </summary>
		public string Redirect {
			get { return !String.IsNullOrEmpty(PageRedirect) ? PageRedirect : TemplateRedirect ; }
		}

		/// <summary>
		/// Gets/sets the page level.
		/// </summary>
		public int Level { get ; private set ; }

		/// <summary>
		/// Gets weather the page is published or not.
		/// </summary>
		public bool IsPublished {
			get { return Published != DateTime.MinValue && Published < DateTime.Now ; }
		}

		/// <summary>
		/// Gets weather the page is the site startpage.
		/// </summary>
		public bool IsStartpage {
			get { return ParentId == Guid.Empty && Seqno == 1 ; }
		}

		/// <summary>
		/// Gets/sets the possible subpages.
		/// </summary>
		public List<Sitemap> Pages { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor, creates a new sitemap record.
		/// </summary>
		public Sitemap() {
			Pages = new List<Sitemap>() ;
		}

		/// <summary>
		/// Gets the sorted sitemap structure.
		/// </summary>
		/// <param name="published">Weather only published pages should be included.</param>
		/// <returns>The site structure</returns>
		public static List<Sitemap> GetStructure(bool published = true) {
			if (published) {
				// Return the cached public sitemap if it exists.
				if (published && HttpContext.Current.Cache[typeof(Sitemap).Name] != null)
					return (List<Sitemap>)HttpContext.Current.Cache[typeof(Sitemap).Name] ;

				// Get the sitemap from the database
				List<Sitemap> pages = Get("page_draft = 0", new Params() { OrderBy = "page_parent_id, page_seqno" }) ;
				pages = Sort(pages, Guid.Empty) ;
			
				// If this is the public sitemap, cache it
				if (published)
					HttpContext.Current.Cache[typeof(Sitemap).Name] = pages ;
				return pages ;
			} else {
				// Get the sitemap from the database
				List<Sitemap> pages = Get("page_draft = 1", new Params() { OrderBy = "page_parent_id, page_seqno" }) ;
				pages = Sort(pages, Guid.Empty) ;			
				return pages ;
			}
		}

		/// <summary>
		/// Invalidate the cache.
		/// </summary>
		public static void InvalidateCache() {
			HttpContext.Current.Cache.Remove(typeof(Sitemap).Name) ;
		}

		#region Private methods
		/// <summary>
		/// Sorts the page structure recursive.
		/// </summary>
		/// <param name="pages">The pages</param>
		/// <param name="parentid">The parent id</param>
		/// <returns>The site structure</returns>
		private static List<Sitemap> Sort(List<Sitemap> pages, Guid parentid, int level = 1) {
			List<Sitemap> ret = new List<Sitemap>() ;

			foreach (Sitemap page in pages) {
				if (page.ParentId == parentid) {
					page.Level = level ;
					page.Pages = Sort(pages, page.Id, level + 1) ;
					ret.Add(page) ;
				}
			}
			return ret ;
		}
		#endregion
	}

	/// <summary>
	/// Extension methods
	/// </summary>
	public static class SitemapHelpers {
		/// <summary>
		/// Flattens the recursive strucutre and returns it as a list.
		/// </summary>
		/// <returns>The sitemap</returns>
		public static List<Sitemap> Flatten(this List<Sitemap> pages) {
			List<Sitemap> ret = new List<Sitemap>() ;

			foreach (Sitemap page in pages) {
				ret.Add(page) ;
				if (page.Pages.Count > 0)
					ret.AddRange(Flatten(page.Pages)) ;
			}
			return ret ;
		}

		/// <summary>
		/// Counts the number of visible pages in the given list.
		/// </summary>
		/// <param name="pages">The pages</param>
		/// <returns>The number of visible pages</returns>
		public static int CountVisible(this List<Sitemap> pages) {
			int count = 0 ;
			foreach (Sitemap page in pages)
				if (!page.IsHidden) count++ ;
			return count ;
		}
	}
}
