using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// A category is used to classify content in the database.
	/// </summary>
	[PrimaryKey(Column="category_id")]
	[Join(TableName="permalink", ForeignKey="category_id", PrimaryKey="permalink_parent_id")]
	public class Category : PiranhaRecord<Category>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="category_id")]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets the optional parent id.
		/// </summary>
		[Column(Name="category_parent_id"), Display(Name="Överordnad")]
		public Guid ParentId { get ; set ; }

		/// <summary>
		/// Gets/sets the name.
		/// </summary>
		[Column(Name="category_name"), Required(ErrorMessage="Du måste ange ett namn för kategorin.")]
		[Display(Name="Namn"), StringLength(64, ErrorMessage="Namnet får max innehålla 64 tecken.")]
		public string Name { get ; set ; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		[Column(Name="permalink_name", ReadOnly=true), Display(Name="Permalänk")]
		public string Permalink { get ; set ; }

		/// <summary>
		/// Gets/sets the description.
		/// </summary>
		[Column(Name="category_description"), Display(Name="Beskrivning")]
		[StringLength(255, ErrorMessage="Beskrivningen får max innehålla 255 tecken.")]
		public string Description { get ; set ; }
		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name="category_created")]
		public override DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name="category_updated")]
		public override DateTime Updated { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="category_created_by")]
		public override Guid CreatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="category_updated_by")]
		public override Guid UpdatedBy { get ; set ; }
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets the child categories.
		/// </summary>
		public List<Category> Categories { get ; set ; }

		/// <summary>
		/// Gets/sets the level of the category.
		/// </summary>
		public int Level { get ; private set ; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new category.
		/// </summary>
		public Category() {
			Categories = new List<Category>() ;
		}

		#region Static accessors
		/// <summary>
		/// Gets the categories sorted recursivly.
		/// </summary>
		/// <returns>The categories</returns>
		public static List<Category> GetStructure() {
			List<Category> cats = Category.Get(new Params() { OrderBy = "category_parent_id, category_name" }) ;
			return Sort(cats, Guid.Empty) ;
		}

		/// <summary>
		/// Get the available categories for the given post
		/// </summary>
		/// <param name="id">The post id</param>
		/// <returns>The categories</returns>
		public static List<Category> GetByPostId(Guid id) {
			return Category.Get("category_id IN (" +
				"SELECT relation_related_id FROM relation WHERE relation_type = @0 AND relation_data_id = @1)",
				Relation.RelationType.POSTCATEGORY, id) ;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Sorts the categories
		/// </summary>
		/// <param name="categories">The categories to sort</param>
		/// <param name="parentid">Parent id</param>
		/// <returns>A list of categories</returns>
		private static List<Category> Sort(List<Category> categories, Guid parentid, int level = 1) {
			List<Category> ret = new List<Category>() ;

			foreach (Category cat in categories) {
				if (cat.ParentId == parentid) {
					cat.Categories = Sort(categories, cat.Id, level + 1) ;
					cat.Level = level ;
					ret.Add(cat) ;
				}
			}
			return ret;
		}
		#endregion
	}


	/// <summary>
	/// Category extensions.
	/// </summary>
	public static class CategoryHelper {
		/// <summary>
		/// Flattens the structure into a list.
		/// </summary>
		/// <param name="groups">The structure to flatten</param>
		/// <returns>A list of groups</returns>
		public static List<Category> Flatten(this List<Category> categories) {
			List<Category> ret = new List<Category>() ;

			foreach (Category cat in categories) {
				ret.Add(cat) ;
				if (cat.Categories.Count > 0)
					ret.AddRange(Flatten(cat.Categories)) ;
			}
			return ret ;
		}
	}
}
