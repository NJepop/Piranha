using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Piranha.Data
{
	/// <summary>
	/// Helper class used to create an open and ready to use database connection using the DbProviderFactory
	/// and the connectionString section in the application settings.
	/// </summary>
	public class Database
	{
		#region Members
		/// <summary>
		/// Private static member to cache the current provider factory.
		/// </summary>
		private static DbProviderFactory _factory = null ;
		#endregion

		/// <summary>
		/// Gets the database connection from the current config.
		/// </summary>
		/// <param name="name">Optional name of the connection string to use</param>
		/// <returns>An open connection</returns>
		public static IDbConnection OpenConnection(string name = "default") {
			if (_factory == null)
				_factory = GetFactory(name) ;
			IDbConnection conn = GetConnection(name) ;
			conn.Open() ;
			return conn ;
		}

		/// <summary>
		/// Opens a connection and creates a transaction on it.
		/// </summary>
		/// <param name="name">Optional name of the connection string to use</param>
		/// <returns>An open transaction</returns>
		public static IDbTransaction OpenTransaction(string name = "default") {
			return OpenConnection().BeginTransaction() ;
		}

		/// <summary>
		/// Creates a command on the given connection and sql statement and fills it
		/// with the given parameters.
		/// </summary>
		/// <param name="conn">The connection</param>
		/// <param name="sql">The sql statement</param>
		/// <param name="args">The parameters</param>
		/// <returns>A db command</returns>
		public static IDbCommand CreateCommand(IDbConnection conn, string sql, object[] args = null) {
			IDbCommand cmd = conn.CreateCommand() ;
			cmd.CommandText = sql ;
			if (args != null)
				for (int n = 0; n < args.Length; n++) {
					if (!(args[n] is Params)) {
						IDbDataParameter p = cmd.CreateParameter() ;
						p.ParameterName = String.Format("@{0}", n) ; 
						if (args[n] == null || (args[n] is DateTime && ((DateTime)args[n]) == DateTime.MinValue)) {
							p.Value = DBNull.Value ;
						} else if (args[n] is Guid) {
							if (((Guid)args[n]) == Guid.Empty) {
								p.Value = DBNull.Value ;
							} else {
								p.Value = ((Guid)args[n]).ToString() ;
								p.DbType = DbType.String ;
							}
						} else {
							p.Value = args[n] ;
						}
						cmd.Parameters.Add(p) ;
					}
				}
			return cmd ;
		}

		#region Private methods
		/// <summary>
		/// Gets the current provider factory specified in the connection string section.
		/// </summary>
		/// <param name="name">The connection string name</param>
		/// <returns>A provider factory</returns>
		private static DbProviderFactory GetFactory(string name) {
			if (ConfigurationManager.ConnectionStrings[name] == null)
				throw new ConfigurationErrorsException("No connection string found with name \"" + name + "\"") ;
			return DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[name].ProviderName) ;
		}

		/// <summary>
		/// Gets a connection from the current provider factory.
		/// </summary>
		/// <param name="name">The name of the current connection string</param>
		/// <returns>A database connection</returns>
		private static IDbConnection GetConnection(string name) {
			IDbConnection conn = _factory.CreateConnection() ;
			conn.ConnectionString = ConfigurationManager.ConnectionStrings[name].ConnectionString ;
			return conn ;
		}
		#endregion
	}
}
