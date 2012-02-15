using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piranha.Linq
{
	/// <summary>
	/// Interface for cached records.
	/// </summary>
	public interface ICacheRecord {
		/// <summary>
		/// Invalidates the current record from the current cache. The method is called
		/// after save and delete.
		/// </summary>
		void InvalidateRecord() ;
	}
}
