using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Piranha.Models
{
	/// <summary>
	/// Login model passed to the controller during post.
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
		/// Gets/sets the optional return action.
		/// </summary>
		public string ReturnAction { get ; set ; }

		/// <summary>
		/// Gets/sets the optional return controller.
		/// </summary>
		public string ReturnController { get ; set ; }

		/// <summary>
		/// Gets/sets the optional return permalink.
		/// </summary>
		public string ReturnPermalink { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public LoginModel() {
			RememberMe = false ;
		}
	}
}
