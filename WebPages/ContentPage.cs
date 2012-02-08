using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Models;

namespace Piranha.WebPages
{
	/// <summary>
	/// Base class for all content pages.
	/// </summary>
	/// <typeparam name="T">The model type</typeparam>
	public abstract class ContentPage<T> : BasePage
	{
		#region Properties
		/// <summary>
		/// Gets/sets the content model.
		/// </summary>
		public new T Model { get ; protected set ; }
		#endregion
	}
}
