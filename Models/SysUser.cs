using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;

using Piranha.Data;

namespace Piranha.Models
{
	[PrimaryKey(Column="sysuser_id")]
	[Join(TableName="SysGroup", ForeignKey="sysuser_group_id", PrimaryKey="sysgroup_id")]
	public class SysUser : GuidRecord<SysUser>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="sysuser_id")]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets the login name.
		/// </summary>
		[Column(Name="sysuser_login")]
		[Required(ErrorMessage="Du måste ange ett användarnamn.")]
		[StringLength(64, ErrorMessage="Användarnamnet får inte vara längre än 64 tecken.")]
		[Display(Name="Användarnamn")]
		public string Login { get ; set ; }

		/// <summary>
		/// Gets/sets the firstname.
		/// </summary>
		[Column(Name="sysuser_firstname")]
		[Required(ErrorMessage="Du måste ange användarens förnamn.")]
		[StringLength(64, ErrorMessage="Förnamnet får inte vara längre än 64 tecken.")]
		[Display(Name="Förnamn")]
		public string Firstname { get ; set ; }

		/// <summary>
		/// Gets/sets the surname.
		/// </summary>
		[Column(Name="sysuser_surname")]
		[Required(ErrorMessage="Du måste ange användarens efternamn.")]
		[StringLength(64, ErrorMessage="Efternamnet får inte vara längre än 128 tecken.")]
		[Display(Name="Efternamn")]
		public string Surname { get ; set ; }

		/// <summary>
		/// Gets/sets the email address.
		/// </summary>
		[Column(Name="sysuser_email")]
		[Required(ErrorMessage="Du måste ange en e-post adress.")]
		[StringLength(64, ErrorMessage="E-post adressen får inte vara längre än 128 tecken.")]
		[Display(Name="E-post")]
		public string Email { get ; set ; }

		/// <summary>
		/// Gets/sets the users current group.
		/// </summary>
		[Column(Name="sysuser_group_id")]
		[Display(Name="Grupp")]
		public Guid GroupId { get ; set ; }

		/// <summary>
		/// Gets/sets the users group name.
		/// </summary>
		[Column(Name="sysgroup_name", ReadOnly=true, Table="sysgroup")]
		public string GroupName { get ; set ; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name="sysuser_created")]
		public DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name="sysuser_updated")]
		public DateTime Updated { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="sysuser_created_by")]
		public Guid CreatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="sysuser_updated_by")]
		public Guid UpdatedBy { get ; set ; }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name for the user.
		/// </summary>
		public string Name {
			get { return (Firstname + " " + Surname).Trim() ; }
		}
		#endregion

		/// <summary>
		/// Authenticates the user and returns if it was successfull.
		/// </summary>
		/// <param name="login">The login</param>
		/// <param name="password">The password</param>
		/// <returns>An authenticated user</returns>
		public static SysUser Authenticate(string login, string password) {
			return GetSingle("sysuser_login = @0 AND sysuser_password = @1",
				login, SysUser.Encrypt(password)) ;
		}

		/// <summary>
		/// Authenticates and logs in the user.
		/// </summary>
		/// <param name="login">The login</param>
		/// <param name="password">The password</param>
		/// <param name="persistent">Weather the cookie should be persistent</param>
		/// <returns>If the user was successfully logged in</returns>
		public static bool LoginUser(string login, string password, bool persistent = false) {
			SysUser user = Authenticate(login, password) ;

			if (user != null) {
				FormsAuthentication.SetAuthCookie(user.Id.ToString(), persistent) ;
				return true ;
			}
			return false ;
		}

		/// <summary>
		/// Logs out the current user.
		/// </summary>
		public static void LogoutUser() {
			FormsAuthentication.SignOut() ;
			HttpContext.Current.Session.Clear() ;
			HttpContext.Current.Session.Abandon() ;
		}

		/// <summary>
		/// Saves the current record to the database.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Wether the operation was successful</returns>
		public override bool Save(System.Data.IDbTransaction tx = null) {
			if (IsNew) {
				Id      = Guid.NewGuid() ;
				Updated = Created = DateTime.Now ;
				UpdatedBy = CreatedBy = Id ;
			} else {
				Updated = DateTime.Now ;
			}
			return base.Save(tx);
		}
	}
}
