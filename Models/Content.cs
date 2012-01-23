using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using Piranha.Data;

namespace Piranha.Models
{
	/// <summary>
	/// A content object is an image, document or other file that's been uploaded from
	/// the manager area. Content uploaded from the application is usually handled using
	/// the Upload record.
	/// </summary>
	[PrimaryKey(Column="content_id")]
	public class Content : PiranhaRecord<Content>, ICacheRecord<Content>
	{
		#region Fields
		/// <summary>
		/// Gets/sets the id.
		/// </summary>
		[Column(Name="content_id")]
		[Required()]
		public override Guid Id { get ; set ; }

		/// <summary>
		/// Gets/sets the filename.
		/// </summary>
		[Column(Name="content_filename")]
		[Display(Name="Filnamn")]
		public string Filename { get ; set ; }

		/// <summary>
		/// Gets/sets the type
		/// </summary>
		[Column(Name="content_type")]
		[Display(Name="Typ")]
		public string Type { get ; set ; }

		/// <summary>
		/// Gets/sets the content size.
		/// </summary>
		public int Size { get ; set ; }

		/// <summary>
		/// Get/sets weather the content is an image or not.
		/// </summary>
		public bool IsImage { get ; set ; }

		/// <summary>
		/// Gets/sets the possible width of the content.
		/// </summary>
		public int Width { get ; set ; }

		/// <summary>
		/// Gets/sets the possible height of th econtent.
		/// </summary>
		public int Height { get ; set ; }

		/// <summary>
		/// Gets/sets the alternate text.
		/// </summary>
		[Column(Name="content_alt")]
		[Display(Name="Alt. text")]
		public string AlternateText { get ; set ; }

		/// <summary>
		/// Gets/sets the decription.
		/// </summary>
		[Column(Name="content_description")]
		[Display(Name="Beskrivning")]
		public string Description { get ; set ; }

		/// <summary>
		/// Gets/sets the created date.
		/// </summary>
		[Column(Name="content_created")]
		public override DateTime Created { get ; set ; }

		/// <summary>
		/// Gets/sets the updated date.
		/// </summary>
		[Column(Name="content_updated")]
		public override DateTime Updated { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="content_created_by")]
		public override Guid CreatedBy { get ; set ; }

		/// <summary>
		/// Gets/sets the user id that created the record.
		/// </summary>
		[Column(Name="content_updated_by")]
		public override Guid UpdatedBy { get ; set ; }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the virtual path for the content media file.
		/// </summary>
		public string VirtualPath { 
			get { return "~/App_Data/Content/" + Id ; }
		}

		/// <summary>
		/// Gets the physical path for the content media file.
		/// </summary>
		public string PhysicalPath {
			get { return HttpContext.Current.Server.MapPath(VirtualPath) ; }
		}

		/// <summary>
		/// Gets the page cache object.
		/// </summary>
		private static Dictionary<Guid, Content> Cache {
			get {
				if (HttpContext.Current.Cache[typeof(Content).Name] == null)
					HttpContext.Current.Cache[typeof(Content).Name] = new Dictionary<Guid, Content>() ;
				return (Dictionary<Guid, Content>)HttpContext.Current.Cache[typeof(Content).Name] ;
			}
		}
		#endregion

		#region Static accessors
		/// <summary>
		/// Gets a single record.
		/// </summary>
		/// <param name="id">The record id</param>
		/// <returns>The record</returns>
		public static Content GetSingle(Guid id) {
			if (!Cache.ContainsKey(id))
				Cache[id] = Content.GetSingle((object)id) ;
			return Cache[id] ;
		}

		/// <summary>
		/// Gets all content attached to the given page id.
		/// </summary>
		/// <param name="id">The page id</param>
		/// <returns>A list of content elements</returns>
		public static List<Content> GetByPageId(Guid id) {
			return Content.Get("content_id IN " +
				"(SELECT attachment_content_id FROM attachment WHERE attachment_parent_id = @0)", id) ;
		}
		#endregion

