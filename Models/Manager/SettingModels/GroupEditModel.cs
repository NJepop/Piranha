using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Piranha.Data;

namespace Piranha.Models.Manager.SettingModels
{
	public class GroupEditModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the group.
		/// </summary>
		public SysGroup Group { get ; set ; }
		
		/// <summary>
		/// Gets/sets of all of the other groups.
		/// </summary>
		public SelectList Groups { get ; set ; }

		/// <summary>
		/// Gets/sets the members of the group.
		/// </summary>
		public List<SysUser> Members { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public GroupEditModel() {
			Group = new SysGroup() ;
	
			List<SysGroup> groups = 
				SysGroup.GetFields("sysgroup_id, sysgroup_name", new Params() { OrderBy = "sysgroup_name ASC" }) ;
			groups.Insert(0, new SysGroup() { Name = "" }) ;

			Groups = new SelectList(groups, "Id", "Name") ;
		}

		/// <summary>
		/// Gets the specified group model.
		/// </summary>
		/// <param name="id">The group id</param>
		/// <returns>The model</returns>
		public static GroupEditModel GetById(Guid id) {
			GroupEditModel m = new GroupEditModel() ;
			List<SysGroup> groups = SysGroup.GetFields("sysgroup_id, sysgroup_name", 
				"sysgroup_id != @0", id, new Params() { OrderBy = "sysgroup_name ASC" }) ;
			groups.Insert(0, new SysGroup() { Name = "" }) ;

			m.Group   = SysGroup.GetSingle(id) ;
			m.Groups  = new SelectList(groups, "Id", "Name", m.Group.ParentId) ;
			m.Members = SysUser.GetFields("sysuser_id, sysuser_firstname, sysuser_surname", 
				"sysuser_group_id=@0", m.Group.Id) ;

			return m ;
		}

		/// <summary>
		/// Saves the group and all related information.
		/// </summary>
		/// <returns>Weather the action succeeded or not.</returns>
		public virtual bool SaveAll() {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				try {
					Group.Save(tx) ;
					tx.Commit();
				} catch { tx.Rollback() ; throw ; }
			}
			return true ;
		}

		/// <summary>
		/// Deletes the group and all related information.
		/// </summary>
		/// <returns>Weather the action succeeded or not.</returns>
		public virtual bool DeleteAll() {
			using (IDbTransaction tx = Database.OpenConnection().BeginTransaction()) {
				try {
					Group.Delete() ;
					tx.Commit() ;
				} catch { tx.Rollback() ; return false ; }
			}
			return true ;
		}

		/// <summary>
		/// Refreshes the model.
		/// </summary>
		public void Refresh() {
			Members = SysUser.GetFields("sysuser_id, sysuser_firstname, sysuser_surname", 
				"sysuser_group_id=@0", Group.Id) ;
		}
	}
}
