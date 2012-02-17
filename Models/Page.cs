using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Data;
using Piranha.WebPages;

namespace Piranha.Models
{
	/// <summary>
	/// Active record for a page.
	/// </summary>
	[PrimaryKey(Column="page_id,page_draft")]
	[Join(TableName="pagetemplate", ForeignKey="page_template_id", PrimaryKey="pagetemplate_id")]
	[Join(TableName="permalink", ForeignKey="page_id", PrimaryKey="permalink_parent_id")]
	public class Page : DraftRecord<Page>, IPage, ISitemap, ICacheRecord<Page>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="page_id")]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets weather this is a draft.
		/// </summary>
		[Column(Name="page_draft")]
		public override bool IsDraft { get ; set ; }

		/// <summary>
		/// Gets/sets the template id.
		/// </summary>
		[Column(Name="page_template_id")]
		public Guid TemplateId { get ; set ; }

		/// <summary>
		/// Gets/sets the group needed to view the page.
		/// </summary>
		[Column(Name="page_group_id")]
		[Display(Name="Behörighet")]
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
		[Required(ErrorMessage="Du måste ange en title."), Display(Name="Titel")]
		[StringLength(128, ErrorMessage="Titeln får max innehålla 128 tecken.")]
		public string Title { get ; set ; }

		/// <summary>
		/// Gets/sets the optional navigation title.
		/// </summary>
		[Column(Name="page_navigation_title")]
		[StringLength(128, ErrorMessage="Titeln får max innehålla 128 tecken."), Display(Name="Navigeringstitel")]
		public string NavigationTitle { get ; set ; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		[Column(Name="permalink_name", ReadOnly=true, Table="permalink")]
		[Display(Name="Permalänk")]
		public string Permalink { get ; set ; }

		/// <summary>
		/// Gets/sets weather the page should be visible in menus or not.
		/// </summary>
		[Column(Name="page_is_hidden")]
		[Display(Name="Dölj sidan")]
		public bool IsHidden { get ; set ; }

		/// <summary>
		/// Gets/sets the meta keywords.
		/// </summary>
		[Column(Name="page_keywords")]
		[StringLength(255, ErrorMessage="Nyckelorden får max innehålla 255 tecken."), Display(Name="Nyckelord")]
		public string Keywords { get ; set ; }

		/// <summary>
		/// Gets/sets the meta description.
		/// </summary>
		[Column(Name="page_description")]
		[StringLength(255, ErrorMessage="Beskrivningen får max innehålla 255 tecken."), Display(Name="Beskrivning")]
		public string Description { get ; set ; }

		/// <summary>
		/// Gets/sets the custom controller.
		/// </summary>
		[Column(Name="page_controller")]
		[StringLength(128, ErrorMessage="Mallens namn får max innehålla 128 tecken."), Display(Name="Webbmall")]
		public string PageController { get ; set ; }

		/// <summary>
		/// Gets/sets the custom redirect.
		/// </summary>
		[Column(Name="page_redirect")]
		[StringLength(128, ErrorMessage="Omdirigeringen får max innehålla 128 tecken."), Display(Name="Omdirigering")]
		public string PageRedirect { get ; set ; }

		/// <summary>
		/// Gets/sets the custom controller.
		/// </summary>
		[Column(Name="pagetemplate_controller", ReadOnly=true, Table="pagetemplate")]
		public string TemplateController { get ; private set ; }

		/// <summary>
		/// Gets/sets the custom controller.
		/// </summary>
		[Column(Name="pagetemplate_redirect", ReadOnly=true, Table="pagetemplate")]
		public string TemplateRedirect { get ; private set ; }

		/// <summary>
		/// Gets/sets the template name.
		/// </summary>
		[Column(Name="pagetemplate_name", ReadOnly=true, Table="pagetemplate", OnLoad="OnNameLoad", OnSave="OnNameSave")]
		public ComplexName TemplateName { get ; private set ; }

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
		public override DateTime Published { get ; set ; }

		/// <summary>
		/// Gets/sets the last published date.
		/// </summary>
		[Column(Name="page_last_published")]
		public override DateTime LastPublished { get ; set ; }

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
		/// Gets the redirect for the page.
		/// </summary>
		public string Redirect {
			get { return !String.IsNullOrEmpty(PageRedirect) ? PageRedirect : TemplateRedirect ; }
		}

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
		/// Gets the page cache object.
		/// </summary>
		private static Dictionary<Guid, Page> Cache {
			get {
				if (HttpContext.Current.Cache[typeof(Page).Name] == null)
					HttpContext.Current.Cache[typeof(Page).Name] = new Dictionary<Guid, Page>() ;
				return (Dictionary<Guid, Page>)HttpContext.Current.Cache[typeof(Page).Name] ;
			}
		}

		/// <summary>
		/// Gets the page cache object by permalink.
		/// </summary>
		private static Dictionary<string, Page> PermalinkCache {
			get {
				if (HttpContext.Current.Cache[typeof(Page).Name + "_Permalink"] == null)
					HttpContext.Current.Cache[typeof(Page).Name + "_Permalink"] = new Dictionary<string, Page>() ;
				return (Dictionary<string, Page>)HttpContext.Current.Cache[typeof(Page).Name + "_Permalink"] ;
			}
		}
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Page() : base() {
			IsDraft = true ;
			Seqno   = 1 ;
		}

		#region Static accessors
		/// <summary>
		/// Gets a single page.
		/// </summary>
		/// <param name="id">The page id</param>
		/// <returns>The page</returns>
		public static Page GetSingle(Guid id) {
			if (!Cache.ContainsKey(id)) {
				Page p = Page.GetSingle("page_id = @0 AND page_draft = 0", id) ;
				
				if (p != null) {
					Cache[p.Id] = p ;
					PermalinkCache[p.Permalink] = p ;
				} else return null ;
			}
			return Cache[id] ;
		}

		/// <summary>
		/// Gets a single page.
		/// </summary>
		/// <param name="id">The page id</param>
		/// <param name="draft">Page status</param>
		/// <returns>The page</returns>
		public static Page GetSingle(Guid id, bool draft) {
			if (!draft)
				return GetSingle(id) ;
			return GetSingle("page_id = @0 AND page_draft = @1", id, draft) ;
		}

		/// <summary>
		/// Gets the site startpage
		/// </summary>
		/// <param name="draft">Weather to get the current draft</param>
		/// <returns>The startpage</returns>
		public static Page GetStartpage(bool draft = false) {
			if (!Cache.ContainsKey(Guid.Empty))
				Cache[Guid.Empty] = Page.GetSingle("page_parent_id IS NULL and page_seqno = 1 AND page_draft = @0", draft) ;
			return Cache[Guid.Empty] ;
		}

		/// <summary>
		/// Gets the page specified by the given permalink.
		/// </summary>
		/// <param name="permalink">The permalink</param>
		/// <param name="draft">Weather to get the current draft</param>
		/// <returns>The page</returns>
		public static Page GetByPermalink(string permalink, bool draft = false) {
			if (!draft) {
				if (!PermalinkCache.ContainsKey(permalink.ToLower())) {
					Page p = Page.GetSingle("permalink_name = @0 AND page_draft = @1", permalink, draft) ;

					Cache[p.Id] = p ;
					PermalinkCache[p.Permalink] = p ;
				}
				return PermalinkCache[permalink.ToLower()] ;
			}
			return Page.GetSingle("permalink_name = @0 AND page_draft = @1", permalink, draft) ;
		}
		#endregion

		/// <summary>
		/// Saves the current record to the database.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Wether the operation was successful</returns>
		public override bool Save(IDbTransaction tx = null) {
			// Move seqno & save, we need a transaction for this
			IDbTransaction t = tx != null ? tx : Database.OpenConnection().BeginTransaction() ;

			// Generate permalink
			if (IsNew && String.IsNullOrEmpty(Permalink))
				Permalink = Title.ToLower().Replace(" ", "-").Replace("å", "a").Replace("ä", "a").Replace("ö", "o") ;
			if (!String.IsNullOrEmpty(PageController))
				Permalink = PageController.ToLower() ;

			// We only move pages around as drafts. When we publish we
			// simply change states.
			if (IsDraft) {
				if (IsNew) {
					MoveSeqno(ParentId, Seqno, true, t) ;
				} else {
					Page old = GetSingle(Id, true) ;
					if (old.ParentId != ParentId || old.Seqno != Seqno) {
						MoveSeqno(old.ParentId, old.Seqno + 1, false, t) ;
						MoveSeqno(ParentId, Seqno, true, t) ;
					}
				}
			}
			return base.Save(tx) ;
		}

		/// <summary>
		/// Deletes the current page.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Weather the operation was successful</returns>
		public override bool Delete(IDbTransaction tx = null) {
			// Move seqno & delete. We need a transaction for this
			IDbTransaction t = tx != null ? tx : Database.OpenConnection().BeginTransaction() ;

			try {
				MoveSeqno(ParentId, Seqno + 1, false, t) ;
				if (base.Delete(t)) {
					if (tx == null) 
						t.Commit() ;
					return true ;
				} else {
					if (tx == null) 
						t.Rollback() ;
					return false ;
				}
			} catch {
				if (tx == null) t.Rollback() ;
				throw ;
			}
		}
	
		/// <summary>
		/// Moves the seqno around so that a page can be inserted into the structure.
		/// </summary>
		/// <param name="parentid">The parent id</param>
		/// <param name="seqno">The seqno</param>
		/// <param name="inc">Weather to increase or decrease</param>
		internal static void MoveSeqno(Guid parentid, int seqno, bool inc, IDbTransaction tx = null) {
			if (parentid != Guid.Empty)
				Execute("UPDATE page SET page_seqno = page_seqno " + (inc ? "+ 1" : "- 1") +
					" WHERE page_parent_id = @0 AND page_seqno >= @1", tx, parentid, seqno) ;
			else Execute("UPDATE page SET page_seqno = page_seqno " + (inc ? "+ 1" : "- 1") +
				" WHERE page_parent_id IS NULL AND page_seqno >= @0", tx, seqno) ;
		}

		/// <summary>
		/// Invalidates the current record from the cache.
		/// </summary>
		/// <param name="record">The record</param>
		public void InvalidateRecord(Page record) {
			if (Cache.ContainsKey(record.Id))
				Cache.Remove(record.Id) ;
			// If we click save & publish right away the permalink is not created yet.
			if (record.Permalink != null && PermalinkCache.ContainsKey(record.Permalink))
				PermalinkCache.Remove(record.Permalink) ;
			if (record.IsStartpage && Cache.ContainsKey(Guid.Empty))
				Cache.Remove(Guid.Empty) ;

			// Invalidate public sitemap
			if (!record.IsDraft)
				Sitemap.InvalidateCache() ;
		}
	}
}
