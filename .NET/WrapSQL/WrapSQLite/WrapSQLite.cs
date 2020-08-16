using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace WrapSQL
{
    /// <summary>
    /// SQLite-Port of WrapSQL.
    /// </summary>
    public class WrapSQLite : WrapSQLBase, IDisposable
    {
        /// <summary>
        /// Creates a new ODBC-Wrapper object.
        /// </summary>
        /// <param name="connectionString">Connection-string for the database</param>
        /// <param name="isFilepath">If set to true, only the filename has to be provided, instead of the whole connection-string</param>
        public WrapSQLite(string connectionString, bool isFilepath = true)
        {
            // Create connection

            if(isFilepath) connection = new SQLiteConnection($@"URI=file:{connectionString}");
            else connection = new SQLiteConnection(connectionString);
        }

        ///<inheritdoc/>
        protected override int ExecuteNonQuery(string sqlQuery, bool aCon, params object[] parameters)
        {
            if (transactionActive && aCon) throw new WrapSQLException("AutoConnect-methods (ACon) are not allowed durring a transaction!");

            using (SQLiteCommand command = new SQLiteCommand(sqlQuery, (SQLiteConnection)Connection))
            {
                if (transactionActive) command.Transaction = (SQLiteTransaction)transaction;
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

            using (SQLiteCommand command = new SQLiteCommand(sqlQuery, (SQLiteConnection)Connection))
            {
                if (transactionActive) command.Transaction = (SQLiteTransaction)transaction;
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
            SQLiteCommand command = new SQLiteCommand(sqlQuery, (SQLiteConnection)Connection);
            foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
            return command.ExecuteReader();
        }

        ///<inheritdoc/>
        public override DataAdapter GetDataAdapter(string sqlQuery, params object[] parameters)
        {
            using (SQLiteCommand command = new SQLiteCommand(sqlQuery, (SQLiteConnection)Connection))
            {
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                return new SQLiteDataAdapter(command);
            }
        }

        ///<inheritdoc/>
        public override DataTable CreateDataTable(string sqlQuery, params object[] parameters)
        {
            using (SQLiteCommand command = new SQLiteCommand(sqlQuery, (SQLiteConnection)Connection))
            {
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
    }
}
