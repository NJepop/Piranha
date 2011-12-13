using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Content class
	/// </summary>
	[PrimaryKey(Column="content_id")]
	public class Content : PiranhaRecord<Content>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="content_id")]
		[Required()]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets the filename.
		/// </summary>
		[Column(Name="content_filename")]
		[Display(Name="Filnamn")]
		public string Filename { get ; set ; }

		/// <summary>
		/// Gets/sets the type
		/// </summary>
		[Column(Name="content_type")]
		[Display(Name="Typ")]
		public string Type { get ; set ; }

		/// <summary>
		/// Gets/sets the alternate text.
		/// </summary>
		[Column(Name="content_alt")]
		[Display(Name="Alt. text")]
		public string AlternateText { get ; set ; }

		/// <summary>
		/// Gets/sets the decription.
		/// </summary>
		[Column(Name="content_description")]
		[Display(Name="Beskrivning")]
		public string Description { get ; set ; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name="content_created")]
		[Required()]
		public override DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name="content_updated")]
		[Required()]
		public override DateTime Updated { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="content_created_by")]
		[Required()]
		public override Guid CreatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="content_updated_by")]
		[Required()]
		public override Guid UpdatedBy { get ; set ; }
		#endregion

		public static List<Content> GetByPageId(Guid id) {
			return Content.Get("content_id IN " +
				"(SELECT attachment_content_id FROM attachment WHERE attachment_parent_id = @0)", id) ;
		}
	}
}
