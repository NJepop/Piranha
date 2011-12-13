using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piranha.Models;

namespace Piranha.Web
{
	public class UploadResult : ImageResult
	{
		/// <summary>
		/// Default constructor. Creates a new image result.
		/// </summary>
		/// <param name="record">The content record</param>
		public UploadResult(Upload record) : 
			base("~/App_Data/Uploads/", record.Id.ToString(), record.Type) {}
	}
}
