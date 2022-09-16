using System;
using System.Data;
using System.Data.Common;

namespace WrapSql
{
    /// <summary>
    /// WrapSQL base class.
    /// </summary>
    public abstract class WrapSqlBase : IDisposable, IWrapSql
    {
        #region Fields

        /// <summary>
        /// Flag to check if a transaction is currently active
        /// </summary>
        protected bool transactionActive = false;

        /// <summary>
        /// Database transaction
        /// </summary>
        protected DbTransaction transaction = null;

        #endregion

        #region Properties

        /// <summary>
        /// SQL-Connection object.
        /// </summary>
        public DbConnection Connection { get; protected set; } = null;

        /// <summary>
        /// Reports the Error-Code of the last executed operation
        /// </summary>
        public WrapSqlErrorCode LastErrorCode { get; protected set; } = WrapSqlErrorCode.None;

        /// <summary>
        /// Determines if Execute-Scalar should return the requested type's default value, or throw an exception (only affects non-nullable types)
        /// </summary>
        public bool SkalarDefaultOnNull { get; set; } = true;
        #endregion

        #region Interface implementations

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            // Dispose the connection object
            if (Connection != null) Connection.Dispose();
        }

        #endregion

        #region Connection Open/Close

        /// <summary>
        /// Opens the SQL-connection, if the connection is closed
        /// </summary>
        /// <returns>Returns true if the connection was established successfully</returns>
        public virtual void Open()
        {
            try
            {
                if (this.Connection.State == ConnectionState.Closed)
                    Connection.Open();
                LastErrorCode = WrapSqlErrorCode.Success;
            }
            catch (Exception ex)
            {
                LastErrorCode = WrapSqlErrorCode.ConnectionCreationFailed;
                throw new WrapSqlException("Connection could not be opened.", ex);
            }
        }

        /// <summary>
        /// Closes the SQL-Connection, if the connection is open.
        /// </summary>
        public virtual void Close()
        {
            try
            {
                if (this.Connection.State == ConnectionState.Open)
                    Connection.Close();
                LastErrorCode = WrapSqlErrorCode.Success;
            }
            catch (Exception ex)
            {
                LastErrorCode = WrapSqlErrorCode.ConnectionCloseFailed;
                throw new WrapSqlException("Connection could not be closed.", ex);
            }
        }

        #endregion

        #region Transaction Begin/Commit/Rollback

        /// <summary>
        /// Starts a transaction.
        /// </summary>
        public void TransactionBegin()
        {
            try
            {
                transactionActive = true;
                transaction = Connection.BeginTransaction();
                LastErrorCode = WrapSqlErrorCode.Success;
            }
            catch (Exception ex)
            {
                LastErrorCode = WrapSqlErrorCode.TransactionBeginFailed;
                throw new WrapSqlException("Transaction could not be started.", ex);
            }
        }

        /// <summary>
        /// Commits a transaction.
        /// </summary>
        public void TransactionCommit()
        {
            try
            {
                transaction.Commit();
                transactionActive = false;
                LastErrorCode = WrapSqlErrorCode.Success;
            }
            catch (Exception ex)
            {
                LastErrorCode = WrapSqlErrorCode.TransactionCommitFailed;
                throw new WrapSqlException("Transaction could not be commited.", ex);
            }
        }

