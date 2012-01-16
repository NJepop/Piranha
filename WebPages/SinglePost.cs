using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Piranha.Models;

namespace Piranha.WebPages
{
	/// <summary>
	/// Standard page class for a single page.
	/// </summary>
	public abstract class SinglePost : SinglePost<PostModel> {}

	/// <summary>
	/// Page class for a single page where the model is of the generic type T.
	/// </summary>
	/// <typeparam name="T">The model type</typeparam>
	public abstract class SinglePost<T> : ContentPage<T> where T : PostModel {
		/// <summary>
		/// Initializes the web page
		/// </summary>
		protected override void InitializePage() {
			string permalink = UrlData.Count > 0 ? UrlData[UrlData.Count - 1] : "" ;

			// Load the current page model
			if (!String.IsNullOrEmpty(permalink))
				InitModel(PostModel.GetByPermalink<T>(permalink)) ;

			HandleCache() ;

			base.InitializePage() ;
		}

		/// <summary>
		/// Generates the unique entity tag for the page.
		/// </summary>
		/// <param name="modified">Last modified date</param>
		/// <returns>The entity tag</returns>
		protected override string GenerateETag(DateTime modified) {
			UTF8Encoding encoder = new UTF8Encoding() ;
			MD5CryptoServiceProvider crypto = new MD5CryptoServiceProvider() ;

			string str = Model.Post.Id.ToString() + modified.ToLongTimeString() ;
			byte[] bts = crypto.ComputeHash(encoder.GetBytes(str)) ;
			return Convert.ToBase64String(bts, 0, bts.Length);
		}

		/// <summary>
		/// Gets the last modification date for the page.
		/// </summary>
		/// <returns>The modification date</returns>
		protected override DateTime GetLastModified() {
			return Model.Post.Updated ;
		}

		#region Private methods
		/// <summary>
		/// Initializes the instance from the given model.
		/// </summary>
		/// <param name="pm">The page model</param>
		protected virtual void InitModel(T pm) {
			Model = pm ;

			Page.Current = null ;
		}
		#endregion
	}
}
