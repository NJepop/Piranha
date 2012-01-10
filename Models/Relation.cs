using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// The relation is a way of connecting information from different tables to
	/// each other in a fairly loosely coupled way.
	/// </summary>
	[PrimaryKey(Column="relation_id")]
	public class Relation : GuidRecord<Relation>
	{
		#region Inner classes
		/// <summary>
		/// Defines the different types of relations.
		/// </summary>
		public enum RelationType {
			POSTCATEGORY
		}
		#endregion

		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="relation_id")]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets the type.
		/// </summary>
		[Column(Name="relation_type")]
		public RelationType Type { get ; set ; }

		/// <summary>
		/// Gets/sets the main data for the relation.
		/// </summary>
		[Column(Name="relation_data_id")]
		public Guid DataId { get ; set ; }

		/// <summary>
		/// Gets/sets the related data for the relation.
		/// </summary>
		[Column(Name="relation_related_id")]
		public Guid RelatedId { get ; set ; }
		#endregion

		/// <summary>
		/// Gets all relations for the given data id.
		/// </summary>
		/// <param name="id">The main object id</param>
		/// <returns>A list of relations</returns>
		public static List<Relation> GetByDataId(Guid id) {
			return Get("relation_data_id = @0", id) ;
		}

		/// <summary>
		/// Gets the available relations for the given type and related id.
		/// </summary>
		/// <param name="type">The relation type</param>
		/// <param name="id">The relation data</param>
		/// <returns>The relations</returns>
		public static List<Relation> GetByTypeAndRelatedId(RelationType type, Guid id) {
			return Relation.Get("relation_type = @0 AND relation_related_id = @1", type, id) ;
		}
	}
}
