using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Piranha.Models
{
	/// <summary>
	/// Login model passed to the account handler.
	/// </summary>
	public class LoginModel {
		#region Properties
		/// <summary>
		/// Gets/sets the login name.
		/// </summary>
		[Required()]
		public string Login { get ; set ; }

		/// <summary>
		/// Gets/sets the password.
		/// </summary>
		[Required()]
		public string Password { get ; set ; }

		/// <summary>
		/// Gets/sets weather the cookie should be persistant.
		/// </summary>
		public bool RememberMe { get ; set ; }

		/// <summary>
		/// Gets/sets the return url.
		/// </summary>
		public string ReturnUrl { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public LoginModel() {
			RememberMe = false ;
		}
	}
}
