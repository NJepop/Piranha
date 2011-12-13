using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piranha.Data;

namespace Piranha.Models.Manager.SettingModels
{
	/// <summary>
	/// Settings list model for the manager area.
	/// </summary>
	public class ListModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the available users.
		/// </summary>
		public List<SysUser> Users { get ; set ; }

		/// <summary>
		/// Gets/sets the available groups.
		/// </summary>
		public List<SysGroup> Groups { get ; set ; }

		/// <summary>
		/// Gets/sets the available access rights.
		/// </summary>
		public List<SysAccess> Access { get ; set ; }

		/// <summary>
		/// Gets/sets the available params.
		/// </summary>
		public List<SysParam> Params { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new list model.
		/// </summary>
		public ListModel() {
			Users = new List<SysUser>() ;
			Groups = new List<SysGroup>() ;
			Access = new List<SysAccess>() ;
			Params = new List<SysParam>() ;
		}

		/// <summary>
		/// Gets all available data.
		/// </summary>
		/// <returns>The model</returns>
		public static ListModel Get() {
			ListModel m = new ListModel() ;

			m.Users = SysUser.Get(new Params() { OrderBy = "sysuser_login" }) ;
			m.Groups = SysGroup.GetStructure().Flatten() ;
			m.Access = SysAccess.Get(new Params() { OrderBy = "sysaccess_function" }) ;
			m.Params = SysParam.Get(new Params() { OrderBy = "sysparam_name" }) ;

			return m ;
		}

		/// <summary>
		/// Gets all available data with filtered users.
		/// </summary>
		/// <param name="where">User search</param>
		/// <param name="args">User search arguments</param>
		/// <returns>The model</returns>
		public static ListModel GetByUserFilter(string where, object[] args) {
			ListModel m = ListModel.Get() ;
			m.Users = SysUser.Get(where, args) ;
 
			return m ;
		}

		/// <summary>
		/// Gets all available data with filtered groups.
		/// </summary>
		/// <param name="where">Group search</param>
		/// <param name="args">Group search arguments</param>
		/// <returns>The model</returns>
		public static ListModel GetByGroupFilter(string where, object[] args) {
			ListModel m = ListModel.Get() ;
			m.Groups = SysGroup.Get(where, args) ;
 
			return m ;
		}

		/// <summary>
		/// Gets all available data with filtered access.
		/// </summary>
		/// <param name="where">Access search</param>
		/// <param name="args">Access search arguments</param>
		/// <returns>The model</returns>
		public static ListModel GetByAccessFilter(string where, object[] args) {
			ListModel m = ListModel.Get() ;
			m.Access = SysAccess.Get(where, args) ;
 
			return m ;
		}

		/// <summary>
		/// Gets all available data with filtered params.
		/// </summary>
		/// <param name="where">Param search</param>
		/// <param name="args">Param search arguments</param>
		/// <returns>The model</returns>
		public static ListModel GetByParamFilter(string where, object[] args) {
			ListModel m = ListModel.Get() ;
			m.Params = SysParam.Get(where, args) ;
 
			return m ;
		}
	}
}
