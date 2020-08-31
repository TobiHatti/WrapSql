﻿using System;
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

        /// <summary>
        /// 
        /// </summary>
        public WrapSQLErrorCode LastErrorCode { get; protected set; } = WrapSQLErrorCode.None;

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
        /// <returns>Returns true if the connection was established successfully</returns>
        public virtual void Open()
        {
            try
            {
                if (this.connection.State == ConnectionState.Closed)
                    connection.Open();
                LastErrorCode = WrapSQLErrorCode.Success;
            }
            catch(Exception ex)
            {
                LastErrorCode = WrapSQLErrorCode.ConnectionCreationFailed;
                throw new WrapSQLException("Connection could not be opened.", ex);
            }
        }

        /// <summary>
        /// Closes the SQL-Connection, if the connection is open.
        /// </summary>
        public virtual void Close()
        {
            try
            {
                if (this.connection.State == ConnectionState.Open)
                    connection.Close();
                LastErrorCode = WrapSQLErrorCode.Success;
            }
            catch (Exception ex)
            {
                LastErrorCode = WrapSQLErrorCode.ConnectionCloseFailed;
                throw new WrapSQLException("Connection could not be closed.", ex);
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
                LastErrorCode = WrapSQLErrorCode.Success;
            }
            catch(Exception ex)
            {
                LastErrorCode = WrapSQLErrorCode.TransactionBeginFailed;
                throw new WrapSQLException("Transaction could not be started.", ex);
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
                LastErrorCode = WrapSQLErrorCode.Success;
            }
            catch(Exception ex)
            {
                LastErrorCode = WrapSQLErrorCode.TransactionCommitFailed;
                throw new WrapSQLException("Transaction could not be commited.", ex);
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
                LastErrorCode = WrapSQLErrorCode.Success;
            }
            catch(Exception ex)
            {
                LastErrorCode = WrapSQLErrorCode.TransactionRollbackFailed;
                throw new WrapSQLException("Transaction could not be rolled back.", ex);
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

        protected int ExecuteNonQueryHandled(string sqlQuery, bool aCon, params object[] parameters)
        {
            try
            {
                int dbResult = ExecuteNonQueryImplement(sqlQuery, aCon, parameters);
                LastErrorCode = WrapSQLErrorCode.Success;
                return dbResult;
            }
            catch(Exception ex)
            {
                LastErrorCode = WrapSQLErrorCode.OperationNonQueryFailed;
                throw new WrapSQLException("The operation \"ExecuteNonQuery\" failed.", ex);
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

        protected T ExecuteScalarHandled<T>(string sqlQuery, bool aCon, params object[] parameters)
        {
            try
            {
                T dbResult = ExecuteScalarImplement<T>(sqlQuery, aCon, parameters);
                LastErrorCode = WrapSQLErrorCode.Success;
                return dbResult;
            }
            catch (Exception ex)
            {
                LastErrorCode = WrapSQLErrorCode.OperationScalarFailed;
                throw new WrapSQLException("The operation \"ExecuteScalar\" failed.", ex);
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
