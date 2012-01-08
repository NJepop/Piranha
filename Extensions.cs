using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Piranha.Models;

/// <summary>
/// Piranha class extensions
/// </summary>
public static class PiranhaApp
{
	#region Members
	public const string USER = "Piranha_User" ;
	#endregion
	
	#region Language extensions
	/// <summary>
	/// Implodes the string array into a string with all item separated by the given separator.
	/// </summary>
	/// <param name="arr">The array to implode</param>
	/// <param name="sep">The optional separator</param>
	/// <returns>The string</returns>
	public static string Implode(this string[] arr, string sep = "") {
		string ret = "" ;
		for (int n = 0; n < arr.Length; n++)
			ret += (n > 0 ? sep : "") + arr[n] ;
		return ret ;
	}

	/// <summary>
	/// Loops the current enumerable and executes the given action on each of
	/// the items.
	/// </summary>
	/// <typeparam name="T">The item type</typeparam>
	/// <param name="ienum">The enumerable</param>
	/// <param name="proc">The action to execute</param>
	public static void Each<T>(this IEnumerable<T> ienum, Action<int, T> proc) {
		int index = 0 ;
		foreach (T itm in ienum)
			proc(index++, itm) ;
	}

	/// <summary>
	/// Gets the unvaliated value with the given key.
	/// </summary>
	/// <param name="provider">The value provider</param>
	/// <param name="key">The key</param>
	/// <returns>The value</returns>
	public static ValueProviderResult GetUnvalidatedValue(this IValueProvider provider, string key) {
		return ((IUnvalidatedValueProvider)provider).GetValue(key, true) ;
	}

	/// <summary>
	/// Gets the first custom attribute of type T for the given type.
	/// </summary>
	/// <typeparam name="T">The attribute type</typeparam>
	/// <param name="type">The current type</param>
	/// <param name="inherit">If inherited attributes should be included</param>
	/// <returns>The attribute, if it was found</returns>
	public static T GetCustomAttribute<T>(this Type type, bool inherit) {
		object[] arr = type.GetCustomAttributes(typeof(T), inherit) ;

		return arr.Length > 0 ? (T)arr[0] : default(T) ;
	}

	/// <summary>
	/// Gets the first custom attribute of type T for the given member.
	/// </summary>
	/// <typeparam name="T">The attribute type</typeparam>
	/// <param name="type">The current type</param>
	/// <param name="inherit">If inherited attributes should be included</param>
	/// <returns>The attribute, if it was found</returns>
	public static T GetCustomAttribute<T>(this MemberInfo member, bool inherit) {
		object[] arr = member.GetCustomAttributes(typeof(T), inherit) ;

		return arr.Length > 0 ? (T)arr[0] : default(T) ;
	}

	/// <summary>
	/// Gets the attributes of type T for the given type.
	/// </summary>
	/// <typeparam name="T">The attribute type</typeparam>
	/// <param name="type">The type</param>
	/// <param name="inherit">If inherited attributes should be included</param>
	/// <returns>An array of attributes</returns>
	public static T[] GetCustomAttributes<T>(this Type type, bool inherit) {
		return Array.ConvertAll<object, T>(type.GetCustomAttributes(typeof(T), inherit), (o) => (T)o) ;
	}
	#endregion

	#region CMS extension
	/// <summary>
	/// Gets the currently logged in user.
	/// </summary>
	/// <param name="p">The security principal</param>
	/// <returns>The current user</returns>
	public static SysUser GetProfile(this IPrincipal p) {
		if (p.Identity.IsAuthenticated) {
			// Reload user if session has been dropped
			if (HttpContext.Current.Session[USER] == null)
				HttpContext.Current.Session[USER] = 
					SysUser.GetSingle(new Guid(p.Identity.Name)) ;
			return (SysUser)HttpContext.Current.Session[USER] ;
		}
		return new SysUser() ;
	}

	/// <summary>
	/// Checks if the current user has access to the function.
	/// </summary>
	/// <param name="p">The principal</param>
	/// <param name="function">The function to check</param>
	/// <returns>Weather the user has access</returns>
	public static bool HasAccess(this IPrincipal p, string function) {
		if (p.Identity.IsAuthenticated) {
			Dictionary<string, SysAccess> access = SysAccess.GetAccessList() ;

			if (access.ContainsKey(function)) {
				SysGroup group = SysGroup.GetStructure().GetGroupById(p.GetProfile().GroupId) ;
				return group != null && (group.Id == access[function].GroupId || group.HasChild(access[function].GroupId)) ;
			}
		}
		return false ;
	}

	/// <summary>
	/// Checks if the user is a member of the given group or is a member
	/// of a group that has higher priviliges than the given group.
	/// </summary>
	/// <param name="p">The principal</param>
	/// <param name="groupid">The group</param>
	/// <returns>Weather the user is a member</returns>
	public static bool IsMember(this IPrincipal p, Guid groupid) {
		if (p.Identity.IsAuthenticated) {
			if (groupid != Guid.Empty) {
				SysGroup g = SysGroup.GetStructure().GetGroupById(p.GetProfile().GroupId) ;
				return g.Id == groupid || g.HasChild(groupid) ;
			}
			return true ;
		}
		return false ;
	}

	/// <summary>
	/// Checks if the user is a member of the given group or is a member
	/// of a group that has higher priviliges than the given group.
	/// </summary>
	/// <param name="p">The principal</param>
	/// <param name="groupname">The group</param>
	/// <returns>Weather the user is a member</returns>
	public static bool IsMember(this IPrincipal p, string groupname) {
		SysGroup g = SysGroup.GetSingle("sysgroup_name = @0", groupname) ;
		if (g != null)
			return IsMember(p, g.Id) ;
		return false ;
	}
	#endregion
}
