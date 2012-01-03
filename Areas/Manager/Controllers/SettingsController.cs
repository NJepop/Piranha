using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Piranha;
using Piranha.Data;
using Piranha.Controllers;
using Piranha.Models.Manager.SettingModels;

namespace byBrick.Areas.Manager.Controllers
{
	/// <summary>
	/// Settings controller for the manager area.
	/// </summary>
    public class SettingsController : ManagerController
    {
		#region Members
		private const string TITLE_USERINSERT = "Lägg till användare" ;
		private const string TITLE_USEREDIT   = "Ändra användare" ; 
		#endregion

		/// <summary>
		/// List action.
		/// </summary>
        public ActionResult Index() {
            return View("Index", ListModel.Get());
        }

		#region User actions
		/// <summary>
		/// Edits or creates a new user.
		/// </summary>
		/// <param name="id">The user id</param>
		[Access(Function="ADMIN_USER")]
		public new ActionResult User(string id) {
			if (!String.IsNullOrEmpty(id)) {
				ViewBag.Title = TITLE_USEREDIT ;
				return View("User", UserEditModel.GetById(new Guid(id))) ;
			} else {
				ViewBag.Title = TITLE_USERINSERT ;
				return View("User", new UserEditModel()) ;
			}
		}

		/// <summary>
		/// Searches the users by the given filter.
		/// </summary>
		/// <param name="filter">The search filter.</param>
		public ActionResult SearchUser(string filter) {
			string[] strings = filter.Split(new char[] { ' ' }) ;
			string where = "" ;
			List<object> args = new List<object>() ;

			ViewBag.SelectedTab = "users" ;

			foreach (string str in strings) {
				where += (where != "" ? " OR " : "") +
					"(sysuser_login LIKE @0 OR " +
					"sysuser_firstname LIKE @0 OR " +
					"sysuser_surname LIKE @0 OR " +
					"sysuser_email LIKE @0 OR " +
					"sysuser_created LIKE @0 OR " +
					"sysuser_updated LIKE @0)" ;
				args.Add("%" + str + "%") ;
			}
			args.Add(new Params() { OrderBy = "sysuser_login ASC" }) ;
			return View("Index", ListModel.GetByUserFilter(where, args.ToArray())) ;
		}

		/// <summary>
		/// Saves the model
		/// </summary>
		/// <param name="em">The model</param>
		[HttpPost()]
		[Access(Function="ADMIN_USER")]
		public new ActionResult User(UserEditModel um) {
			if (um.User.IsNew)
				ViewBag.Title = TITLE_USERINSERT ;
			else ViewBag.Title = TITLE_USEREDIT ;

			if (ModelState.IsValid) {
				try {
					if (um.SaveAll()) {
						ModelState.Clear() ;
						ViewBag.Message = "Användaren har sparats" ;
					} else ViewBag.Message = "Det gick inte att spara användaren" ;
				} catch (Exception e) {
					ViewBag.Message = e.ToString() ;
				}
			}
			return View("User", um) ;
		}

		/// <summary>
		/// Deletes the specified user
		/// </summary>
		/// <param name="id">The user id</param>
		[Access(Function="ADMIN_USER")]
		public ActionResult DeleteUser(string id) {
			UserEditModel um = UserEditModel.GetById(new Guid(id)) ;
			
			ViewBag.SelectedTab = "users" ;
			if (um.DeleteAll())
				ViewBag.Message = "Användaren har raderats." ;
			else ViewBag.Message = "Ett fel har inträffat och användaren kunde inte raderas." ;
			
			return Index() ;
		}
		#endregion

		#region Group actions
		/// <summary>
		/// Edits or creates a new group
		/// </summary>
		/// <param name="id">The group id</param>
		[Access(Function="ADMIN_GROUP")]
		public ActionResult Group(string id) {
			if (!String.IsNullOrEmpty(id)) {
				ViewBag.Title = "Ändra grupp" ;
				return View("Group", GroupEditModel.GetById(new Guid(id))) ;
			} else {
				ViewBag.Title = "Lägg till grupp" ;
				return View("Group", new GroupEditModel()) ;
			}
		}

