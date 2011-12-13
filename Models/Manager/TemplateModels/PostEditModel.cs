using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piranha.Models.Manager.TemplateModels
{
	/// <summary>
	/// Post template edit model for the manager area.
	/// </summary>
	public class PostEditModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the post template.
		/// </summary>
		public PostTemplate Template { get ; set ; }
		#endregion

		/// <summary>
		/// Gets the model for the template specified by the given id.
		/// </summary>
		/// <param name="id">The template id</param>
		/// <returns>The model</returns>
		public static PostEditModel GetById(Guid id) {
			PostEditModel m = new PostEditModel() ;
			m.Template = PostTemplate.GetSingle(id) ;

			return m ;
		}

		/// <summary>
		/// Saves the model.
		/// </summary>
		/// <returns>Weather the operation succeeded</returns>
		public virtual bool SaveAll() {
			try {
				return Template.Save() ;
			} catch { return false ; }
		}
	}
}
