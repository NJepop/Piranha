using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Active record for a post template.
	/// </summary>
	[PrimaryKey(Column="posttemplate_id")]
	public class PostTemplate : PiranhaRecord<PostTemplate>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="posttemplate_id")]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets the name.
		/// </summary>
		[Column(Name="posttemplate_name", OnLoad="OnNameLoad", OnSave="OnNameSave")]
		public ComplexName Name { get ; set ; }

		/// <summary>
		/// Gets/sets the description.
		/// </summary>
		[Column(Name="posttemplate_description")]
		public string Description { get ; set ; }

		/// <summary>
		/// Gets/sets the optional view name for the template.
		/// </summary>
		[Column(Name="posttemplate_view")]
		public string View { get ; set ; }

		/// <summary>
		/// Gets/sets the optional controller for the template.
		/// </summary>
		[Column(Name="posttemplate_controller")]
		public string Controller { get ; set ; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name="posttemplate_created")]
		public override DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name="posttemplate_updated")]
		public override DateTime Updated { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="posttemplate_created_by")]
		public override Guid CreatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="posttemplate_updated_by")]
		public override Guid UpdatedBy { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public PostTemplate() : base() {
			Name = new ComplexName() ;
		}
	}
}