		/// <summary>
		/// Searches the groups for the given search filter
		/// </summary>
		/// <param name="filter">The search string</param>
		public ActionResult SearchGroup(string filter) {
			string[] strings = filter.Split(new char[] { ' ' }) ;
			string where = "" ;
			List<object> args = new List<object>() ;

			ViewBag.SelectedTab = "groups" ;

			foreach (string str in strings) {
				where += (where != "" ? " OR " : "") +
					"(sysgroup_name LIKE @0 OR " +
					"sysgroup_description LIKE @0 OR " +
					"sysgroup_created LIKE @0 OR " +
					"sysgroup_updated LIKE @0)" ;
				args.Add("%" + str + "%") ;
			}
			args.Add(new Params() { OrderBy = "sysgroup_name ASC" }) ;
			return View("Index", ListModel.GetByGroupFilter(where, args.ToArray())) ;
		}

		/// <summary>
		/// Saves the group
		/// </summary>
		/// <param name="gd">The model</param>
		[HttpPost()]
		[Access(Function="ADMIN_GROUP")]
		public ActionResult Group(GroupEditModel gm) {
			ViewBag.Title = "Ändra grupp" ;
			if (ModelState.IsValid) {
				try {
					if (gm.SaveAll()) {
						ModelState.Clear() ;
						ViewBag.Message = "Gruppen har sparats" ;
					} else ViewBag.Message = "Det gick inte att spara gruppen" ;
				} catch (Exception e) {
					ViewBag.Message = e.ToString() ;
				}
			}
			gm.Refresh() ;
			return View("Group", gm) ;
		}

		/// <summary>
		/// Deletes the specified group
		/// </summary>
		/// <param name="id">The group id</param>
		[Access(Function="ADMIN_GROUP")]
		public ActionResult DeleteGroup(string id) {
			GroupEditModel gm = GroupEditModel.GetById(new Guid(id)) ;
			
			ViewBag.SelectedTab = "groups" ;
			if (gm.DeleteAll())
				ViewBag.Message = "Gruppen har raderats." ;
			else ViewBag.Message = "Ett fel har inträffat och gruppen kunde inte raderas." ;
			
			return Index() ;
		}
		#endregion

		#region Access actions
		/// <summary>
		/// Edits or creates a new group
		/// </summary>
		/// <param name="id">The group id</param>
		[Access(Function="ADMIN_ACCESS")]
		public ActionResult Access(string id) {
			if (!String.IsNullOrEmpty(id)) {
				ViewBag.Title = "Ändra behörighet" ;
				return View("Access", AccessEditModel.GetById(new Guid(id))) ;
			} else {
				ViewBag.Title = "Lägg till behörighet" ;
				return View("Access", new AccessEditModel()) ;
			}
		}

		/// <summary>
		/// Searches the access roles by the given filter.
		/// </summary>
		/// <param name="filter">The search filter.</param>
		public ActionResult SearchAccess(string filter) {
			string[] strings = filter.Split(new char[] { ' ' }) ;
			string where = "" ;
			List<object> args = new List<object>() ;

			ViewBag.SelectedTab = "access" ;

			foreach (string str in strings) {
				where += (where != "" ? " OR " : "") +
					"(sysaccess_function LIKE @0 OR " +
					"sysaccess_description LIKE @0 OR " +
					"sysgroup_name LIKE @0 OR " +
					"sysaccess_created LIKE @0 OR " +
					"sysaccess_updated LIKE @0)" ;
				args.Add("%" + str + "%") ;
			}
			args.Add(new Params() { OrderBy = "sysaccess_function ASC" }) ;
			return View("Index", ListModel.GetByAccessFilter(where, args.ToArray())) ;
		}

