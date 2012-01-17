using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

		/// <summary>
		/// Initializes the web page.
		/// </summary>
		protected override void InitializePage() {
			base.InitializePage() ;
	
			if (IsPost) {
				if (Request.Form.AllKeys.Contains("piranha_form_action")) {
					MethodInfo m = GetType().GetMethod(Request.Form["piranha_form_action"],
						BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.IgnoreCase);
					if (m != null) {
						List<object> args = new List<object>() ;
						foreach (var param in m.GetParameters())
							args.Add(ModelBinder.BindModel(param.ParameterType)) ;
						m.Invoke(this, args.ToArray()) ;
					}
				}
			}
		}
	}
}
