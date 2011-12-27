using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Piranha.WebPages
{
	/// <summary>
	/// Binds models from the http form data.
	/// </summary>
	public class ModelBinder
	{
		/// <summary>
		/// Binds the model from the current form data.
		/// </summary>
		/// <typeparam name="T">The model type</typeparam>
		/// <param name="prefix">Optional name prefix</param>
		/// <returns>The model</returns>
		public static T BindModel<T>(string prefix = "") {
			return (T)BindModel(typeof(T), prefix) ;
		}

		/// <summary>
		/// Binds the model of the given type from the current form data.
		/// </summary>
		/// <param name="type">The model type</param>
		/// <param name="prefix">Optional name prefix</param>
		/// <returns>The model</returns>
		public static object BindModel(Type type, string prefix = "") {
			return BindModel(HttpContext.Current.Request.Form, type, prefix) ;
		}

		#region Private members
		/// <summary>
		/// Binds the model of the given type from the given name value collection.
		/// </summary>
		/// <param name="form">The form data</param>
		/// <param name="type">The model type</param>
		/// <param name="prefix">Optional name prefix</param>
		/// <returns>The model</returns>
		private static object BindModel(NameValueCollection form, Type type, string prefix = "") {
			object ret = Activator.CreateInstance(type) ;

			foreach (PropertyInfo p in ret.GetType().GetProperties()) {
				if (form.AllKeys.Contains(prefix + p.Name)) {
					if (p.PropertyType == typeof(HtmlString))
						p.SetValue(ret, new HtmlString(form[prefix + p.Name]), null) ;
					else if (typeof(Enum).IsAssignableFrom(p.PropertyType))
						p.SetValue(ret, Enum.Parse(p.PropertyType, form[prefix + p.Name]), null) ;
					else p.SetValue(ret, Convert.ChangeType(form[prefix + p.Name], p.PropertyType), null) ;
				} else {
					var subform = new NameValueCollection() ;
					form.AllKeys.Each((i, e) => {
						if (e.StartsWith(prefix + p.Name))
							subform.Add(e, form[e]) ;
					});
					if (subform.Count > 0)
						p.SetValue(ret, BindModel(subform, p.PropertyType, prefix + p.Name + "."), null) ;
				}
			}
			return ret ;
		}
		#endregion
	}
}
