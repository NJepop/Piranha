using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Piranha.Data
{
	#region Attributes
	/// <summary>
	/// Attribute for defining the table for a record.
	/// </summary>
	public class TableAttribute : Attribute {
		/// <summary>
		/// The table name.
		/// </summary>
		public string Name { get ; set ; }
	}

	/// <summary>
	/// Attribute for defining a primary key for a record.
	/// </summary>
	public class PrimaryKeyAttribute : Attribute {
		/// <summary>
		/// The primary key column.
		/// </summary>
		public string Column { get ; set ; }
	}

	/// <summary>
	/// Attribute used to mark a property as a database column for an active record.
	/// </summary>
	public class ColumnAttribute : Attribute
	{
		#region Properties
		/// <summary>
		/// Gets/sets the optional field name.
		/// </summary>
		public string Name { get ; set ; }

		/// <summary>
		/// Gets/sets the table this column belongs to if it's joined.
		/// </summary>
		public string Table { get ; set ; }

		/// <summary>
		/// Gets/sets weather the field is read only.
		/// </summary>
		public bool ReadOnly { get ; set ; }
		
		/// <summary>
		/// Gets/sets method to invoke on load.
		/// </summary>
		public string OnLoad { get ; set ; }

		/// <summary>
		/// Gets/sets method to invoke on save.
		/// </summary>
		public string OnSave { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ColumnAttribute() : base() {
			ReadOnly = false ;
		}
	}

	/// <summary>
	/// Attribute used to join tables together for an active record.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple=true)]
	public class JoinAttribute : Attribute
	{
		/// <summary>
		/// Gets/sets the table to join.
		/// </summary>
		public string TableName { get ; set ; }

		/// <summary>
		/// Gets/sets the primary key.
		/// </summary>
		public string PrimaryKey { get ; set ; }

		/// <summary>
		/// Gets/sets the foreign key.
		/// </summary>
		public string ForeignKey { get ; set ; }
	}
	#endregion

	#region Interfaces
	/// <summary>
	/// Interface for active records that are cahced.
	/// </summary>
	/// <typeparam name="T">The record type</typeparam>
	public interface ICacheRecord<T> {
		/// <summary>
		/// Invalidates the given record from the current cache. The method is called
		/// after save and delete.
		/// </summary>
		/// <param name="record"></param>
		void InvalidateRecord(T record) ;
	}
	#endregion

	/// <summary>
	/// Generic implementation of the Active Record pattern. The class uses the DataHandler
	/// for communication with the database.
	/// </summary>
	/// <typeparam name="T">The record type</typeparam>
	public abstract class ActiveRecord<T> : IDisposable
	{
		#region Members
		// Reflected information
		private static string _tablename ;
		private static List<string> _primarykey ;
		private static string _joins ;
		private static string _fields ;
		private static Dictionary<string, PropertyInfo> _columns ;
		private static Dictionary<string, ColumnAttribute> _attributes ;

		// SQL statements 
		private const string SqlSelect = "SELECT {3} {0} FROM {1} {2} {4} {5}" ;
		private const string SqlInsert = "INSERT INTO {0} ({1}) VALUES({2})" ;
		private const string SqlUpdate = "UPDATE {0} SET {1} WHERE {2}" ;
		private const string SqlDelete = "DELETE FROM {0} WHERE {1}" ;
		#endregion

		#region Properties
		/// <summary>
		/// Gets/sets wether this object is new or loaded from the database.
		/// </summary>
		public bool IsNew { get ; set ; }
		#endregion

		#region Static properties
		/// <summary>
		/// Gets the table name.
		/// </summary>
		protected static string TableName { 
			get {
				if (String.IsNullOrEmpty(_tablename)) {
					TableAttribute ar = typeof(T).GetCustomAttribute<TableAttribute>(true) ;
					if (ar != null && !String.IsNullOrEmpty(ar.Name))
						_tablename = ar.Name ;
					else _tablename = typeof(T).Name.ToLower() ;
				}
				return _tablename ;
			}
		}

		/// <summary>
		/// Gets all of the fields to select.
		/// </summary>
		protected static string AllFields {
			get {
				if (String.IsNullOrEmpty(_fields))
					_fields = GenerateSelectFields() ;
				return _fields ;
			}
		}

		/// <summary>
		/// Gets the primary keys.
		/// </summary>
		protected static List<string> PrimaryKeys {
			get {
				if (_primarykey == null)
					InitPrimaryKeys() ;
				return _primarykey ;
			}
		}

		/// <summary>
		/// Gets the table joins.
		/// </summary>
		protected static string TableJoins {
			get {
				if (_joins == null) {
					_joins = "" ;
					JoinAttribute[] ja = typeof(T).GetCustomAttributes<JoinAttribute>(true) ;
					if (ja != null) {
						foreach (var join in ja)
							_joins += " JOIN " + join.TableName + " ON " + TableName + "." + join.ForeignKey + "=" +
								join.TableName + "." + join.PrimaryKey ;
					}
				}
				return _joins ;
			} 
		}

		/// <summary>
		/// Gets the properties that should be loaded from the database.
		/// </summary>
		protected static Dictionary<string, PropertyInfo> Columns { 
			get {
				if (_columns == null)
					InitColumns() ;
				return _columns ;
			} 
		}

		/// <summary>
		/// Gets the property attributes.
		/// </summary>
		protected static Dictionary<string, ColumnAttribute> Attributes { 
			get {
				if (_attributes == null)
					InitColumns() ;
				return _attributes ;
			} 
		}
		#endregion

		/// <summary>
		/// Default constructor. Creates a new active record.
		/// </summary>
		protected ActiveRecord() {
			IsNew = true;
		}

		/// <summary>
		/// Saves the current object to the database.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>If the operation succeeded</returns>
		public virtual bool Save(IDbTransaction tx = null) {
			bool result = false ;

			// Check primary key
			foreach (string key in PrimaryKeys)
				if (Columns[key].GetValue(this, null) == null)
					throw new ArgumentException("Property \"" +  Columns[key].Name + "\" is marked as Primary Key and can not contain null") ;

			// Execute command
			using (IDbConnection conn = tx != null ? null : Database.OpenConnection()) {
				if (IsNew) {
					using (IDbCommand cmd = CreateInsertCommand(tx != null ? tx.Connection : conn, tx)) {
						result = cmd.ExecuteNonQuery() > 0 ;
						IsNew = !result ;
					}
				} else {
					using (IDbCommand cmd = CreateUpdateCommand(tx != null ? tx.Connection : conn, tx)) {
						result = cmd.ExecuteNonQuery() > 0 ;
					}
				}
			}
			// Check for cache interface
			if (this is ICacheRecord<T>)
				((ICacheRecord<T>)this).InvalidateRecord((T)((object)this)) ;
			return result ;
		}

		/// <summary>
		/// Deletes the current object from the database.
		/// </summary>
		/// <param name="tx">Optional transaction</param>
		/// <returns>If the operation succeeded</returns>
		public virtual bool Delete(IDbTransaction tx = null) {
			if (!IsNew) {
				//object id = Columns[PrimaryKey].GetValue(this, null) ;
				bool result = false ;
	
				// Execute command
				using (IDbConnection conn = tx != null ? null : Database.OpenConnection()) {
					using (IDbCommand cmd = CreateDeleteCommand(tx != null ? tx.Connection : conn, tx)) {
						result = cmd.ExecuteNonQuery() > 0 ;
						IsNew = result ;
					}
				}
				// Check for cache interface
				if (this is ICacheRecord<T>)
					((ICacheRecord<T>)this).InvalidateRecord((T)((object)this)) ;
				return result ;
			}
			return false ;
		}

		#region Static accessors
		/// <summary>
		/// Gets a single matching record.
		/// </summary>
		/// <param name="id">The primary key</param>
		/// <returns>A single record</returns>
		public static T GetSingle(object id) {
			return GetSingle(PrimaryKeys[0] + "=@0", id) ;
		}

		/// <summary>
		/// Gets a single record matching the given input.
		/// </summary>
		/// <param name="where">Where clause</param>
		/// <param name="args">Optional where parameters</param>
		/// <returns>A matching record.</returns>
		public static T GetSingle(string where, params object[] args) {	
			List<T> result = GetFields(AllFields, where, args) ;
			if (result.Count > 0)
				return result[0] ;
			return default(T) ;
		}

		/// <summary>
		/// Gets the available records with the given query params.
		/// </summary>
		/// <param name="param">The query params</param>
		/// <returns>A list of records</returns>
		public static List<T> Get(Params param) {
			return GetFields(AllFields, "", param) ;
		}

		/// <summary>
		/// Gets the available records matching the given input.
		/// </summary>
		/// <param name="where">Optional where clause</param>
		/// <param name="args">Optional where parameters</param>
		/// <returns>A list of records</returns>
		public static List<T> Get(string where = "", params object[] args) {
			return GetFields(AllFields, where, args) ;
		}

		/// <summary>
		/// Gets all available records with the given query params.
		/// </summary>
		/// <param name="fields">The fields to load</param>
		/// <param name="param">The query params</param>
		/// <returns>A list of records</returns>
		public static List<T> GetFields(string fields, Params param) {
			return GetFields(fields, "", param) ;
		}

		/// <summary>
		/// Gets all available records matching the given input.
		/// </summary>
		/// <param name="fields">The fields to load</param>
		/// <param name="where">Optional where clause</param>
		/// <param name="args">Optional where parameters</param>
		/// <returns>A list of records</returns>
		public static List<T> GetFields(string fields, string where = "", params object[] args) {
			Params gp = args.Length > 0 && args[args.Length - 1] is Params ? (Params)args[args.Length - 1] : null ;

			return Query(String.Format(SqlSelect, fields, TableName + TableJoins, 
				where != "" ? "WHERE " + where : "", gp != null && gp.Distinct ? "DISTINCT" : "", 
				gp != null && !String.IsNullOrEmpty(gp.GroupBy) ? "GROUP BY " + gp.GroupBy : "",
				gp != null && !String.IsNullOrEmpty(gp.OrderBy) ? "ORDER BY " + gp.OrderBy : ""), args) ;
		}

		/// <summary>
		/// Executes the given statement and returns a scalar result.
		/// </summary>
		/// <param name="statement">The sql statement</param>
		/// <param name="args">Statement parameters</param>
		/// <returns>The scalar value</returns>
		public static int GetScalar(string statement, params object[] args) {
			int result = 0 ;

			// Execute statement
			using (IDbConnection conn = Database.OpenConnection()) {
				using (IDbCommand cmd = CreateCommand(conn, statement, null, args)) {
					result = Convert.ToInt32(cmd.ExecuteScalar()) ;
				}
			}
			return result ;
		}

		/// <summary>
		/// Executes the given query and returns the matching records.
		/// </summary>
		/// <param name="query">The query to run</param>
		/// <param name="args">Optional query parameters</param>
		/// <returns>A list of records</returns>
		public static List<T> Query(string query, params object[] args) {
			List<T> result = new List<T>() ;

			using (IDbConnection conn = Database.OpenConnection()) {
				using (IDbCommand cmd = Database.CreateCommand(conn, query, args)) {
					using (IDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection)) { 
						while (rdr.Read()) {
							// Create and fill object
							object o = Activator.CreateInstance<T>() ;
							for (int n = 0;  n < rdr.FieldCount; n++) {
								if (Columns.ContainsKey(rdr.GetName(n))) {
									object val  = rdr[n] != DBNull.Value ? rdr[n] : null ;
									string name = rdr.GetName(n) ;

									// Check if the property is marked with the "OnLoad" property
									if (!String.IsNullOrEmpty(Attributes[name].OnLoad)) {
										MethodInfo m = o.GetType().GetMethod(Attributes[name].OnLoad, 
											BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance) ;
										val = m.Invoke(o, new object[] { val }) ;
									} else {
										// Automatically convert strings to HtmlString if needed.
										if (Columns[name].PropertyType == typeof(HtmlString))
											val = new HtmlString(Convert.ToString(val)) ;
										else if (typeof(Enum).IsAssignableFrom(Columns[name].PropertyType))
											val = Enum.Parse(Columns[name].PropertyType, (string)val) ;
									}
									Columns[name].SetValue(o, val, null) ;
								}
							}
							// Set correct object state
							if (o is ActiveRecord<T>)
								((ActiveRecord<T>)o).IsNew = false ;
							result.Add((T)o); 
						}
					}
				}
			}
			return result ;
		}

		/// <summary>
		/// Executes the given statement and returns the number of affected rows.
		/// </summary>
		/// <param name="statement">The sql statement</param>
		/// <param name="tx">Optional transaction</param>
		/// <param name="args">Statement parameters</param>
		/// <returns>The number of affected rows</returns>
		public static int Execute(string statement, IDbTransaction tx = null, params object[] args) {
			int result = 0 ;

			// Execute statement
			using (IDbConnection conn = tx != null ? null : Database.OpenConnection()) {
				using (IDbCommand cmd = CreateCommand(tx != null ? tx.Connection : conn, statement, tx, args)) {
					result = cmd.ExecuteNonQuery() ;
				}
			}
			return result ;
		}
		#endregion

		#region IDisposable Members
		/// <summary>
		/// Dispose the current record.
		/// </summary>
		public virtual void Dispose() {}
		#endregion

		#region Private members
		/// <summary>
		/// Initializes the column information from the associated attributes.
		/// </summary>
		private static void InitColumns() {
			_columns = new Dictionary<string, PropertyInfo>() ;
			_attributes = new Dictionary<string, ColumnAttribute>() ;

			PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.FlattenHierarchy) ;
			foreach (PropertyInfo prop in props) {
				ColumnAttribute af = prop.GetCustomAttribute<ColumnAttribute>(true) ;

				if (af != null) {
					_columns.Add(!String.IsNullOrEmpty(af.Name) ? af.Name : prop.Name, prop) ;
					_attributes.Add(!String.IsNullOrEmpty(af.Name) ? af.Name : prop.Name, af) ;
				}
			}
		}

		/// <summary>
		/// Initializes the primary keys
		/// </summary>
		private static void InitPrimaryKeys() {
			_primarykey = new List<string>() ;

			PrimaryKeyAttribute pa = typeof(T).GetCustomAttribute<PrimaryKeyAttribute>(true) ;
			if (pa != null && !String.IsNullOrEmpty(pa.Column))
				_primarykey.AddRange(pa.Column.Split(new char[] {','})) ;
			else _primarykey.Add("Id") ;
		}

		/// <summary>
		/// Generates the fields string.
		/// </summary>
		/// <returns>The database fields</returns>
		private static string GenerateSelectFields() {
			string ret = "" ;

			foreach (string key in Columns.Keys) {
				ColumnAttribute col = Attributes[key] ;

				if (!key.StartsWith("(")) {
					if (!String.IsNullOrEmpty(col.Table))
						ret += (ret != "" ? "," : "") + col.Table + "." + key ;
					else ret += (ret != "" ? "," : "") + TableName + "." + key ;
				} else ret += (ret != "" ? "," : "") + key ;
			}
			return ret ;
		}

		/// <summary>
		/// Creates a database command to insert the database with the given record.
		/// </summary>
		/// <param name="conn">The current database connection</param>
		/// <param name="tx">Optional transaction</param>
		/// <returns>The command</returns>
		private IDbCommand CreateInsertCommand(IDbConnection conn, IDbTransaction tx = null) {
			List<object> args = new List<object>() ;

			// Build strings
			string fields = "", values = "" ;
			foreach (string key in Columns.Keys) {
				// Exclude joined members
				if (!Attributes[key].ReadOnly) {
					fields += (fields != "" ? "," : "") + "[" + key + "]" ;
					values += (values != "" ? "," : "") + "@" + args.Count ;

					// If the ActiveField is marked with the OnSave property, process the value
					// before saving it.
					if (!String.IsNullOrEmpty(Attributes[key].OnSave)) {
						MethodInfo m = this.GetType().GetMethod(Attributes[key].OnSave, 
							BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance) ;
						args.Add(m.Invoke(this, new object[] { Columns[key].GetValue(this, null) })) ;
					} else {
						if (Columns[key].PropertyType == typeof(HtmlString) && Columns[key].GetValue(this, null) != null)
							args.Add(((HtmlString)Columns[key].GetValue(this, null)).ToHtmlString()) ;
						else if (typeof(Enum).IsAssignableFrom(Columns[key].PropertyType))
							args.Add(Columns[key].GetValue(this, null).ToString()) ;
						else args.Add(Columns[key].GetValue(this, null)) ;
					}
				}
			}

			// Create command
			IDbCommand cmd = Database.CreateCommand(conn, String.Format(SqlInsert, 
				TableName, fields, values), args.ToArray()) ;
			if (tx != null)
				cmd.Transaction = tx ;
			return cmd ;
		}

		/// <summary>
		/// Creates a database command to update the database with the given record.
		/// </summary>
		/// <param name="conn">The current database connection</param>
		/// <param name="record">The record to update</param>
		/// <param name="tx">Optional transaction to run the update in</param>
		/// <returns>The command</returns>
		private IDbCommand CreateUpdateCommand(IDbConnection conn, IDbTransaction tx = null) {
			List<object> args = new List<object>() ;

			// Build set statement
			string values = "" ;
			foreach (string key in Columns.Keys) {
				// Exclude joined members
				if (!Attributes[key].ReadOnly) {
					values += (values != "" ? "," : "") + "[" + key + "] = @" + args.Count.ToString() ;

					// Check if the ActiveField is marked with the OnSave property.
					if (!String.IsNullOrEmpty(Attributes[key].OnSave)) {
						MethodInfo m = this.GetType().GetMethod(Attributes[key].OnSave, 
							BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance) ;
						args.Add(m.Invoke(this, new object[] { Columns[key].GetValue(this, null) })) ;
					} else {
						if (Columns[key].PropertyType == typeof(HtmlString) && Columns[key].GetValue(this, null) != null)
							args.Add(((HtmlString)Columns[key].GetValue(this, null)).ToHtmlString()) ;
						else if (typeof(Enum).IsAssignableFrom(Columns[key].PropertyType))
							args.Add(Columns[key].GetValue(this, null).ToString()) ;
						else args.Add(Columns[key].GetValue(this, null)) ;
					}
				}
			}

			// Build where clause
			string where = "" ; //PrimaryKey + "=@" + args.Count.ToString() ;
			foreach (string pk in PrimaryKeys) {
				where += (where != "" ? " AND " : "") +
					pk + "=@" + args.Count.ToString() ;
					args.Add(Columns[pk].GetValue(this, null)) ;
			}
			//args.Add(Columns[PrimaryKey].GetValue(this, null)) ;

			// Create command
			IDbCommand cmd = Database.CreateCommand(conn, String.Format(SqlUpdate, 
				TableName, values, where), args.ToArray()) ;
			if (tx != null)
				cmd.Transaction = tx ;
			return cmd ;
		}

		/// <summary>
		/// Creates a database command to delete the specified record from the database.
		/// </summary>
		/// <param name="conn">The current database connection</param>
		/// <param name="id">The id of the record to delete</param>
		/// <param name="tx">Optional transaction to run the insert in</param>
		/// <returns>The command</returns>
		private IDbCommand CreateDeleteCommand(IDbConnection conn, IDbTransaction tx = null) {
			// Build where clause
			List<object> args = new List<object>() ;
			string where = "" ;
			foreach (string pk in PrimaryKeys) {
				where += (where != "" ? " AND " : "") +
					pk + "=@" + args.Count.ToString() ;
					args.Add(Columns[pk].GetValue(this, null)) ;
			}

			// Create command
			IDbCommand cmd = Database.CreateCommand(conn, String.Format(SqlDelete,
				TableName, where), args.ToArray()) ;
			if (tx != null)
				cmd.Transaction = tx ;
			return cmd ;
		}

		/// <summary>
		/// Creates a generic command from the given statement and arguments
		/// </summary>
		/// <param name="conn">The current connection</param>
		/// <param name="statement">The sql statement</param>
		/// <param name="tx">Database transaction, can be null</param>
		/// <param name="args">The command arguments</param>
		/// <returns>The command</returns>
		private static IDbCommand CreateCommand(IDbConnection conn, string statement, IDbTransaction tx, object[] args) {
			// Create command
			IDbCommand cmd = Database.CreateCommand(conn, statement, args) ;
			if (tx != null)
				cmd.Transaction = tx ;
			return cmd ;
		}
		#endregion
	}

	/// <summary>
	/// Class used to pass in extra parameters to the get operations available in ActiveRecord.
	/// </summary>
	public class Params
	{
		#region Properties
		/// <summary>
		/// Gets/sets wether the get operation is distinct or not.
		/// </summary>
		public bool Distinct { get ; set ; }

		/// <summary>
		/// Gets/sets order statement.
		/// </summary>
		public string OrderBy { get ; set ; }

		/// <summary>
		/// Gets/sets grouping statement
		/// </summary>
		public string GroupBy { get ; set ; }
		#endregion

		/// <summary>
		/// Default constructor. Creates a new get parameter object.
		/// </summary>
		public Params() {
			Distinct = false ;
		}
	}
}
