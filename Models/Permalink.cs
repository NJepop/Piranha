using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Piranha.Data;

namespace Piranha.Models
{
	[PrimaryKey(Column="permalink_id")]
	public class Permalink : PiranhaRecord<Permalink>, ICacheRecord<Permalink>
	{
		#region Inner classes
		public enum PermalinkType {
			PAGE, POST, CATEGORY
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

		#region Properties
		/// <summary>
		/// Gets the permalink cache object.
		/// </summary>
		private static Dictionary<string, Permalink> Cache {
			get {
				if (HttpContext.Current.Cache[typeof(Permalink).Name] == null)
					HttpContext.Current.Cache[typeof(Permalink).Name] = new Dictionary<string, Permalink>() ;
				return (Dictionary<string, Permalink>)HttpContext.Current.Cache[typeof(Permalink).Name] ;
			}
		}
		#endregion

		#region Static accessors
		/// <summary>
		/// Gets the permalink with the given name.
		/// </summary>
		/// <param name="name">The permalink name</param>
		/// <returns>The permalink</returns>
		public static Permalink GetByName(string name) {
			if (!Cache.ContainsKey(name))
				Cache[name] = GetSingle("permalink_name = @0", name) ;
			return Cache[name] ;
		}

		/// <summary>
		/// Gets the permalink associated with the given parent id.
		/// </summary>
		/// <param name="id">The id</param>
		/// <returns>The permalink</returns>
		public static Permalink GetByParentId(Guid id) {
			return Permalink.GetSingle("permalink_parent_id = @0", id) ;
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

		/// <summary>
		/// Invalidates the current record from the cache.
		/// </summary>
		/// <param name="record">The record</param>
		public void InvalidateRecord(Permalink record) {
			if (Cache.ContainsKey(record.Name))
				Cache.Remove(record.Name) ;
		}
	}
}