		/// <summary>
		/// Saves the access
		/// </summary>
		/// <param name="gd">The model</param>
		[HttpPost()]
		[Access(Function="ADMIN_ACCESS")]
		public ActionResult Access(AccessEditModel am) {
			ViewBag.Title = "Ändra behörighet" ;
			if (ModelState.IsValid) {
				try {
					if (am.SaveAll()) {
						ModelState.Clear() ;
						ViewBag.Message = "Behörigheten har sparats" ;
					} else ViewBag.Message = "Det gick inte att spara behörigheten" ;
				} catch (Exception e) {
					ViewBag.Message = e.ToString() ;
				}
				ControllerContext.HttpContext.Application.Remove(PiranhaApp.ACCESS_LIST) ;
			}
			return View("Access", am) ;
		}

		/// <summary>
		/// Deletes the specified group
		/// </summary>
		/// <param name="id">The access id</param>
		[Access(Function="ADMIN_ACCESS")]
		public ActionResult DeleteAccess(string id) {
			AccessEditModel am = AccessEditModel.GetById(new Guid(id)) ;
			
			ViewBag.SelectedTab = "access" ;
			if (am.DeleteAll())
				ViewBag.Message = "Behörigheten har raderats." ;
			else ViewBag.Message = "Ett fel har inträffat och behörigheten kunde inte raderas." ;

			ControllerContext.HttpContext.Application.Remove(PiranhaApp.ACCESS_LIST) ;			

			return Index() ;
		}
		#endregion

		#region Param actions
		/// <summary>
		/// Edits or creates a new parameter
		/// </summary>
		/// <param name="id">Parameter id</param>
		[Access(Function="ADMIN_PARAM")]
		public ActionResult Param(string id) {
			if (!String.IsNullOrEmpty(id)) {
				ViewBag.Title = "Ändra parameter" ;
				return View("Param", ParamEditModel.GetById(new Guid(id))) ;
			} else {
				ViewBag.Title = "Lägg till parameter" ;
				return View("Param", new ParamEditModel()) ;
			}
		}

		/// <summary>
		/// Searches the params by the given filter.
		/// </summary>
		/// <param name="filter">The search filter.</param>
		public ActionResult SearchParam(string filter) {
			string[] strings = filter.Split(new char[] { ' ' }) ;
			string where = "" ;
			List<object> args = new List<object>() ;

			ViewBag.SelectedTab = "params" ;

			foreach (string str in strings) {
				where += (where != "" ? " OR " : "") +
					"(sysparam_name LIKE @0 OR " +
					"sysparam_description LIKE @0 OR " +
					"sysparam_value LIKE @0 OR " +
					"sysparam_created LIKE @0 OR " +
					"sysparam_updated LIKE @0)" ;
				args.Add("%" + str + "%") ;
			}
			args.Add(new Params() { OrderBy = "sysparam_name ASC" }) ;
			return View("Index", ListModel.GetByParamFilter(where, args.ToArray())) ;
		}

		/// <summary>
		/// Edits or creates a new parameter
		/// </summary>
		/// <param name="id">Parameter id</param>
		[HttpPost()]
		[Access(Function="ADMIN_PARAM")]
		public ActionResult Param(ParamEditModel pm) {
			ViewBag.Title = "Ändra parameter" ;
			if (ModelState.IsValid) {
				try {
					if (pm.SaveAll()) {
						ModelState.Clear() ;
						ViewBag.Message = "Parametern har sparats" ;
					} else ViewBag.Message = "Det gick inte att spara parametern" ;
				} catch (Exception e) {
					ViewBag.Message = e.ToString() ;
				}
			}
			return View("Param", pm) ;
		}


		/// <summary>
		/// Deletes the specified param
		/// </summary>
		/// <param name="id">The param</param>
		public ActionResult DeleteParam(string id) {
			ParamEditModel pm = ParamEditModel.GetById(new Guid(id)) ;
			
			ViewBag.SelectedTab = "params" ;
			if (pm.DeleteAll())
				ViewBag.Message = "Parametern har raderats." ;
			else ViewBag.Message = "Ett fel har inträffat och parametern kunde inte raderas." ;

			return Index() ;
		}
		#endregion
	}
}