        /// <summary>
        /// Terminates a transaction.
        /// </summary>
        public void TransactionRollback()
        {
            try
            {
                transaction.Rollback();
                transactionActive = false;
                LastErrorCode = WrapSqlErrorCode.Success;
            }
            catch (Exception ex)
            {
                LastErrorCode = WrapSqlErrorCode.TransactionRollbackFailed;
                throw new WrapSqlException("Transaction could not be rolled back.", ex);
            }
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// Executes a non-query statement. 
        /// NOTE FOR IMPLEMENTATION: 
        /// Do not handle exceptions or errors within this method!
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="aCon">Manage connection states (AutoConnect)</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>NonQuery result</returns>
        protected abstract int ExecuteNonQueryImplement(string sqlQuery, bool aCon, params object[] parameters);

        /// <summary>
        /// Executes a non-query statement (Handled wrapper).
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="aCon">Manage connection states (AutoConnect)</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>NonQuery result</returns>
        private int ExecuteNonQueryHandled(string sqlQuery, bool aCon, params object[] parameters)
        {
            try
            {
                int dbResult = ExecuteNonQueryImplement(sqlQuery, aCon, parameters);
                LastErrorCode = WrapSqlErrorCode.Success;
                return dbResult;
            }
            catch (Exception ex)
            {
                LastErrorCode = WrapSqlErrorCode.OperationNonQueryFailed;
                throw new WrapSqlException("The operation \"ExecuteNonQuery\" failed.", ex);
            }
        }

        /// <summary>
        /// Executes a non-query statement. 
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>NonQuery result</returns>
        public int ExecuteNonQuery(string sqlQuery, params object[] parameters)
            => ExecuteNonQueryHandled(sqlQuery, false, parameters);

        /// <summary>
        /// Executes a non-query statement. 
        /// Automatically opens and closes the connection.
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>NonQuery result</returns>
        public int ExecuteNonQueryACon(string sqlQuery, params object[] parameters)
            => ExecuteNonQueryHandled(sqlQuery, true, parameters);

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// Executes a execute-scalar statement.
        /// NOTE FOR IMPLEMENTATION: 
        /// Do not handle exceptions or errors within this method!
        /// </summary>
        /// <typeparam name="T">Target-datatype of the result</typeparam>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="aCon">Manage connection states (AutoConnect)</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Result of the scalar-query</returns>
        protected abstract T ExecuteScalarImplement<T>(string sqlQuery, bool aCon, params object[] parameters);

        /// <summary>
        /// Executes a execute-scalar statement (Handled wrapper).
        /// </summary>
        /// <typeparam name="T">Target-datatype of the result</typeparam>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="aCon">Manage connection states (AutoConnect)</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Result of the scalar-query</returns>
        private T ExecuteScalarHandled<T>(string sqlQuery, bool aCon, params object[] parameters)
        {
            try
            {
                T dbResult = ExecuteScalarImplement<T>(sqlQuery, aCon, parameters);
                LastErrorCode = WrapSqlErrorCode.Success;
                return dbResult;
            }
            catch (Exception ex)
            {
                LastErrorCode = WrapSqlErrorCode.OperationScalarFailed;
                throw new WrapSqlException("The operation \"ExecuteScalar\" failed.", ex);
            }
        }

        /// <summary>
        /// Executes a execute-scalar statement. 
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Result of the scalar-query</returns>
        public object ExecuteScalar(string sqlQuery, params object[] parameters)
            => ExecuteScalarHandled<object>(sqlQuery, false, parameters);

        /// <summary>
        /// Executes a execute-scalar statement. 
        /// Automatically opens and closes the connection.
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Result of the scalar-query</returns>
        public object ExecuteScalarACon(string sqlQuery, params object[] parameters)
            => ExecuteScalarHandled<object>(sqlQuery, true, parameters);

        /// <summary>
        /// Executes a execute-scalar statement. 
        /// </summary>
        /// <typeparam name="T">Target-datatype of the result</typeparam>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Result of the scalar-query</returns>
        public T ExecuteScalar<T>(string sqlQuery, params object[] parameters)
            => ExecuteScalarHandled<T>(sqlQuery, false, parameters);

        /// <summary>
        /// Executes a execute-scalar statement.
        /// Automatically opens and closes the connection.
        /// </summary>
        /// <typeparam name="T">Target-datatype of the result</typeparam>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Result of the scalar-query</returns>
        public T ExecuteScalarACon<T>(string sqlQuery, params object[] parameters)
            => ExecuteScalarHandled<T>(sqlQuery, true, parameters);

        #endregion

        #region ExecuteQuery

        /// <summary>
        /// Executes a query-statement.
        /// NOTE FOR IMPLEMENTATION: 
        /// Do not handle exceptions or errors within this method!
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>DataReader fetching the query-results</returns>
        protected abstract DbDataReader ExecuteQueryImplement(string sqlQuery, params object[] parameters);

        /// <summary>
        /// Executes a query-statement (Handled wrapper).
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>DataReader fetching the query-results</returns>
        private DbDataReader ExecuteQueryManaged(string sqlQuery, params object[] parameters)
        {
            try
            {
                DbDataReader dbResult = ExecuteQueryImplement(sqlQuery, parameters);
                LastErrorCode = WrapSqlErrorCode.Success;
                return dbResult;
            }
            catch (Exception ex)
            {
                LastErrorCode = WrapSqlErrorCode.OperationQueryFailed;
                throw new WrapSqlException("The operation \"ExecuteQuery\" failed.", ex);
            }
        }

        /// <summary>
        /// Executes a query-statement.
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>DataReader fetching the query-results</returns>
        public DbDataReader ExecuteQuery(string sqlQuery, params object[] parameters)
            => ExecuteQueryManaged(sqlQuery, parameters);

        #endregion

        #region DataTable

        /// <summary>
        /// Creates a DataTable with the results of a query-statement.
        /// NOTE FOR IMPLEMENTATION: 
        /// Do not handle exceptions or errors within this method!
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Results of a query-statement</returns>
        protected abstract DataTable CreateDataTableImplement(string sqlQuery, params object[] parameters);

        /// <summary>
        /// Creates a DataTable with the results of a query-statement (Handled wrapper). 
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Results of a query-statement</returns>
        private DataTable CreateDataTableHandled(string sqlQuery, params object[] parameters)
        {
            try
            {
                DataTable dbResult = CreateDataTableImplement(sqlQuery, parameters);
                LastErrorCode = WrapSqlErrorCode.Success;
                return dbResult;
            }
            catch (Exception ex)
            {
                LastErrorCode = WrapSqlErrorCode.OperationDataTableFailed;
                throw new WrapSqlException("The operation \"CreateDataTable\" failed.", ex);
            }
        }

        /// <summary>
        /// Creates a DataTable with the results of a query-statement.
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Results of a query-statement</returns>
        public DataTable CreateDataTable(string sqlQuery, params object[] parameters)
            => CreateDataTableHandled(sqlQuery, parameters);

        #endregion

        #region DataAdapter

        /// <summary>
        /// Creates a DataAdapter on the given query-statement.
        /// NOTE FOR IMPLEMENTATION: 
        /// Do not handle exceptions or errors within this method!
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>DataAdapter of the given query-statement</returns>
        protected abstract DataAdapter GetDataAdapterImplement(string sqlQuery, params object[] parameters);

        /// <summary>
        /// Creates a DataAdapter on the given query-statement (Handled wrapper).
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>DataAdapter of the given query-statement</returns>
        private DataAdapter GetDataAdapterHandled(string sqlQuery, params object[] parameters)
        {
            try
            {
                DataAdapter dbResult = GetDataAdapterImplement(sqlQuery, parameters);
                LastErrorCode = WrapSqlErrorCode.Success;
                return dbResult;
            }
            catch (Exception ex)
            {
                LastErrorCode = WrapSqlErrorCode.OperationDataAdapterFailed;
                throw new WrapSqlException("The operation \"GetDataAdapter\" failed.", ex);
            }
        }

        /// <summary>
        /// Creates a DataAdapter on the given query-statement.
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>DataAdapter of the given query-statement</returns>
        public DataAdapter GetDataAdapter(string sqlQuery, params object[] parameters)
            => GetDataAdapterHandled(sqlQuery, parameters);

        #endregion
    }
}
