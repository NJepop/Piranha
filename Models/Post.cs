using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Active record for a page.
	/// </summary>
	[PrimaryKey(Column="post_id")]
	[Join(TableName="posttemplate", ForeignKey="post_template_id", PrimaryKey="posttemplate_id")]
	[Join(TableName="permalink", ForeignKey="post_id", PrimaryKey="permalink_parent_id")]
	public class Post : PiranhaRecord<Post>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="post_id")]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets the template id.
		/// </summary>
		[Column(Name="post_template_id")]
		[Required()]
		public Guid TemplateId { get ; set ; }

		/// <summary>
		/// Gets/sets the title.
		/// </summary>
		[Column(Name="post_title")]
		[Required()]
		[Display(Name="Titel")]
		public string Title { get ; set ; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		//[Column(Name="post_permalink")]
		[Column(Name="permalink_name", ReadOnly=true)]
		[Display(Name="Permalänk")]
		public string Permalink { get ; set ; }

		/// <summary>
		/// Gets/sets the excerpt.
		/// </summary>
		[Column(Name="post_excerpt")]
		[Display(Name="Sammanfattning")]
		public HtmlString Excerpt { get ; set ; }

		/// <summary>
		/// Gets/sets the body.
		/// </summary>
		[Column(Name="post_body")]
		[Display(Name="Innehåll")]
		public HtmlString Body { get ; set ; }

		/// <summary>
		/// Gets/sets the custom view
		/// </summary>
		[Column(Name="posttemplate_view", ReadOnly=true)]
		public string View { get ; set ; }

		/// <summary>
		/// Gets/sets the template name.
		/// </summary>
		[Column(Name="posttemplate_name", ReadOnly=true, OnLoad="OnNameLoad", OnSave="OnNameSave")]
		public ComplexName TemplateName { get ; set ; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name="post_created")]
		public override DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name="post_updated")]
		public override DateTime Updated { get ; set ; }

		/// <summary>
		/// Gets/sets the published date.
		/// </summary>
		[Column(Name="post_published")]
		public  DateTime Published { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="post_created_by")]
		public override Guid CreatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="post_updated_by")]
		public override Guid UpdatedBy { get ; set ; }
		#endregion

		/// <summary>
		/// Gets the post specified by the given permalink.
		/// </summary>
		/// <param name="permalink">The permalink</param>
		/// <returns>The post</returns>
		public static Post GetByPermalink(string permalink) {
			return Post.GetSingle("permalink_name = @0", permalink) ;
		}

		/// <summary>
		/// Gets the posts of the given template.
		/// </summary>
		/// <param name="templateid">The template id</param>
		/// <returns>A list of posts</returns>
		public static List<Post> GetByTemplateId(Guid templateid) {
			return Post.Get("post_template_id = @0", templateid) ;
		}

		/// <summary>
		/// Gets the post of the given template.
		/// </summary>
		/// <param name="name">The template name</param>
		/// <returns>A list of posts</returns>
		public static List<Post> GetByTemplateName(string name) {
			return Post.Get("post_template_id = (SELECT posttemplate_id FROM posttemplate WHERE posttemplate_name LIKE @0)",
				"%" + name) ;
		}
	}
}