		/// <summary>
		/// Gets the physical media related to the content record and writes it to
		/// the given http response.
		/// </summary>
		/// <param name="response">The http response</param>
		public void GetMedia(HttpResponse response) {
			WriteFile(response, PhysicalPath) ;
		}

		/// <summary>
		/// Gets a thumbnail representing the current content file.
		/// </summary>
		/// <param name="response">The http response</param>
		/// <param name="size">The desired size</param>
		public void GetThumbnail(HttpResponse response, int size = 60) {
			if (File.Exists(CachedThumbnailPath(size))) {
				// Return generated & cached thumbnail
				WriteFile(response, CachedThumbnailPath(size)) ;
			} else if (File.Exists(PhysicalPath)) {
				string[] segments = Filename.ToLower().Split(new char[] { '.' }) ;
				
				Image img = null ; 
				try {
					img = Image.FromFile(PhysicalPath) ;
				} catch {}
				
				if (img != null) {
					// Generate thumbnail from image
					using (Bitmap bmp = new Bitmap(size, size)) {
						Graphics grp = Graphics.FromImage(bmp) ;

						grp.SmoothingMode = SmoothingMode.HighQuality ;
						grp.CompositingQuality = CompositingQuality.HighQuality ;
						grp.InterpolationMode = InterpolationMode.High ;

						// Resize and crop image
						Rectangle dst = new Rectangle(0, 0, bmp.Width, bmp.Height) ;
						grp.DrawImage(img, dst, img.Width > img.Height ? (img.Width - img.Height) / 2 : 0,
							img.Height > img.Width ? (img.Height - img.Width) / 2 : 0, Math.Min(img.Width, img.Height), 
							Math.Min(img.Height, img.Width), GraphicsUnit.Pixel) ;

						bmp.Save(CachedThumbnailPath(size)) ;
					}
					WriteFile(response, CachedThumbnailPath(size)) ;
				} else {
					// TODO: Generate thumbnail for non-images
				}
			}
		}

		/// <summary>
		/// Deletes all cached versions of the content media.
		/// </summary>
		public void DeleteCache() {
			DirectoryInfo dir = new DirectoryInfo(CacheDir()) ;

			foreach (FileInfo file in dir.GetFiles(Id.ToString() + "*")) 
				file.Delete() ;
		}

		/// <summary>
		/// Deletes the current record.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>Weather the operation succeeded or not</returns>
		public override bool Delete(System.Data.IDbTransaction tx = null) {
			bool ret = base.Delete(tx) ;
			if (ret)
				DeleteCache() ;
			return ret ;
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

		/// <summary>
		/// Gets the physical dir for the content cache.
		/// </summary>
		/// <returns>The path</returns>
		private string CacheDir() {
			return HttpContext.Current.Server.MapPath("~/App_Data/Cache/Content/") ;
		}
		
		/// <summary>
		/// Gets the physical path for the cached file with the given name.
		/// </summary>
		/// <param name="size">Thumbnail size</param>
		/// <returns>The physical cache path</returns>
		private string CachedThumbnailPath(int size) {
			return CacheDir() + Id.ToString() + "-" + size.ToString() ;
		}

		/// <summary>
		/// Gets the physical path for the cached file with the given dimensions.
		/// </summary>
		/// <param name="width">The image width</param>
		/// <param name="height">The image height</param>
		/// <returns>The physical path</returns>
		private string CachedImagePath(int width, int height) {
			return CacheDir() + Id.ToString() + "-" + width.ToString() + "x" + height.ToString() ;
		}
		#endregion

		/// <summary>
		/// Invalidates the cache for the given record.
		/// </summary>
		/// <param name="record">The record</param>
		public void InvalidateRecord(Content record) {
			if (Cache.ContainsKey(record.Id))
				Cache.Remove(record.Id) ;
		}
	}
}
