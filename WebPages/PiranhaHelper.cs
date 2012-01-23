using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.WebPages;
using System.Web.WebPages.Html;

using Piranha.Models;

namespace Piranha.WebPages
{
	/// <summary>
	/// View helper class.
	/// </summary>
	public class PiranhaHelper
	{
		#region Members
		private HtmlHelper Html ;
		private WebPage Parent ;
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="html"></param>
		public PiranhaHelper(WebPage parent, HtmlHelper html) {
			Parent = parent ;
			Html = html ;
		}

		/// <summary>
		/// Generates the tags appropriate for the html head.
		/// </summary>
		/// <returns>The head information</returns>
		public IHtmlString Head() {
			StringBuilder str = new StringBuilder() ;

			str.AppendLine("<meta name=\"generator\" content=\"Piranha\" />") ;
			if (Parent.Page.Current != null) {
				/**
				 * Basic meta tags
				 */
				if (!String.IsNullOrEmpty(((Page)Parent.Page.Current).Description))
					str.AppendLine("<meta name=\"description\" content=\"" + 
						((Page)Parent.Page.Current).Description + "\" />") ;
				if (!String.IsNullOrEmpty(((Page)Parent.Page.Current).Keywords))
					str.AppendLine("<meta name=\"keywords\" content=\"" + 
						((Page)Parent.Page.Current).Keywords + "\" />") ;

				/**
				 * Open graph meta tags
				 */
				str.AppendLine("<meta property=\"og:site_name\" content=\"" + 
					SysParam.GetByName("SITE_TITLE").Value + "\" />") ;
				str.AppendLine("<meta property=\"og:url\" content=\"" + 
					"http://" + Parent.Request.Url.DnsSafeHost +
					Parent.Request.RawUrl + "\" />") ;
				if (((Page)Parent.Page.Current).IsStartpage) {
					str.AppendLine("<meta property=\"og:type\" content=\"website\" />") ;
					str.AppendLine("<meta property=\"og:description\" content=\"" + 
						SysParam.GetByName("SITE_DESCRIPTION").Value + "\" />") ;
				} else {
					str.AppendLine("<meta property=\"og:type\" content=\"article\" />") ;
					if (!String.IsNullOrEmpty(((Page)Parent.Page.Current).Description)) {
						str.AppendLine("<meta property=\"og:description\" content=\"" + 
							((Page)Parent.Page.Current).Description + "\" />") ;
					}
				}
				str.AppendLine("<meta property=\"og:title\" content=\"" + 
					((Page)Parent.Page.Current).Title + "\" />") ;
			} else {
				/**
				 * Open graph meta tags
				 */
				str.AppendLine("<meta property=\"og:type\" content=\"article\" />") ;
			}

			return new HtmlString(str.ToString()) ;
		}

		/// <summary>
		/// Generates an image tag for the specified thumbnail.
		/// </summary>
		/// <param name="id">The content id</param>
		/// <param name="size">Optional size</param>
		/// <returns>The image html string</returns>
		public IHtmlString Thumbnail(Guid id, int size = 0) {
			Content cnt = Content.GetSingle(id) ;
			
			if (cnt != null)
				return new HtmlString(String.Format("<img src=\"{0}\" alt=\"{1}\" />", Parent.Href("~/thumb/" +
					id.ToString() + (size > 0 ? "/" + size.ToString() : "")), cnt.AlternateText)) ;
			return new HtmlString("") ; // TODO: Maybe a "missing image" image
		}

		/// <summary>
		/// Generates an image tag for the specified thumbnail.
		/// </summary>
		/// <param name="id">The content id</param>
		/// <param name="size">Optional size</param>
		/// <returns>The image html string</returns>
		public IHtmlString Thumbnail(string id, int size = 0) {
			return Thumbnail(new Guid(id), size) ;
		}

		/// <summary>
		/// Return the site structure as an ul/li list with the current page selected.
		/// </summary>
		/// <param name="StartLevel">The start level of the menu</param>
		/// <param name="StopLevel">The stop level of the menu</param>
		/// <returns>A html string</returns>
		public IHtmlString Menu(int StartLevel = 1, int StopLevel = Int32.MaxValue, 
			string RootNode = "") 
		{
			StringBuilder str = new StringBuilder() ;
			List<Sitemap> sm = null ;

			Page CurrentPage = (Page)Parent.Page.Current ;

			if (CurrentPage != null || StartLevel == 1) {
				if (CurrentPage == null)
					CurrentPage = new Page() ;
				if (RootNode != "") {
					Permalink pr = Permalink.GetSingle("permalink_name = @0", RootNode) ;
					if (pr != null) {
						Sitemap page = GetRootNode(Sitemap.GetStructure(true), pr.ParentId) ;
						if (page != null)
							sm = page.Pages ;
					}
				} else {
					sm = GetStartLevel(Sitemap.GetStructure(true), 
						CurrentPage.Id, StartLevel) ;
				}
				if (sm != null) {
					RenderUL(CurrentPage, sm, str, StopLevel) ;
				}
			}
			return new HtmlString(str.ToString()) ;
		}

