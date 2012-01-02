using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Text;
using System.Web;
using System.Web.WebPages;

using Piranha.Models;

namespace Piranha.WebPages
{
	/// <summary>
	/// Base class for Razor syntax web pages.
	/// </summary>
	public abstract class BasePage : WebPage
	{
		#region Properties
		/// <summary>
		/// Gets the helper for the piranha methods.
		/// </summary>
		public PiranhaHelper Piranha { get ; private set ; }

		/// <summary>
		/// Gets the current permalink.
		/// </summary>
		protected string Permalink { get ; private set ; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new page.
		/// </summary>
		public BasePage() : base() {
			Piranha = new PiranhaHelper(this, Html) ;
		}
	}
}
