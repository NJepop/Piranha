using System;
using System.Collections.Generic;
using System.Linq;
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
			return Save(tx, true) ;
		}

		/// <summary>
		/// Saves the current record to the database.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <param name="setdates">Weather to automatically set the dates</param>
		/// <returns>Wether the operation was successful</returns>
		protected bool Save(System.Data.IDbTransaction tx = null, bool setdates = true) {
			var user = HttpContext.Current.User;

			if (user.Identity.IsAuthenticated) {
				if (IsNew) {
					if (setdates)
						Created = DateTime.Now ;
					CreatedBy = new Guid(user.Identity.Name) ;
				}
				if (setdates)
					Updated = DateTime.Now ;
				UpdatedBy = new Guid(user.Identity.Name) ;

				return base.Save(tx) ;
			}
			throw new AccessViolationException("User must be logged in to save data.") ;
		}		
	}
}
