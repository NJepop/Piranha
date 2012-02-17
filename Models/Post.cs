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
	[PrimaryKey(Column="post_id,post_draft")]
	[Join(TableName="posttemplate", ForeignKey="post_template_id", PrimaryKey="posttemplate_id")]
	[Join(TableName="permalink", ForeignKey="post_id", PrimaryKey="permalink_parent_id")]
	public class Post : DraftRecord<Post>, IPost
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="post_id")]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets weather this is a draft.
		/// </summary>
		[Column(Name="post_draft")]
		public override bool IsDraft { get ; set ; }

		/// <summary>
		/// Gets/sets the template id.
		/// </summary>
		[Column(Name="post_template_id")]
		public Guid TemplateId { get ; set ; }

		/// <summary>
		/// Gets/sets the title.
		/// </summary>
		[Column(Name="post_title")]
		[Required(ErrorMessage="Du måste ange en titel."), Display(Name="Titel")]
		[StringLength(128, ErrorMessage="Titeln får max innehålla 128 tecken.")]
		public string Title { get ; set ; }

		/// <summary>
		/// Gets/sets the permalink.
		/// </summary>
		[Column(Name="permalink_name", ReadOnly=true, Table="permalink")]
		[Display(Name="Permalänk")]
		public string Permalink { get ; set ; }

		/// <summary>
		/// Gets/sets the excerpt.
		/// </summary>
		[Column(Name="post_excerpt")]
		[Display(Name="Sammanfattning"), StringLength(255, ErrorMessage="Sammanfattningen får max innehålla 255 tecken.")]
		public string Excerpt { get ; set ; }

		/// <summary>
		/// Gets/sets the body.
		/// </summary>
		[Column(Name="post_body")]
		[Display(Name="Innehåll")]
		public HtmlString Body { get ; set ; }

		/// <summary>
		/// Gets/sets the custom view
		/// </summary>
		[Column(Name="posttemplate_view", ReadOnly=true, Table="posttemplate")]
		public string View { get ; private set ; }

		/// <summary>
		/// Gets/sets the template name.
		/// </summary>
		[Column(Name="posttemplate_name", ReadOnly=true, Table="posttemplate", OnLoad="OnNameLoad", OnSave="OnNameSave")]
		public ComplexName TemplateName { get ; private set ; }

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
		public override DateTime Published { get ; set ; }

		/// <summary>
		/// Gets/sets the published date.
		/// </summary>
		[Column(Name="post_last_published")]
		public override DateTime LastPublished { get ; set ; }

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
		/// Default constructor. Creates a new post.
		/// </summary>
		public Post() {
			IsDraft = true ;
		}

		#region Static accessors
		/// <summary>
		/// Gets a single post from the database.
		/// </summary>
		/// <param name="id">The post id</param>
		/// <param name="draft">Weather to get the draft</param>
		/// <returns>The post</returns>
		public static Post GetSingle(Guid id, bool draft) {
			if (!draft)
				return GetSingle(id) ;
			return GetSingle("post_id = @0 AND post_draft = @1", id, draft) ;
		}

		/// <summary>
		/// Gets the post specified by the given permalink.
		/// </summary>
		/// <param name="permalink">The permalink</param>
		/// <returns>The post</returns>
		public static Post GetByPermalink(string permalink, bool draft = false) {
			return Post.GetSingle("permalink_name = @0 AND post_draft = @1", permalink, draft) ;
		}
		#endregion
	}
}
