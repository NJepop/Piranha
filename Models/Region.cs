using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Active record for a page region.
	/// </summary>
	[PrimaryKey(Column="region_id")]
	public class Region : PiranhaRecord<Region>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="region_id")]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets the parent id.
		/// </summary>
		[Column(Name="region_page_id")]
		public Guid PageId { get ; set ; }

		/// <summary>
		/// Gets/sets the name.
		/// </summary>
		[Column(Name="region_name")]
		public string Name { get ; set ; }

		/// <summary>
		/// Gets/sets the body content.
		/// </summary>
		[Column(Name="region_body"), AllowHtml()]
		public HtmlString Body { get ; set ; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name="region_created")]
		public override DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name="region_updated")]
		public override DateTime Updated { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="region_created_by")]
		public override Guid CreatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="region_updated_by")]
		public override Guid UpdatedBy { get ; set ; }
		#endregion
	}
}
