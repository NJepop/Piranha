using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Web;
using System.Windows.Forms;

using Piranha.Models;

namespace Piranha.WebPages
{
    public class WebThumb
    {
		#region Inner classes
		private class WebThumbEngine
		{
			#region Members
			public Bitmap Bitmap { get ; private set ; }
			public string Url { get ; set ; }
            public int Width { get ; set ; }
            public int Height { get ; set ; }

            private const int BrowserWidth = 1024 ;
            private const int BrowserHeight = 768 ;
			#endregion

			/// <summary>
			/// Default constructor.
			/// </summary>
			public WebThumbEngine() {}

			/// <summary>
			/// Starts a new thread and returns the generated bitmap after it completes.
			/// </summary>
			/// <returns>The thumbnail</returns>
            public Bitmap GetThumb() {
                ThreadStart start = new ThreadStart(Generate);
                Thread thread = new Thread(start) ;

                thread.SetApartmentState(ApartmentState.STA) ;
                thread.Start() ;
                thread.Join() ;

                return Bitmap ;
            }

			/// <summary>
			/// Generates the thumbnail.
			/// </summary>
            private void Generate() {
                WebBrowser browser = new WebBrowser() { ScrollBarsEnabled = false } ;

				browser.Navigate(Url) ;
                browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(DocumentCompleted) ;

                while (browser.ReadyState != WebBrowserReadyState.Complete)
                    Application.DoEvents();
                browser.Dispose();
            }

			/// <summary>
			/// Generates the thumbnail after the web request has completed.
			/// </summary>
			/// <param name="sender">The browser</param>
			/// <param name="e">Event arguments</param>
            private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
                WebBrowser browser = (WebBrowser)sender ;

                browser.ClientSize        = new Size(BrowserWidth, BrowserHeight) ;
                browser.ScrollBarsEnabled = false ;

                Bitmap = new Bitmap(browser.Bounds.Width, browser.Bounds.Height) ;
                browser.BringToFront();
                browser.DrawToBitmap(Bitmap, browser.Bounds);
                
                if (Width != 0 && Height != 0)
					Bitmap = (Bitmap)Bitmap.GetThumbnailImage(Width, Height, null, IntPtr.Zero);                
            }
		}
		#endregion

		/// <summary>
		/// Generates the thumbnail for the specifed page id.
		/// </summary>
		/// <param name="response">The http response</param>
		/// <param name="id">The page id</param>
		/// <param name="url">The url</param>
		/// <param name="width">Thumbnail width</param>
		/// <param name="height">Thumbnail height</param>
		/// <returns>The thumbnail</returns>
		public static void GetThumbnail(HttpResponse response, Guid id, string url, int width, int height) {
			Bitmap bmp = null ;

			if (HasPagePreview(id)) {
				bmp = (Bitmap)Bitmap.FromFile(GetPagePreviewPath(id)) ;
			} else {
				bmp = new WebThumbEngine() { 
					Url    = url, 
					Width  = width, 
					Height = height }.GetThumb() ;
				bmp.Save(GetPagePreviewPath(id)) ;
			}

			if (bmp != null) {
				response.StatusCode = 200 ;
				response.ContentType = "image/jpeg" ;
				bmp.Save(response.OutputStream, ImageFormat.Jpeg) ;
				response.End() ;
			} else {
				response.StatusCode = 404 ;
			}
        }

		public static string GetPagePreviewPath(Guid id) {
			return HttpContext.Current.Server.MapPath("~/App_Data/Cache/Previews/" + id.ToString()) ;
		}

		public static bool HasPagePreview(Guid id) {
			return File.Exists(GetPagePreviewPath(id)) ;
		}

		public static void RemovePagePreview(Guid id) {
			if (HasPagePreview(id))
				File.Delete(GetPagePreviewPath(id)) ;
		}
	
		/// <summary>
		/// Generates the thumbnail for the specifed url with the given size.
		/// </summary>
		/// <param name="url">The url</param>
		/// <param name="width">Thumbnail width</param>
		/// <param name="height">Thumbnail height</param>
		/// <returns>The thumbnail</returns>
		public static Bitmap GetThumbnail(string url, int width, int height) {
			return new WebThumbEngine() { 
				Url    = url, 
				Width  = width, 
				Height = height }.GetThumb() ;
        }
	}
}