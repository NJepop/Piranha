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
	/// Active record for the access control table.
	/// </summary>
	[PrimaryKey(Column="sysaccess_id")] 
	[Join(TableName="sysgroup", ForeignKey="sysaccess_group_id", PrimaryKey="sysgroup_id")]
	public class SysAccess : PiranhaRecord<SysAccess>, ICacheRecord<SysAccess>
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

		[Column(Name="sysaccess_locked")]
		public bool IsLocked { get ; set ; }

		[Column(Name="sysaccess_created")]
		public override DateTime Created { get ; set ; }

		[Column(Name="sysaccess_updated")]
		public override DateTime Updated { get ; set ; }

		[Column(Name="sysaccess_created_by")]
		public override Guid CreatedBy { get ; set ; }

		[Column(Name="sysaccess_updated_by")]
		public override Guid UpdatedBy { get ; set ; }
		#endregion

		#region Static accessors
		/// <summary>
		/// Gets the indexed access list for the applications
		/// </summary>
		/// <returns>The access list</returns>
		public static Dictionary<string, SysAccess> GetAccessList() {
			if (HttpContext.Current.Cache[typeof(SysAccess).Name] == null) {
				HttpContext.Current.Cache[typeof(SysAccess).Name] = new Dictionary<string, SysAccess>() ;
				SysAccess.Get().ForEach((e) => 
					((Dictionary<string, SysAccess>)HttpContext.Current.Cache[typeof(SysAccess).Name]).Add(e.Function, e)) ;
			}
			return (Dictionary<string, SysAccess>)HttpContext.Current.Cache[typeof(SysAccess).Name] ;
		}
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

		/// <summary>
		/// Invalidates the current access cache.
		/// </summary>
		/// <param name="record">The record</param>
		public void InvalidateRecord(SysAccess record) {
			HttpContext.Current.Cache.Remove(typeof(SysAccess).Name) ;
		}
	}
}
