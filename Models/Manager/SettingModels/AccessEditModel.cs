using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Piranha.Data;

namespace Piranha.Models.Manager.SettingModels
{
	/// <summary>
	/// Edit model for the sys access record.
	/// </summary>
	public class AccessEditModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the current access record.
		/// </summary>
		public SysAccess Access { get ; set ; }

		/// <summary>
		/// Gets/sets the available groups.
		/// </summary>
		public SelectList Groups { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public AccessEditModel() {
			Access = new SysAccess() ;
	
			List<SysGroup> groups = SysGroup.GetStructure().Flatten() ;
			groups.Insert(0, new SysGroup() { Name = "" }) ;
			Groups = new SelectList(groups, "Id", "Name") ;
		}

		/// <summary>
		/// Gets the specified access model.
		/// </summary>
		/// <param name="id">The access id</param>
		/// <returns>The model</returns>
		public static AccessEditModel GetById(Guid id) {
			AccessEditModel m = new AccessEditModel() ;

			m.Access = SysAccess.GetSingle(id) ;

			return m ;
		}

		/// <summary>
		/// Saves the access and all related information.
		/// </summary>
		/// <returns>Weather the action succeeded or not.</returns>
		public virtual bool SaveAll() {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				try {
					Access.Save(tx) ;
					tx.Commit();
				} catch { tx.Rollback() ; throw ; }
			}
			return true ;
		}

		/// <summary>
		/// Deletes the access and all related information.
		/// </summary>
		/// <returns>Weather the action succeeded or not.</returns>
		public virtual bool DeleteAll() {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				try {
					Access.Delete() ;
					tx.Commit() ;
				} catch { tx.Rollback() ; return false ; }
			}
			return true ;
		}
	}
}
