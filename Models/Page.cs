using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Active record for a page.
	/// </summary>
	[PrimaryKey(Column="page_id")]
	[Join(TableName="pagetemplate", ForeignKey="page_template_id", PrimaryKey="pagetemplate_id")]
	[Join(TableName="permalink", ForeignKey="page_id", PrimaryKey="permalink_parent_id")]
	public class Page : PiranhaRecord<Page>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="page_id")]
		[Required()]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets the template id.
		/// </summary>
		[Column(Name="page_template_id")]
		[Required()]
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
		[Required(), StringLength(128), Display(Name="Titel")]
		public string Title { get ; set ; }

		/// <summary>
		/// Gets/sets the optional navigation title.
		/// </summary>
		[Column(Name="page_navigation_title")]
		[StringLength(128), Display(Name="Navigeringstitel")]
		public string NavigationTitle { get ; set ; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		//[Column(Name="page_permalink")]
		[Column(Name="permalink_name", ReadOnly=true, Table="permalink")]
		[Required(), StringLength(128), Display(Name="Permalänk")]
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
		[StringLength(255), Display(Name="Nyckelord")]
		public string Keywords { get ; set ; }

		/// <summary>
		/// Gets/sets the meta description.
		/// </summary>
		[Column(Name="page_description")]
		[StringLength(255), Display(Name="Beskrivning")]
		public string Description { get ; set ; }

		/// <summary>
		/// Gets/sets the custom controller.
		/// </summary>
		[Column(Name="page_controller")]
		[StringLength(128), Display(Name="Kontroller")]
		public string PageController { get ; set ; }

		/// <summary>
		/// Gets/sets the custom view.
		/// </summary>
		[Column(Name="page_view")]
		[StringLength(128), Display(Name="Vy")]
		public string PageView { get ; set ; }

		/// <summary>
		/// Gets/sets the custom redirect.
		/// </summary>
		[Column(Name="page_redirect")]
		[StringLength(128), Display(Name="Omdirigering")]
		public string PageRedirect { get ; set ; }

		/// <summary>
		/// Gets/sets the custom view.
		/// </summary>
		[Column(Name="pagetemplate_view", ReadOnly=true, Table="pagetemplate")]
		private string TemplateView { get ; set ; }

		/// <summary>
		/// Gets/sets the custom controller.
		/// </summary>
		[Column(Name="pagetemplate_controller", ReadOnly=true, Table="pagetemplate")]
		private string TemplateController { get ; set ; }

		/// <summary>
		/// Gets/sets the custom controller.
		/// </summary>
		[Column(Name="pagetemplate_redirect", ReadOnly=true, Table="pagetemplate")]
		private string TemplateRedirect { get ; set ; }

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
		[Required()]
		public override DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name="page_updated")]
		[Required()]
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
		[Required()]
		public override Guid CreatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="page_updated_by")]
		[Required()]
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
		/// Gets weather the page is published or not.
		/// </summary>
		public bool IsPublished {
			get { return Published != DateTime.MinValue && Published < DateTime.Now ; }
		}
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Page() : base() {
			this.Seqno   = 1;
		}

		/// <summary>
		/// Gets the site startpage
		/// </summary>
		/// <param name="lm">Optional load model</param>
		/// <returns>The startpage</returns>
		public static Page GetStartpage() {
			return Page.GetSingle("page_parent_id IS NULL and page_seqno = 1") ;
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
	}
}
