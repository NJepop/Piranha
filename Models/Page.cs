using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Data;

namespace Piranha.Models
{
	#region Client API
	/// <summary>
	/// This is the interface through which the page is accessed from the client API.
	/// </summary>
	public interface IPage {
		Guid Id { get ; }
		Guid GroupId { get ; }
		string Title { get ; }
		string NavigationTitle { get ; }
		string Permalink { get ; }
		string Keywords { get ; }
		string Description { get ; }
		ComplexName TemplateName { get ; }
		DateTime Created { get ; }
		DateTime Updated { get ; }
		DateTime Published { get ; }
	}
	#endregion

	/// <summary>
	/// Active record for a page.
	/// </summary>
	[PrimaryKey(Column="page_id")]
	[Join(TableName="pagetemplate", ForeignKey="page_template_id", PrimaryKey="pagetemplate_id")]
	[Join(TableName="permalink", ForeignKey="page_id", PrimaryKey="permalink_parent_id")]
	public class Page : PiranhaRecord<Page>, IPage, ICacheRecord<Page>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="page_id")]
		public override Guid Id { get ; set ; }

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
		//[Column(Name="page_permalink")]
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
		public string TemplateController { get ; set ; }

		/// <summary>
		/// Gets/sets the custom controller.
		/// </summary>
		[Column(Name="pagetemplate_redirect", ReadOnly=true, Table="pagetemplate")]
		public string TemplateRedirect { get ; set ; }

		/// <summary>
		/// Gets/sets the custom manager view.
		/// </summary>
		[Column(Name="pagetemplate_manager_view", ReadOnly=true, Table="pagetemplate")]
		public string ManagerView { get ; set ; }

		/// <summary>
		/// Gets/sets the custom manager controller.
		/// </summary>
		[Column(Name="pagetemplate_manager_controller", ReadOnly=true, Table="pagetemplate")]
		public string ManagerController { get ; set ; }

		/// <summary>
		/// Gets/sets the template name.
		/// </summary>
		[Column(Name="pagetemplate_name", ReadOnly=true, Table="pagetemplate", OnLoad="OnNameLoad", OnSave="OnNameSave")]
		public ComplexName TemplateName { get ; set ; }

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
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Page() : base() {
			this.Seqno   = 1;
		}

		/// <summary>
		/// Gets a single page.
		/// </summary>
		/// <param name="id">The page id</param>
		/// <returns>The page</returns>
		public static Page GetSingle(Guid id) {
			if (!Cache.ContainsKey(id))
				Cache[id] = Page.GetSingle((object)id) ;
			return Cache[id] ;
		}

		/// <summary>
		/// Gets the site startpage
		/// </summary>
		/// <param name="lm">Optional load model</param>
		/// <returns>The startpage</returns>
		public static Page GetStartpage() {
			if (!Cache.ContainsKey(Guid.Empty))
				Cache[Guid.Empty] = Page.GetSingle("page_parent_id IS NULL and page_seqno = 1") ;
			return Cache[Guid.Empty] ;
		}

		/// <summary>
		/// Gets the page specified by the given permalink.
		/// </summary>
		/// <param name="permalink">The permalink</param>
		/// <returns>The page</returns>
		public static Page GetByPermalink(string permalink) {
			return Page.GetSingle("permalink_name = @0", permalink) ;
		}

		/// <summary>
		/// Saves the current record to the database.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Wether the operation was successful</returns>
		public override bool Save(IDbTransaction tx = null) {
			// Generate permalink
			if (IsNew && String.IsNullOrEmpty(Permalink))
				Permalink = Title.ToLower().Replace(" ", "-").Replace("å", "a").Replace("ä", "a").Replace("ö", "o") ;
			if (!String.IsNullOrEmpty(PageController))
				Permalink = PageController.ToLower() ;

			// Move seqno & save, we need a transaction for this
			IDbTransaction t = tx != null ? tx : Database.OpenConnection().BeginTransaction() ;

			try {
				if (IsNew) {
					MoveSeqno(ParentId, Seqno, true, t) ;
				} else {
					Page old = GetSingle(Id) ;
					if (old.ParentId != ParentId || old.Seqno != Seqno) {
						MoveSeqno(old.ParentId, old.Seqno + 1, false, t) ;
						MoveSeqno(ParentId, Seqno, true, t) ;
					}
				}
				if (base.Save(t)) {
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
			//DataHandler<Page> dh = new DataHandler<Page>() ;

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
			if (record.IsStartpage && Cache.ContainsKey(Guid.Empty))
				Cache.Remove(Guid.Empty) ;

			// Invalidate public sitemap
			Sitemap.InvalidateCache() ;
		}
	}
}
