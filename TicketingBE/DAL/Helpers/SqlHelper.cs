using System.Data;
using System.Data.Common;
using Npgsql;
using NpgsqlTypes;

namespace TicketingBE.DAL.Helpers
{
    /// <summary>
    /// Helper class for PostgreSQL database operations using ADO.NET (Npgsql)
    /// Provides reusable methods for executing queries and managing connections
    /// </summary>
    public class SqlHelper
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly IsolationLevel _isolationLevel = IsolationLevel.ReadCommitted;

        public SqlHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found.");
        }

        /// <summary>
        /// Gets the connection string
        /// </summary>
        public string GetConnectionString()
        {
            return _connectionString;
        }

        /// <summary>
        /// Executes a non-query command (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="query">SQL query to execute</param>
        /// <param name="parameters">Query parameters</param>
        /// <returns>Number of rows affected</returns>
        public int ExecuteData(string query, params DbParameter[] parameters)
        {
            try
            {
                return ExecuteData(null!, null!, query, false, parameters);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error executing query: {query}", ex);
            }
        }

        /// <summary>
        /// Executes a non-query command with transaction support
        /// </summary>
        /// <param name="connection">Existing connection (optional)</param>
        /// <param name="transaction">Existing transaction (optional)</param>
        /// <param name="query">SQL query to execute</param>
        /// <param name="commit">Whether to commit the transaction</param>
        /// <param name="parameters">Query parameters</param>
        /// <returns>Number of rows affected</returns>
        public int ExecuteData(IDbConnection connection, IDbTransaction transaction, string query, bool commit, params DbParameter[] parameters)
        {
            NpgsqlConnection? conn = null;
            NpgsqlTransaction? txn = null;
            int result = 0;

            try
            {
                conn = connection != null ? (NpgsqlConnection)connection : new NpgsqlConnection(_connectionString);
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                txn = transaction != null ? (NpgsqlTransaction)transaction : BeginTransaction(conn);

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn, txn))
                {
                    if (parameters != null)
                    {
                        foreach (DbParameter param in parameters)
                        {
                            cmd.Parameters.Add((NpgsqlParameter)param);
                        }
                    }
                    result = cmd.ExecuteNonQuery();
                }

                if (commit && txn != null)
                {
                    txn.Commit();
                }
            }
            catch (Exception ex)
            {
                if (txn != null && commit)
                {
                    try
                    {
                        txn.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        throw new AggregateException("Transaction rollback failed after original exception", ex, rollbackEx);
                    }
                }
                throw new InvalidOperationException($"Error executing query: {query}", ex);
            }
            finally
            {
                if (commit && conn != null)
                {
                    CloseConnection(conn);
                }
            }

            return result;
        }

        /// <summary>
        /// Executes a query and returns a result using a custom data reader function
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="query">SQL query to execute</param>
        /// <param name="dataReaderFn">Function to process the data reader</param>
        /// <param name="parameters">Query parameters</param>
        /// <returns>Result of type T</returns>
        public T ExecuteReader<T>(string query, Func<DbDataReader, T> dataReaderFn, params DbParameter[] parameters)
        {
            return ExecuteReader<T>(null!, null!, query, dataReaderFn, true, parameters);
        }

        /// <summary>
        /// Executes a query with transaction support and returns a result using a custom data reader function
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="connection">Existing connection (optional)</param>
        /// <param name="transaction">Existing transaction (optional)</param>
        /// <param name="query">SQL query to execute</param>
        /// <param name="dataReaderFn">Function to process the data reader</param>
        /// <param name="commit">Whether to commit the transaction</param>
        /// <param name="parameters">Query parameters</param>
        /// <returns>Result of type T</returns>
        public T ExecuteReader<T>(IDbConnection connection, IDbTransaction transaction, string query, Func<DbDataReader, T> dataReaderFn, bool commit, params DbParameter[] parameters)
        {
            T? ret = default(T);
            NpgsqlConnection? conn = null;
            NpgsqlTransaction? txn = null;
            NpgsqlDataReader? dataReader = null;

            try
            {
                conn = connection != null ? (NpgsqlConnection)connection : new NpgsqlConnection(_connectionString);
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                txn = transaction != null ? (NpgsqlTransaction)transaction : BeginTransaction(conn);

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn, txn))
                {
                    if (parameters != null)
                    {
                        foreach (DbParameter param in parameters)
                        {
                            cmd.Parameters.Add((NpgsqlParameter)param);
                        }
                    }

                    dataReader = cmd.ExecuteReader();

                    if (dataReaderFn != null)
                    {
                        ret = dataReaderFn(dataReader);
                    }
                }

                if (commit && txn != null)
                {
                    txn.Commit();
                }

                return ret!;
            }
            catch (Exception ex)
            {
                if (txn != null && commit)
                {
                    try
                    {
                        txn.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        throw new AggregateException("Transaction rollback failed after original exception", ex, rollbackEx);
                    }
                }
                throw new InvalidOperationException($"Error executing query: {query}", ex);
            }
            finally
            {
                try
                {
                    if (dataReader != null && !dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }
                }
                catch (Exception)
                {
                    // Log but don't throw exceptions in finally block
                }

                if (commit && conn != null)
                {
                    CloseConnection(conn);
                }
            }
        }

        /// <summary>
        /// Begins a new transaction
        /// </summary>
        public NpgsqlTransaction BeginTransaction(NpgsqlConnection conn)
        {
            return conn.BeginTransaction(_isolationLevel);
        }

        /// <summary>
        /// Begins a new transaction on the given connection
        /// </summary>
        public IDbTransaction BeginTransaction(IDbConnection conn)
        {
            return conn.BeginTransaction(_isolationLevel);
        }

        /// <summary>
        /// Opens a database connection
        /// </summary>
        public void OpenConnection(IDbConnection connection)
        {
            if (connection != null && connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        /// <summary>
        /// Closes a database connection
        /// </summary>
        public void CloseConnection(IDbConnection connection)
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        // Parameter creation methods
        
        public DbParameter CreateParam(string paramName, int value)
        {
            NpgsqlParameter param = new NpgsqlParameter(paramName, NpgsqlDbType.Integer);
            param.Value = value;
            return param;
        }

        public DbParameter CreateParam(string paramName, long value)
        {
            NpgsqlParameter param = new NpgsqlParameter(paramName, NpgsqlDbType.Bigint);
            param.Value = value;
            return param;
        }

        public DbParameter CreateParam(string paramName, string value)
        {
            NpgsqlParameter param = new NpgsqlParameter(paramName, NpgsqlDbType.Varchar);
            param.Value = value ?? (object)DBNull.Value;
            return param;
        }

        public DbParameter CreateParam(string paramName, DateTime value)
        {
            NpgsqlParameter param = new NpgsqlParameter(paramName, NpgsqlDbType.Timestamp);
            param.Value = value;
            return param;
        }

        public DbParameter CreateParam(string paramName, bool value)
        {
            NpgsqlParameter param = new NpgsqlParameter(paramName, NpgsqlDbType.Boolean);
            param.Value = value;
            return param;
        }

        public DbParameter CreateParam(string paramName, Guid value)
        {
            NpgsqlParameter param = new NpgsqlParameter(paramName, NpgsqlDbType.Uuid);
            param.Value = value;
            return param;
        }

        public DbParameter CreateParam(string paramName, byte[] value)
        {
            NpgsqlParameter param = new NpgsqlParameter(paramName, NpgsqlDbType.Bytea);
            param.Value = value ?? (object)DBNull.Value;
            return param;
        }

        public DbParameter CreateParam(string paramName, decimal value)
        {
            NpgsqlParameter param = new NpgsqlParameter(paramName, NpgsqlDbType.Numeric);
            param.Value = value;
            return param;
        }

        public DbParameter CreateParam(string paramName, List<string> value)
        {
            NpgsqlParameter param = new NpgsqlParameter(paramName, NpgsqlDbType.Array | NpgsqlDbType.Varchar);
            param.Value = value;
            return param;
        }

        public DbParameter CreateParam(string paramName, List<Guid> values)
        {
            NpgsqlParameter param = new NpgsqlParameter(paramName, NpgsqlDbType.Array | NpgsqlDbType.Uuid);
            param.Value = values.ToArray();
            return param;
        }

        public NpgsqlParameter CreateParam(string paramName, object value, NpgsqlDbType dbType)
        {
            NpgsqlParameter param = new NpgsqlParameter(paramName, dbType);
            param.Value = value ?? DBNull.Value;
            return param;
        }

        public NpgsqlParameter CreateParam(string paramName, object value, NpgsqlDbType dbType, int size)
        {
            NpgsqlParameter param = new NpgsqlParameter(paramName, dbType, size);
            param.Value = value ?? DBNull.Value;
            return param;
        }

        public NpgsqlParameter CreateOutParam(string paramName, NpgsqlDbType dbType)
        {
            NpgsqlParameter paramOut = new NpgsqlParameter(paramName, dbType);
            paramOut.Direction = ParameterDirection.Output;
            return paramOut;
        }

        public NpgsqlParameter CreateOutParam(string paramName, NpgsqlDbType dbType, int size)
        {
            NpgsqlParameter paramOut = new NpgsqlParameter(paramName, dbType, size);
            paramOut.Direction = ParameterDirection.Output;
            return paramOut;
        }
    }
}
