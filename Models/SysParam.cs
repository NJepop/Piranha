using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Active record for the system parameters.
	/// </summary>
	[PrimaryKey(Column="sysparam_id")] 
	public class SysParam : PiranhaRecord<SysParam>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="sysparam_id"), Required()]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets the param name.
		/// </summary>
		[Column(Name="sysparam_name"), Display(Name="Namn")]
		[Required(ErrorMessage="Du måste ange ett namn.")]
		[StringLength(64, ErrorMessage="Namnet får inte vara längre än 64 tecken.")]
		public string Name { get ; set ; }

		/// <summary>
		/// Gets/sets the param value.
		/// </summary>
		[Column(Name="sysparam_value"), Display(Name="Värde")]
		[StringLength(128, ErrorMessage="Värdet får inte vara längre än 128 tecken.")]
		public string Value { get ; set ; }

		/// <summary>
		/// Gets/sets the param description.
		/// </summary>
		[Column(Name="sysparam_description"), Display(Name="Beskrivning")]
		[StringLength(255, ErrorMessage="Beskrivningen får inte vara längre än 255 tecken.")]
		public string Description { get ; set ; }

		/// <summary>
		/// Gets/sets weather the param is locked or not. This field can not be set through
		/// the admin interface.
		/// </summary>
		[Column(Name="sysparam_locked")]
		public bool IsLocked { get ; set ; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name="sysparam_created")]
		public override DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the update date.
		/// </summary>
		[Column(Name="sysparam_updated")]
		public override DateTime Updated { get ; set ; }

		/// <summary>
		/// Gets/sets the created by id
		/// </summary>
		[Column(Name="sysparam_created_by")]
		public override Guid CreatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the updated by id.
		/// </summary>
		[Column(Name="sysparam_updated_by")]
		public override Guid UpdatedBy { get ; set ; }
		#endregion

		#region Static accessors
		/// <summary>
		/// Gets the param with the given name.
		/// </summary>
		/// <param name="name">The param name</param>
		/// <returns>The param</returns>
		public static SysParam GetByName(string name) {
			return GetSingle("sysparam_name = @0", name) ;
		}
		#endregion

		/// <summary>
		/// Saves the current record.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Weather the action was successful</returns>
		public override bool Save(System.Data.IDbTransaction tx = null) {
			if (Name != null)
				Name = Name.ToUpper() ;
			return base.Save(tx);
		}
	}
}
