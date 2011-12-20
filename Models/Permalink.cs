using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Piranha.Data;

namespace Piranha.Models
{
	[PrimaryKey(Column="permalink_id")]
	public class Permalink : PiranhaRecord<Permalink>
	{
		#region Inner classes
		public enum PermalinkType {
			PAGE, POST
		}
		#endregion

		#region Fields
		[Column(Name="permalink_id")]
		[Required()]
		public override Guid Id { get ; set ; }

		[Column(Name="permalink_parent_id")]
		public Guid ParentId { get ; set ; }

		[Column(Name="permalink_type")]
		public PermalinkType Type { get ; set ; }

		[Column(Name="permalink_name")]
		public string Name { get ; set ; }

		[Column(Name="permalink_created")]
		public override DateTime Created { get ; set ; }

		[Column(Name="permalink_updated")]
		public override DateTime Updated { get ; set ; }

		[Column(Name="permalink_created_by")]
		public override Guid CreatedBy { get ; set ; }

		[Column(Name="permalink_updated_by")]
		public override Guid UpdatedBy { get ; set ; }
		#endregion

		#region Static accessors
		/// <summary>
		/// Gets the permalink with the given name.
		/// </summary>
		/// <param name="name">The permalink name</param>
		/// <returns>The permalink</returns>
		public static Permalink GetByName(string name) {
			return GetSingle("permalink_name = @0", name) ;
		}
		#endregion

		/// <summary>
		/// Converts the given string to a web safe permalink.
		/// </summary>
		/// <param name="str">The string</param>
		/// <returns>A permalink</returns>
		public static string Generate(string str) {
			return Regex.Replace(str.ToLower().Replace(" ", "-").Replace("å", "a").Replace("ä", "a").Replace("ö", "o"),
				@"[^a-z0-9-]", "") ;
		}
	}
}
