using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piranha.Data;

namespace Piranha.Models.Manager.ContentModels
{
	/// <summary>
	/// List model for the content view in the manager area.
	/// </summary>
	public class ListModel
	{
		#region Properties
		/// <summary>
		/// Gets/sets the available content.
		/// </summary>
		public List<Piranha.Models.Content> Content { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new model.
		/// </summary>
		public ListModel() {
			Content = new List<Piranha.Models.Content>() ;
		}

		/// <summary>
		/// Gets all available content.
		/// </summary>
		/// <returns>A list of content records</returns>
		public static ListModel Get() {
			ListModel lm = new ListModel() ;
			lm.Content = Piranha.Models.Content.Get(new Params() { OrderBy = "content_filename ASC" });

			return lm ;
		}
	}
}
