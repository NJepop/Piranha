using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Web;

namespace Piranha.Linq
{
	/// <summary>
	/// Base class for all piranha records.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BaseRecord<T> : ActiveRecord<T> where T : class
	{
		#region Properties
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		public abstract Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		public abstract DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		public abstract DateTime Updated { get ; set ; }

		/// <summary>
		/// Gets/sets the created by id.
		/// </summary>
		public abstract Guid CreatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the updated by id.
		/// </summary>
		public abstract Guid UpdatedBy { get ; set ; }
		#endregion

		/// <summary>
		/// Executed just before the record is inserted into the database.
		/// </summary>
		protected override void OnInsert() {
			var user = HttpContext.Current.User ;

			if (user.Identity.IsAuthenticated) {
				if (Id == Guid.Empty)
					Id = Guid.NewGuid() ;
				Created = Updated = DateTime.Now ;
				CreatedBy = UpdatedBy = new Guid(user.Identity.Name) ;
			} else throw new UnauthorizedAccessException("User must be logged in to save data.") ;
		}

		/// <summary>
		/// Executed just before the record is updated in the database.
		/// </summary>
		protected override void OnUpdate() {
			var user = HttpContext.Current.User ;

			if (user.Identity.IsAuthenticated) {
				Updated = DateTime.Now ;
				UpdatedBy = new Guid(user.Identity.Name) ;
			}
		}

		/// <summary>
		/// Executed just before the record is deleted from the database.
		/// </summary>
		protected override void OnDelete() {
			if (!HttpContext.Current.User.Identity.IsAuthenticated)
				throw new UnauthorizedAccessException("User must be logged in to delete data.") ;
		}
	}
}
