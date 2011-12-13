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
	public class ParamEditModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the current access record.
		/// </summary>
		public SysParam Param { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ParamEditModel() {
			Param = new SysParam() ;
		}

		/// <summary>
		/// Gets the specified access model.
		/// </summary>
		/// <param name="id">The access id</param>
		/// <returns>The model</returns>
		public static ParamEditModel GetById(Guid id) {
			ParamEditModel m = new ParamEditModel() ;
			m.Param = SysParam.GetSingle(id) ;

			return m ;
		}

		/// <summary>
		/// Saves the access and all related information.
		/// </summary>
		/// <returns>Weather the action succeeded or not.</returns>
		public virtual bool SaveAll() {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				try {
					Param.Save(tx) ;
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
					Param.Delete() ;
					tx.Commit() ;
				} catch { tx.Rollback() ; return false ; }
			}
			return true ;
		}
	}
}
