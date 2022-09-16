using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace WrapSql
{
    /// <summary>
    /// OleDB-Port of WrapSQL.
    /// </summary>
    public class WrapOleDB : WrapSqlBase, IDisposable
    {
        /// <summary>
        /// Creates a new ODBC-Wrapper object.
        /// </summary>
        /// <param name="connectionString">Connection-string for the database</param>
        public WrapOleDB(string connectionString)
        {
            // Create connection
            Connection = new OleDbConnection(connectionString);
        }

        ///<inheritdoc/>
        protected override int ExecuteNonQueryImplement(string sqlQuery, bool aCon, params object[] parameters)
        {
            if (transactionActive && aCon) throw new WrapSqlException("AutoConnect-methods (ACon) are not allowed durring a transaction!");

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
        protected override T ExecuteScalarImplement<T>(string sqlQuery, bool aCon, params object[] parameters)
        {
            if (transactionActive && aCon) throw new WrapSqlException("AutoConnect-methods (ACon) are not allowed durring a transaction!");

            using (OleDbCommand command = new OleDbCommand(sqlQuery, (OleDbConnection)Connection))
            {
                if (transactionActive) command.Transaction = (OleDbTransaction)transaction;
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                if (aCon) Open();
                object retval = command.ExecuteScalar();
                if (aCon) Close();
                if (retval is null && Nullable.GetUnderlyingType(typeof(T)) == null && SkalarDefaultOnNull) return default(T);
                else return (T)Convert.ChangeType(retval, typeof(T));
            }
        }

        ///<inheritdoc/>
        protected override DbDataReader ExecuteQueryImplement(string sqlQuery, params object[] parameters)
        {
            OleDbCommand command = new OleDbCommand(sqlQuery, (OleDbConnection)Connection);
            foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
            return command.ExecuteReader();
        }

        ///<inheritdoc/>
        protected override DataAdapter GetDataAdapterImplement(string sqlQuery, params object[] parameters)
        {
            using (OleDbCommand command = new OleDbCommand(sqlQuery, (OleDbConnection)Connection))
            {
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                return new OleDbDataAdapter(command);
            }
        }

        ///<inheritdoc/>
        protected override DataTable CreateDataTableImplement(string sqlQuery, params object[] parameters)
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
