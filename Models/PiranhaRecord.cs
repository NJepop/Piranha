using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Base class for all active records in the Piranha framework.
	/// </summary>
	/// <typeparam name="T">The record type</typeparam>
	public abstract class PiranhaRecord<T> : GuidRecord<T>
	{
		#region Properties
		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		public abstract DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		public abstract DateTime Updated { get ; set ; }

		/// <summary>
		/// Gets/sets the updated by id.
		/// </summary>
		public abstract Guid CreatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the updated by id.
		/// </summary>
		public abstract Guid UpdatedBy { get ; set ; }
		#endregion

		/// <summary>
		/// Saves the current record to the database.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Wether the operation was successful</returns>
		public override bool Save(System.Data.IDbTransaction tx = null) {
			var user = HttpContext.Current.User;

			if (user.Identity.IsAuthenticated) {
				if (IsNew) {
					Created = DateTime.Now ;
					CreatedBy = new Guid(user.Identity.Name) ;
				}
				Updated = DateTime.Now ;
				UpdatedBy = new Guid(user.Identity.Name) ;

				return base.Save(tx) ;
			}
			throw new AccessViolationException("User must be logged in to save data.") ;
		}
	}

	/// <summary>
	/// Base class for all records that uses a guid as their primary key column.
	/// </summary>
	/// <typeparam name="T">The type of the record</typeparam>
	public abstract class GuidRecord<T> : ActiveRecord<T>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		public abstract Guid Id { get ; set ; }
		#endregion

		#region Handlers
		/// <summary>
		/// Used to format a string list when binding the active record.
		/// </summary>
		/// <param name="val">The database value</param>
		/// <returns>The record value</returns>
		public List<string> OnListLoad(string val) {
			if (!String.IsNullOrEmpty(val))
				return val.Split(new char[] {','}).ToList<string>() ;
			return new List<string>() ;
		}

		/// <summary>
		/// Used to format a string list when saving the active record.
		/// </summary>
		/// <param name="val">The record value</param>
		/// <returns>The database value</returns>
		public string OnListSave(List<string> val) {
			return val.ToArray().Implode(",") ;
		}

		/// <summary>
		/// Used to format a name when binding the active record.
		/// </summary>
		/// <param name="val">The database value</param>
		/// <returns>The record value</returns>
		protected ComplexName OnNameLoad(string val) {
			ComplexName name = new ComplexName() ;
			string[]     vals = val.Split(new char[] {','}) ;

			name.Singular = vals[0] ;
			name.Plural   = vals.Length > 1 ? vals[1] : null ;

			return name ;
		}

		/// <summary>
		/// Used to format a name when saving the active record.
		/// </summary>
		/// <param name="val">The record value</param>
		/// <returns>The database value</returns>
		protected string OnNameSave(ComplexName val) {
			return val.Singular + (val.Plural != null ? "," + val.Plural : "") ;
		}

		/// <summary>
		/// NEVER load passwords from the database ever.
		/// </summary>
		/// <param name="pwd">The password</param>
		/// <returns>An empty string</returns>
		protected string OnPasswordLoad(string pwd) {
			return "" ;
		}

		/// <summary>
		/// Encrypts the password before saving it to the database.
		/// </summary>
		/// <param name="pwd">The password</param>
		/// <returns>The encrypted password</returns>
		protected string OnPasswordSave(string pwd) {
			return Encrypt(pwd) ;
		}
		#endregion

		/// <summary>
		/// Saves the current record to the database.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Wether the operation was successful</returns>
		public override bool Save(System.Data.IDbTransaction tx = null) {
			if (IsNew && Id == Guid.Empty)
				Id = Guid.NewGuid() ;
			return base.Save(tx);			
		}

		/// <summary>
		/// Encrypts the given string with MD5.
		/// </summary>
		/// <param name="str">The encrypted string</param>
		/// <returns></returns>
		public static string Encrypt(string str) {
			UTF8Encoding encoder = new UTF8Encoding() ;
			SHA256CryptoServiceProvider crypto = new SHA256CryptoServiceProvider() ;

			byte[] bytes = crypto.ComputeHash(encoder.GetBytes(str)) ;
			return Convert.ToBase64String(bytes) ;
		}
	}
}
