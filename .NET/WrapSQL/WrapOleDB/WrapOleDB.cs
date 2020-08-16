using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace WrapSQL
{
    /// <summary>
    /// OleDB-Port of WrapSQL.
    /// </summary>
    public class WrapOleDB : WrapSQLBase, IDisposable
    {
        /// <summary>
        /// Creates a new ODBC-Wrapper object.
        /// </summary>
        /// <param name="connectionString">Connection-string for the database</param>
        public WrapOleDB(string connectionString)
        {
            // Create connection
            connection = new OleDbConnection(connectionString);
        }

        ///<inheritdoc/>
        protected override int ExecuteNonQuery(string sqlQuery, bool aCon, params object[] parameters)
        {
            if (transactionActive && aCon) throw new WrapSQLException("AutoConnect-methods (ACon) are not allowed durring a transaction!");

            using (OleDbCommand command = new OleDbCommand(sqlQuery, (OleDbConnection)Connection))
            {
                if (transactionActive) command.Transaction = (OleDbTransaction)transaction;
                int result;
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                if (aCon) Open();
                result = command.ExecuteNonQuery();
                if (aCon) Close();
                return result;
            }
        }

        ///<inheritdoc/>
        protected override T ExecuteScalar<T>(string sqlQuery, bool aCon, params object[] parameters)
        {
            if (transactionActive && aCon) throw new WrapSQLException("AutoConnect-methods (ACon) are not allowed durring a transaction!");

            using (OleDbCommand command = new OleDbCommand(sqlQuery, (OleDbConnection)Connection))
            {
                if (transactionActive) command.Transaction = (OleDbTransaction)transaction;
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                if (aCon) Open();
                object retval = command.ExecuteScalar();
                if (aCon) Close();
                return (T)Convert.ChangeType(retval, typeof(T));
            }
        }

        ///<inheritdoc/>
        public override DbDataReader ExecuteQuery(string sqlQuery, params object[] parameters)
        {
            OleDbCommand command = new OleDbCommand(sqlQuery, (OleDbConnection)Connection);
            foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
            return command.ExecuteReader();
        }

        ///<inheritdoc/>
        public override DataAdapter GetDataAdapter(string sqlQuery, params object[] parameters)
        {
            using (OleDbCommand command = new OleDbCommand(sqlQuery, (OleDbConnection)Connection))
            {
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                return new OleDbDataAdapter(command);
            }
        }

        ///<inheritdoc/>
        public override DataTable CreateDataTable(string sqlQuery, params object[] parameters)
        {
            using (OleDbCommand command = new OleDbCommand(sqlQuery, (OleDbConnection)Connection))
            {
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                using (OleDbDataAdapter da = new OleDbDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
    }
}
