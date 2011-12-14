using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Active record for the access control table.
	/// </summary>
	[PrimaryKey(Column="sysaccess_id")] 
	[Join(TableName="sysgroup", ForeignKey="sysaccess_group_id", PrimaryKey="sysgroup_id")]
	public class SysAccess : PiranhaRecord<SysAccess>
	{
		#region Fields
		[Column(Name="sysaccess_id")]
		[Required()]
		public override Guid Id { get ; set ; }

		[Column(Name="sysaccess_group_id")]
		[Required(ErrorMessage="Du måste välja en grupp.")]
		[Display(Name="Grupp")]
		public Guid GroupId { get ; set ; }

		[Column(Name="sysgroup_name", ReadOnly=true)]
		public string GroupName { get ; set ; }

		[Column(Name="sysaccess_function")]
		[Required(ErrorMessage="Du måste namnge en funktion.")]
		[StringLength(64, ErrorMessage="Funktionsnamnet får max innehåll 64 tecken.")]
		[Display(Name="Funktion")]
		public string Function { get ; set ; }

		[Column(Name="sysaccess_description")]
		[StringLength(255), Display(Name="Beskrivning")]
		public string Description { get ; set ; }

		[Column(Name="sysaccess_created")]
		public override DateTime Created { get ; set ; }

		[Column(Name="sysaccess_updated")]
		public override DateTime Updated { get ; set ; }

		[Column(Name="sysaccess_created_by")]
		public override Guid CreatedBy { get ; set ; }

		[Column(Name="sysaccess_updated_by")]
		public override Guid UpdatedBy { get ; set ; }
		#endregion

		/// <summary>
		/// Saves the current record.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Weather the action was successful</returns>
		public override bool Save(System.Data.IDbTransaction tx = null) {
			if (Function != null)
				Function = Function.ToUpper() ;
			return base.Save(tx);
		}
	}
}
