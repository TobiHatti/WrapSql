using System;
using System.Data;
using System.Data.Common;

namespace WrapSQL
{
    /// <summary>
    /// WrapSQL base class.
    /// </summary>
    public abstract class WrapSQLBase : IDisposable
    {
        #region Fields

        /// <summary>
        /// Flag to check if a transaction is currently active
        /// </summary>
        protected bool transactionActive = false;

        /// <summary>
        /// Database connection
        /// </summary>
        protected DbConnection connection = null;

        /// <summary>
        /// Database transaction
        /// </summary>
        protected DbTransaction transaction = null;

        #endregion

        #region Properties

        /// <summary>
        /// SQL-Connection object.
        /// </summary>
        public DbConnection Connection
        {
            get => connection;
        }

        #endregion

        #region Interface implementations

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            // Dispose the connection object
            if (connection != null) connection.Dispose();
        }

        #endregion

        #region Connection Open/Close

        /// <summary>
        /// Opens the SQL-connection, if the connection is closed
        /// </summary>
        public void Open()
        {
            if (this.connection.State == ConnectionState.Closed)
                connection.Open();
        }

        /// <summary>
        /// Closes the SQL-Connection, if the connection is open.
        /// </summary>
        public void Close()
        {
            if (this.connection.State == ConnectionState.Open)
                connection.Close();
        }

        #endregion

        #region Transaction Begin/Commit/Rollback

        /// <summary>
        /// Starts a transaction.
        /// </summary>
        public void TransactionBegin()
        {
            transactionActive = true;
            transaction = Connection.BeginTransaction();
        }

        /// <summary>
        /// Commits a transaction.
        /// </summary>
        public void TransactionCommit()
        {
            transaction.Commit();
            transactionActive = false;
        }

        /// <summary>
        /// Terminates a transaction.
        /// </summary>
        public void TransactionRollback()
        {
            transaction.Rollback();
            transactionActive = false;
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// Executes a non-query statement. 
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="aCon">Manage connection states (AutoConnect)</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>NonQuery result</returns>
        protected abstract int ExecuteNonQuery(string sqlQuery, bool aCon, params object[] parameters);

        /// <summary>
        /// Executes a non-query statement. 
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>NonQuery result</returns>
        public int ExecuteNonQuery(string sqlQuery, params object[] parameters)
            => ExecuteNonQuery(sqlQuery, false, parameters);

        /// <summary>
        /// Executes a non-query statement. 
        /// Automatically opens and closes the connection.
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>NonQuery result</returns>
        public int ExecuteNonQueryACon(string sqlQuery, params object[] parameters)
            => ExecuteNonQuery(sqlQuery, true, parameters);

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// Executes a execute-scalar statement.
        /// </summary>
        /// <typeparam name="T">Target-datatype of the result</typeparam>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="aCon">Manage connection states (AutoConnect)</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Result of the scalar-query</returns>
        protected abstract T ExecuteScalar<T>(string sqlQuery, bool aCon, params object[] parameters);

        /// <summary>
        /// Executes a execute-scalar statement. 
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Result of the scalar-query</returns>
        public object ExecuteScalar(string sqlQuery, params object[] parameters)
            => ExecuteScalar<object>(sqlQuery, false, parameters);

        /// <summary>
        /// Executes a execute-scalar statement. 
        /// Automatically opens and closes the connection.
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Result of the scalar-query</returns>
        public object ExecuteScalarACon(string sqlQuery, params object[] parameters)
            => ExecuteScalar<object>(sqlQuery, true, parameters);

        /// <summary>
        /// Executes a execute-scalar statement. 
        /// </summary>
        /// <typeparam name="T">Target-datatype of the result</typeparam>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Result of the scalar-query</returns>
        public T ExecuteScalar<T>(string sqlQuery, params object[] parameters)
            => ExecuteScalar<T>(sqlQuery, false, parameters);

        /// <summary>
        /// Executes a execute-scalar statement.
        /// Automatically opens and closes the connection.
        /// </summary>
        /// <typeparam name="T">Target-datatype of the result</typeparam>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Result of the scalar-query</returns>
        public T ExecuteScalarACon<T>(string sqlQuery, params object[] parameters)
            => ExecuteScalar<T>(sqlQuery, true, parameters);

        #endregion

        #region ExecuteQuery

        /// <summary>
        /// Executes a query-statement.
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>DataReader fetching the query-results</returns>
        public abstract DbDataReader ExecuteQuery(string sqlQuery, params object[] parameters);

        #endregion

        #region DataAdapter

        /// <summary>
        /// Creates a DataTable with the results of a query-statement.
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Results of a query-statement</returns>
        public abstract DataTable CreateDataTable(string sqlQuery, params object[] parameters);

        /// <summary>
        /// Creates a DataAdapter on the given query-statement.
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>DataAdapter of the given query-statement</returns>
        public abstract DataAdapter GetDataAdapter(string sqlQuery, params object[] parameters);

        #endregion
    }
}
