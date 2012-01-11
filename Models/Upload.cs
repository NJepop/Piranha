using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// Record for content upload by the application that's not a content 
	/// record handled by the cms.
	/// </summary>
	[PrimaryKey(Column="upload_id")]
	public class Upload : GuidRecord<Upload>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="upload_id")]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/set the id of the entity this uploaded is related to.
		/// </summary>
		[Column(Name="upload_parent_id")]
		public Guid ParentId { get ; set ; }

		/// <summary>
		/// Gets/sets the filename.
		/// </summary>
		[Column(Name="upload_filename")]
		public string Filename { get ; set ; }

		/// <summary>
		/// Gets/sets the file content type.
		/// </summary>
		[Column(Name="upload_type")]
		public string Type { get ; set ; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name="upload_created")]
		public DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the id of the creator.
		/// </summary>
		[Column(Name="upload_created_by")]
		public Guid CreatedBy { get ; set ; }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the virtual path for the content media file.
		/// </summary>
		public string VirtualPath { 
			get { return "~/App_Data/Uploads/" + Id ; }
		}

		/// <summary>
		/// Gets the physical path for the content media file.
		/// </summary>
		public string PhysicalPath {
			get { return HttpContext.Current.Server.MapPath(VirtualPath) ; }
		}
		#endregion

		#region Static accessors
		/// <summary>
		/// Gets the uploads associated with the given parent id.
		/// </summary>
		/// <param name="id">The parent id</param>
		/// <returns>A list of uploads</returns>
		public static List<Upload> GetByParentId(Guid id) {
			return Get("upload_parent_id = @0", id) ;
		}

		/// <summary>
		/// Gets a single upload associated with the given parent id.
		/// </summary>
		/// <param name="id">The parent id</param>
		/// <returns>The upload</returns>
		public static Upload GetSingleByParentId(Guid id) {
			List<Upload> ur = GetByParentId(id) ;
			if (ur.Count > 0)
				return ur[0] ;
			return null ;
		}

		/// <summary>
		/// Checks if there exists an upload for the given parent id.
		/// </summary>
		/// <param name="id">The parent id</param>
		/// <returns>If there exists an upload</returns>
		public static bool ExistsForParentId(Guid id) {
			return GetSingleByParentId(id) != null ;
			
		}
		#endregion

		/// <summary>
		/// Gets the physical media related to the upload and writes it to
		/// the given http response.
		/// </summary>
		/// <param name="response">The http response</param>
		public void GetFile(HttpResponse response) {
			WriteFile(response, PhysicalPath) ;
		}

		/// <summary>
		/// Saves the record to the database.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Weather the operation succeeded or not</returns>
		public override bool Save(System.Data.IDbTransaction tx = null) {
			if (IsNew) {
				Created   = DateTime.Now ;
				CreatedBy = new Guid(HttpContext.Current.User.Identity.Name) ;
			}
			return base.Save(tx) ;
		}

		#region Private methods
		/// <summary>
		/// Writes the given file to the http response
		/// </summary>
		/// <param name="response"></param>
		/// <param name="path"></param>
		private void WriteFile(HttpResponse response, string path) {
			if (File.Exists(path)) {
				response.StatusCode = 200 ;
				response.ContentType = Type ;
				response.WriteFile(path) ;
				response.End() ;
			} else {
				response.StatusCode = 404 ;
			}
		}
		#endregion
	}
}
