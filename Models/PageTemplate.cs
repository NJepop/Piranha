using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Active record for a page template.
	/// </summary>
	[PrimaryKey(Column="pagetemplate_id")]
	public class PageTemplate : PiranhaRecord<PageTemplate>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="pagetemplate_id")]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets the name.
		/// </summary>
		[Column(Name="pagetemplate_name", OnLoad="OnNameLoad", OnSave="OnNameSave")]
		public ComplexName Name { get ; set ; }

		/// <summary>
		/// Gets/sets the template's description.
		/// </summary>
		[Column(Name="pagetemplate_description")]
		[Display(Name="Beskrivning")]
		public string Description { get ; set ; }

		/// <summary>
		/// Gets/sets the template's html preview.
		/// </summary>
		[Column(Name="pagetemplate_preview")]
		[Display(Name="Html-tumnagel")]
		public HtmlString Preview { get ; set ; }

		/// <summary>
		/// Gets/sets the regions associated with this page.
		/// </summary>
		[Column(Name="pagetemplate_page_regions", OnLoad="OnListLoad", OnSave="OnListSave")]
		public List<string> PageRegions { get ; set ; }

		/// <summary>
		/// Gets/sets the associated properties.
		/// </summary>
		[Column(Name="pagetemplate_properties", OnLoad="OnListLoad", OnSave="OnListSave")]
		public List<string> Properties { get ; set ; }

		/// <summary>
		/// Gets/sets the optional view name for the template.
		/// </summary>
		[Column(Name="pagetemplate_view")]
		[Display(Name="Vy")]
		public string View { get ; set ; }

		/// <summary>
		/// Gets/sets wether the view can be overridden by the implementing page.
		/// </summary>
		[Column(Name="pagetemplate_view_show")]
		public bool ShowView { get ; set ; }

		/// <summary>
		/// Gets/sets the optional view name for the template.
		/// </summary>
		[Column(Name="pagetemplate_controller")]
		[Display(Name="Kontroller")]
		public string Controller { get ; set ; }

		/// <summary>
		/// Gets/sets wether the controller can be overridden by the implementing page.
		/// </summary>
		[Column(Name="pagetemplate_controller_show")]
		public bool ShowController { get ; set ; }

		/// <summary>
		/// Gets/sets the optional permalink of a page this sould redirect to.
		/// </summary>
		[Column(Name="pagetemplate_redirect")]
		[Display(Name="Omdirigering")]
		public string Redirect { get ; set ; }

		/// <summary>
		/// Gets/sets if the redirect can be overriden by the implementing page.
		/// </summary>
		[Column(Name="pagetemplate_redirect_show")]
		public bool ShowRedirect { get ; set ; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name="pagetemplate_created")]
		public override DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name="pagetemplate_updated")]
		public override DateTime Updated { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="pagetemplate_created_by")]
		public override Guid CreatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="pagetemplate_updated_by")]
		public override Guid UpdatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the number of pages of the current template type.
		/// </summary>
		[Column(Name="(SELECT COUNT(*) FROM page WHERE page.page_template_id = pagetemplate_id)", ReadOnly=true)]
		public int Count { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public PageTemplate() : base() {
			PageRegions = new List<string>() ;
			Properties = new List<string>() ;
			Name = new ComplexName() ;
		}
	}
}