		/// <summary>
		/// Creates the action input for a piranha post back.
		/// </summary>
		/// <param name="action">The form action</param>
		/// <returns>A html string</returns>
		public IHtmlString FormAction(string action) {
			return new HtmlString(String.Format("<input type=\"hidden\" name=\"piranha_form_action\" value=\"{0}\" />",
				action)) ;
		}

		#region Private methods
		/// <summary>
		/// Gets the current start level for the sitemap.
		/// </summary>
		/// <param name="sm">The sitemap</param>
		/// <param name="id">The id of the current page</param>
		/// <param name="start">The desired startlevel</param>
		/// <returns>The sitemap</returns>
		private List<Sitemap> GetStartLevel(List<Sitemap> sm, Guid id, int start) {
			if (sm == null || sm.Count == 0 || sm[0].Level == start)
				return sm ;
			foreach (Sitemap page in sm)
				if (ChildActive(page, id))
					return GetStartLevel(page.Pages, id, start) ;
			return null ;
		}

		/// <summary>
		/// Gets the page with the given id from the structure
		/// </summary>
		/// <param name="sm">The sitemap</param>
		/// <param name="id">The id</param>
		/// <returns>The record</returns>
		private Sitemap GetRootNode(List<Sitemap> sm, Guid id) {
			if (sm != null) {
				foreach (Sitemap page in sm) {
					if (page.Id == id)
						return page ;
					Sitemap subpage = GetRootNode(page.Pages, id) ;
					if (subpage != null)
						return subpage ;
				}
			}
			return null ;
		}

		/// <summary>
		/// Renders an UL list for the given sitemap elements
		/// </summary>
		/// <param name="curr">The current page</param>
		/// <param name="sm">The sitemap elements</param>
		/// <param name="str">The string builder</param>
		/// <param name="stoplevel">The desired stop level</param>
		private void RenderUL(Page curr, List<Sitemap> sm, StringBuilder str, int stoplevel) {
			if (sm != null && sm.CountVisible() > 0 && sm[0].Level <= stoplevel) {
				str.AppendLine("<ul class=\"menu\">") ;
				foreach (Sitemap page in sm)
					if (!page.IsHidden) RenderLI(curr, page, str, stoplevel) ;
				str.AppendLine("</ul>") ;
			}
		}

		/// <summary>
		/// Renders an LI element for the given sitemap node.
		/// </summary>
		/// <param name="curr">The current page</param>
		/// <param name="page">The sitemap element</param>
		/// <param name="str">The string builder</param>
		/// <param name="stoplevel">The desired stop level</param>
		private void RenderLI(Page curr, Sitemap page, StringBuilder str, int stoplevel) {
			if (page.GroupId == Guid.Empty || HttpContext.Current.User.IsMember(page.GroupId)) {
				str.AppendLine("<li" + (curr.Id == page.Id ? " class=\"active\"" : 
					(ChildActive(page, curr.Id) ? " class=\"active-child	\"" : "")) + ">") ;
				str.AppendLine(String.Format("<a href=\"{0}\">{1}</a>", GenerateUrl(page),
					!String.IsNullOrEmpty(page.NavigationTitle) ? page.NavigationTitle : page.Title)) ;
				if (page.Pages.Count > 0)
					RenderUL(curr, page.Pages, str, stoplevel) ;
				str.AppendLine("</li>") ;
			}
		}

		/// <summary>
		/// Checks if the given sitemap is active or has an active child
		/// </summary>
		/// <param name="page">The sitemap element</param>
		/// <param name="id">The page id to search for</param>
		/// <returns>If a child is selected</returns>
		private bool ChildActive(Sitemap page, Guid id) {
			if (page.Id == id)
				return true ;
			foreach (Sitemap sr in page.Pages) {
				if (ChildActive(sr, id))
					return true ;
			}
			return false ;
		}

		/// <summary>
		/// Generate the correct URL for the given sitemap node
		/// </summary>
		/// <param name="helper">The url helper</param>
		/// <param name="page">The sitemap</param>
		/// <returns>An action url</returns>
		private string GenerateUrl(Sitemap page) {
			if (page != null) {
				if (!String.IsNullOrEmpty(page.Redirect)) {
					if (page.Redirect.Contains("://"))
						return page.Redirect ;
					Sitemap sr = Sitemap.GetSingle("permalink_name = @0", page.Redirect) ;
					return GenerateUrl(sr) ;
				} 
				return Parent.Href("~/hem/" + page.Permalink.ToLower()) ;
			}
			return "" ;
		}
		#endregion  
	}
}
