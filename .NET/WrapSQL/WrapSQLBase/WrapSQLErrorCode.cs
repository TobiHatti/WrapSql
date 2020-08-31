using System;
using System.Collections.Generic;
using System.Text;

namespace WrapSQL
{
    /// <summary>
    /// Error-Codes for WrapSQL
    /// </summary>
    public enum WrapSQLErrorCode
    {
        /// <summary>
        /// No error (initial value)
        /// </summary>
        None = 0,
        /// <summary>
        /// Success - The last operation was executed successfully
        /// </summary>
        Success = 200,



        /// <summary>
        /// Connection creation failed - WrapSQL could not establish a connection to the database
        /// </summary>
        ConnectionCreationFailed = 1100,
        /// <summary>
        /// Connection close failed - WrapSQL could not close the current connection
        /// </summary>
        ConnectionCloseFailed = 1101,



        /// <summary>
        /// Transaction begin failed - The transaction could not be started
        /// </summary>
        TransactionBeginFailed = 1201,
        /// <summary>
        /// Transaction commit failed - The transaction could not be commited
        /// </summary>
        TransactionCommitFailed = 1202,
        /// <summary>
        /// Transaction rollback failed - The transaction could not be rolled back
        /// </summary>
        TransactionRollbackFailed = 1203,



        /// <summary>
        /// Operation NonQuery failed - An error occured within the ExecuteNonQuery-Method
        /// </summary>
        OperationNonQueryFailed = 2100,
        /// <summary>
        /// Operation Scalar failed - An error occured within the ExecuteScalar-Method
        /// </summary>
        OperationScalarFailed = 2200,
        /// <summary>
        /// Operation Query failed - An error occured within the ExecuteQuery-Method
        /// </summary>
        OperationQueryFailed = 2300,



        /// <summary>
        /// Operation DataAdapter failed - An error occured within the GetDataAdapter-Method
        /// </summary>
        OperationDataAdapterFailed = 3100,
        /// <summary>
        /// Operation DataTable failed - An error occured within the CreateDataTable-Method
        /// </summary>
        OperationDataTableFailed = 3200
    }
}
