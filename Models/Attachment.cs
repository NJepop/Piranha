using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piranha.Data;

namespace Piranha.Models
{
	[PrimaryKey(Column="attachment_id")]
	public class Attachment : PiranhaRecord<Attachment>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="attachment_id")]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets the content id.
		/// </summary>
		[Column(Name="attachment_content_id")]
		public Guid ContentId { get ; set ; }

		/// <summary>
		/// Gets/sets the parent id.
		/// </summary>
		[Column(Name="attachment_parent_id")]
		public Guid ParentId { get ; set ; }

		/// <summary>
		/// Gets/sets weather the attachment is primary.
		/// </summary>
		[Column(Name="attachment_primary")]
		public bool IsPrimary { get ; set ; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name="attachment_created")]
		public override DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name="attachment_updated")]
		public override DateTime Updated { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="attachment_created_by")]
		public override Guid CreatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="attachment_updated_by")]
		public override Guid UpdatedBy { get ; set ; }
		#endregion
	}
}
