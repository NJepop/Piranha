using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using Piranha.Data;
using Piranha.Models;

namespace byBrick.Areas.Manager.Controllers
{
	public class InstallModel {
		[Required(ErrorMessage="Du måste välja ett användarnamn.")]
		public string UserLogin { get ; set ; }

		[Required(ErrorMessage="Du måste ange en e-post adress.")]
		public string UserEmail { get ; set ; }

		[Required(ErrorMessage="Du måste ange ett lösenord")]
		public string Password { get ; set ; }

		[Compare("Password", ErrorMessage="Lösenorden matchar inte.")]
		public string PasswordConfirm { get ; set ; }

		public string InstallType { get ; set ; }
	}

	/// <summary>
	/// Login controller for the manager interface.
	/// </summary>
    public class InstallController : Controller
    {
		/// <summary>
		/// Default action
		/// </summary>
        public ActionResult Index() {
			// Check for existing installation.
			try {
				SysParam p = SysParam.GetByName("SITE_VERSION") ;
				
				// If we got here we have a database. TODO: Check version and go to
				// update page if needed.
				return RedirectToAction("Index", "Account") ;
			} catch {}
            return View("Index");
        }

		/// <summary>
		/// Creates a new site installation.
		/// </summary>
		/// <param name="m">The model</param>
		[HttpPost()]
		public ActionResult Create(InstallModel m) {
			if (m.InstallType == "SCHEMA" || ModelState.IsValid) {
				// Read embedded create script
				Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("Piranha.Data.Scripts.Create.sql") ;
				String sql = new StreamReader(str).ReadToEnd() ;
				str.Close() ;

				// Read embedded data script
				str = Assembly.GetExecutingAssembly().GetManifestResourceStream("Piranha.Data.Scripts.Data.sql") ;
				String data = new StreamReader(str).ReadToEnd() ;
				str.Close() ;

				// Split statements and execute
				string[] stmts = sql.Split(new char[] { ';' }) ;
				using (IDbTransaction tx = Database.OpenTransaction()) {
					// Create database from script
					foreach (string stmt in stmts) {
						if (!String.IsNullOrEmpty(stmt.Trim()))
							SysUser.Execute(stmt, tx) ;
					}
					tx.Commit() ;
				}

				if (m.InstallType.ToUpper() == "FULL") {
					// Split statements and execute
					stmts = data.Split(new char[] { ';' }) ;
					using (IDbTransaction tx = Database.OpenTransaction()) {
						// Create database from script
						foreach (string stmt in stmts) {
							if (!String.IsNullOrEmpty(stmt.Trim()))
								SysUser.Execute(stmt, tx) ;
						}

						// Create user
						SysUser usr = new SysUser() {
							Login = m.UserLogin,
							Email = m.UserEmail,
							GroupId = new Guid("7c536b66-d292-4369-8f37-948b32229b83"),
							CreatedBy = new Guid("ca19d4e7-92f0-42f6-926a-68413bbdafbc"),
							UpdatedBy = new Guid("ca19d4e7-92f0-42f6-926a-68413bbdafbc"),
							Created = DateTime.Now,
							Updated = DateTime.Now
						} ;
						usr.Save(tx) ;

						// Create user password
						SysUserPassword pwd = new SysUserPassword() {
							Id = usr.Id,
							Password = m.Password,
							IsNew = false
						} ;
						pwd.Save(tx) ;
		
						tx.Commit() ;
					}	
				}
				return RedirectToAction("Index", "Account") ;
			}
			return Index() ;
		}
    }
}
