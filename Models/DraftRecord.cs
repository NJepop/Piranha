using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Piranha.Models
{
	/// <summary>
	/// Extension of Piranha record that supports drafted version.
	/// </summary>
	/// <typeparam name="T">The record type</typeparam>
	public abstract class DraftRecord<T> : PiranhaRecord<T>
	{
		#region Properties
		/// <summary>
		/// Gets/sets weather this is a draft.
		/// </summary>
		public abstract bool IsDraft { get ; set ; }

		/// <summary>
		/// Gets/sets the date of first publish.
		/// </summary>
		public abstract DateTime Published { get ; set ; }

		/// <summary>
		/// Gets/sets the last published date.
		/// </summary>
		public abstract DateTime LastPublished { get ; set ; }
		#endregion

		/// <summary>
		/// Saves and publishes the current record
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Weather the operation succeeded or not</returns>
		public bool SaveAndPublish(System.Data.IDbTransaction tx = null) {
			var user = HttpContext.Current.User ;

			if (user.Identity.IsAuthenticated) {
				// First get previously published record
				IsDraft = false ;
				T self = GetSelf() ;

				// Set up the dates.
				LastPublished = Updated = DateTime.Now ;
				if (IsNew)
					Created = Updated ;
				if (self == null)
					Published = Updated ;

				// First save an up-to-date draft
				IsDraft = true ;
				Save(tx, false) ;

				// Now save a published version
				IsDraft = false ;
				if (self == null)
					IsNew = true ;
				Save(tx, false) ;

				return true ;
			}
			throw new AccessViolationException("User must be logged in to save data.") ;
		}

		/// <summary>
		/// Saves the current record.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns></returns>
		public override bool Save(System.Data.IDbTransaction tx = null) {
			// Always save as draft
			IsDraft = true ;
			return base.Save(tx);
		}

		#region Private methods
		/// <summary>
		/// Retrieves the current record from the database.
		/// </summary>
		/// <returns>The current record</returns>
		private T GetSelf() {
			List<object> args = new List<object>() ;
			string where = "" ;

			for (int n = 0; n < PrimaryKeys.Count; n++) {
				where += (n > 0 ? " AND " : "") + PrimaryKeys[n] + "=@" + n.ToString() ;
				args.Add(Columns[PrimaryKeys[n]].GetValue(this, null)) ;
			}
			return GetSingle(where, args.ToArray()) ;
		}
		#endregion
	}
}
