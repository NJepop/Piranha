using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// The SysUserPassword record is used for changing a users password in the database.
	/// </summary>
	[Table(Name="sysuser"), PrimaryKey(Column="sysuser_id")]
	public class SysUserPassword : GuidRecord<SysUserPassword>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="sysuser_id")]
		public override Guid Id { get ; set ; }
		
		/// <summary>
		/// Gets/sets the password.
		/// </summary>
		[Column(Name="sysuser_password", OnLoad="OnPasswordLoad", OnSave="OnPasswordSave")]
		public string Password { get ; set ; }
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the confirmation password.
		/// </summary>
		[Compare("Password", ErrorMessage="Lösenorden matchar inte.")]
		public string PasswordConfirm { get ; set ; }

		/// <summary>
		/// Checks weather the password is set and can be saved.
		/// </summary>
		public bool IsSet { get { return !String.IsNullOrEmpty(Password) ; } }
		#endregion

		/// <summary>
		/// Saves the record to the database. Checks so empty passwords don't get saved.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Weather the operation succeeded or not</returns>
		public override bool Save(System.Data.IDbTransaction tx = null) {
			if (IsSet)
				return base.Save(tx);
			else return false ;
		}
	}
}
