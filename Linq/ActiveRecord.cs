using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Piranha.Linq
{
	/// <summary>
	/// Active record implementation based on linq to sql.
	/// </summary>
	/// <typeparam name="T">The record type</typeparam>
	public class ActiveRecord<T> where T : class
	{
		#region Inner classes
		/// <summary>
		/// Database helper.
		/// </summary>
		public static class DB
		{
			#region Members
			private static DataContext _context = null ;
			#endregion

			/// <summary>
			/// Gets the data context.
			/// </summary>
			public static DataContext Context {
				get { 
					// Create the context
					if (_context == null)
						_context = new DBContext(ConfigurationManager.ConnectionStrings["piranha"].ConnectionString) ;
					return _context ;
				}
			}
		}

		/// <summary>
		/// Extension of the standard LINQ data context.
		/// </summary>
		public class DBContext : DataContext {
			/// <summary>
			/// Default constructor. Creates a new context.
			/// </summary>
			/// <param name="fileOrServerConnection">file or server connection string</param>
			public DBContext(string fileOrServerConnection) : base(fileOrServerConnection) {}

			/// <summary>
			/// Submits all changes made.
			/// </summary>
			/// <param name="failureMode">The failure mode</param>
			public override void SubmitChanges(ConflictMode failureMode) {
				ChangeSet changes = GetChangeSet() ;

				// Call invalidate record for all ICacheRecord
				foreach (var del in changes.Deletes)
					if (del is ICacheRecord)
						((ICacheRecord)del).InvalidateRecord() ;
				foreach (var upt in changes.Updates)
					if (upt is ICacheRecord)
						((ICacheRecord)upt).InvalidateRecord() ;
				foreach (var ins in changes.Inserts)
					if (ins is ICacheRecord)
						((ICacheRecord)ins).InvalidateRecord() ;

				base.SubmitChanges(failureMode) ;
			}
		}

		/// <summary>
		/// Class for storing database fields.
		/// </summary>
		private class DBField {
			/// <summary>
			/// Gets/sets the column name of the field.
			/// </summary>
			public string Name { get ; set ; }

			/// <summary>
			/// Gets/sets the column property.
			/// </summary>
			public PropertyInfo Property { get ; set ; }
		}
		#endregion

		#region Members
		private static string _table = null ;
		private static List<DBField> _keys ;
		private static List<DBField> _fields ;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the table name.
		/// </summary>
		private static string TableName {
			get {
				if (_table == null)
					InitRecord() ;
				return _table ;
			}
		}

		/// <summary>
		/// Gets the primary keys.
		/// </summary>
		private static List<DBField> Keys {
			get {
				if (_keys == null)
					InitRecord() ;
				return _keys ;
			}
		}

		/// <summary>
		/// Gets the updatable fields.
		/// </summary>
		private static List<DBField> Fields {
			get { 
				if (_fields == null)
					InitRecord() ;
				return _fields ; 
			}
		}
		#endregion

		/// <summary>
		/// Gets the query object.
		/// </summary>
		/// <param name="ctx">Optional data context to use.</param>
		/// <returns>The query object</returns>
		public static Table<T> Get(DataContext ctx = null) {
			DataContext c = ctx != null ? ctx : DB.Context ;

			return c.GetTable<T>() ;
		}

		/// <summary>
		/// Saves the current record.
		/// </summary>
		/// <param name="ctx">Optional context. If it is provided SubmitChanges will not be called</param>
		public void Save(DataContext ctx = null) {
			DataContext c = ctx != null ? ctx : DB.Context ;
			T old = GetSelf(c) ;

			if (old != null) {
				foreach (var field in Fields)
					field.Property.SetValue(old, field.Property.GetValue(this, null), null) ;
			} else {
				c.GetTable<T>().InsertOnSubmit((T)(object)this) ;
			}
			if (ctx == null)
				c.SubmitChanges() ;
		}

		/// <summary>
		/// Deletes the current record.
		/// </summary>
		/// <param name="ctx">Optional context. If it is provided SubmitChanges will not be called</param>
		public void Delete(DataContext ctx = null) {
			DataContext c = ctx != null ? ctx : DB.Context ;
			T old = GetSelf(c) ;

			if (old != null) {
				c.GetTable<T>().DeleteOnSubmit(old) ;
				if (ctx == null)
					c.SubmitChanges() ;
			}
		}

		#region Private methods
		/// <summary>
		/// Gets the current object from the databas.
		/// </summary>
		/// <param name="ctx">The current datacontext.</param>
		/// <returns>The current object</returns>
		private T GetSelf(DataContext ctx) {
			string select = "SELECT * FROM {0} WHERE {1}" ;
			List<object> args = new List<object>() ;
			string where = "" ;

			// Build where clause
			for (int n = 0; n < Keys.Count; n++) {
				where += (n > 0 ? " AND " : "") + Keys[n].Name + "={" + n.ToString() + "}" ;
				args.Add(Keys[n].Property.GetValue(this, null)) ;
			}
			// Get record from context
			return ctx.ExecuteQuery<T>(String.Format(select, _table, where), 
				args.ToArray()).Take(1).ElementAtOrDefault(0) ;
		}

		/// <summary>
		/// Initializes the records static fields.
		/// </summary>
		private static void InitRecord() {
			// Create collections
			_keys   = new List<DBField>() ;
			_fields = new List<DBField>() ;

			// Get table name
			var tbl = typeof(T).GetType().GetCustomAttribute<TableAttribute>(true) ;
			_table = !String.IsNullOrEmpty(tbl.Name) ? tbl.Name : typeof(T).GetType().Name ;

			// Get primary keys and fields
			foreach (var prop in typeof(T).GetProperties()) {
				var col = prop.GetCustomAttribute<ColumnAttribute>(true) ;
				if (col != null && col.IsPrimaryKey)
					_keys.Add(new DBField() { 
						Name = !String.IsNullOrEmpty(col.Name) ? col.Name : prop.Name, 
						Property = prop }) ;
				else if (col != null)
					_fields.Add(new DBField() { 
						Name = !String.IsNullOrEmpty(col.Name) ? col.Name : prop.Name, 
						Property = prop }) ;
			}
		}
		#endregion
	}
}
