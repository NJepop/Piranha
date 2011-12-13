using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piranha.Models.Manager.PageModels
{
	/// <summary>
	/// Page insert model for the manager area.
	/// </summary>
	public class InsertModel
	{
		#region Properties
		/// <summary>
		/// Page template id.
		/// </summary>
		public Guid TemplateId { get ; set ; }

		/// <summary>
		/// Parent id.
		/// </summary>
		public Guid ParentId { get ; set ; }

		/// <summary>
		/// Page seqno
		/// </summary>
		public int Seqno { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor, creates a new model.
		/// </summary>
		public InsertModel() {
			TemplateId = Guid.Empty ;
			ParentId = Guid.Empty ;
			Seqno = 1 ;
		}
	}
}
