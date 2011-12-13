using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piranha.Models
{
	/// <summary>
	/// Interface all models supplied to a layout must conform to.
	/// </summary>
	public interface IModel
	{
		/// <summary>
		/// Gets the current page record.
		/// </summary>
		Page Page { get ; }
	}
}
