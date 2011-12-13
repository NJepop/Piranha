using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piranha.Models;

namespace Piranha.Web
{
	public class PiranhaImageResult : ImageResult
	{
		/// <summary>
		/// Default constructor. Creates a new image result.
		/// </summary>
		/// <param name="record">The content record</param>
		public PiranhaImageResult(Content record) : 
			base("~/App_Data/Content/", record.Id.ToString(), record.Type) {}
	}
}
