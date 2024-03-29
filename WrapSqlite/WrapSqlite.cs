﻿using System.Data;
using System.Data.SQLite;

namespace WrapSql
{
    /// <summary>
    /// SQLite-Port of WrapSQL.
    /// </summary>
    public class WrapSqlite : WrapSqlBase<SQLiteDataReader, SQLiteDataAdapter>
    {
        /// <summary>
        /// Creates a new ODBC-Wrapper object.
        /// </summary>
        /// <param name="connectionString">Connection-string for the database</param>
        /// <param name="isFilepath">If set to true, only the filename has to be provided, instead of the whole connection-string</param>
        public WrapSqlite(string connectionString, bool isFilepath = true)
        {
            // Create connection

            if (isFilepath)
            {
                if (!Directory.Exists(Path.GetDirectoryName(connectionString)))
                    Directory.CreateDirectory(Path.GetDirectoryName(connectionString));

                Connection = new SQLiteConnection($@"URI=file:{connectionString}");
            }
            else Connection = new SQLiteConnection(connectionString);
        }

        ///<inheritdoc/>
        protected override int ExecuteNonQueryImplement(string sqlQuery, bool aCon, params object[] parameters)
        {
            if (transactionActive && aCon) throw new WrapSqlException("AutoConnect-methods (ACon) are not allowed durring a transaction!");

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
        protected override T ExecuteScalarImplement<T>(string sqlQuery, bool aCon, params object[] parameters)
        {
            if (transactionActive && aCon) throw new WrapSqlException("AutoConnect-methods (ACon) are not allowed durring a transaction!");

            using (SQLiteCommand command = new SQLiteCommand(sqlQuery, (SQLiteConnection)Connection))
            {
                if (transactionActive) command.Transaction = (SQLiteTransaction)transaction;
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                if (aCon) Open();
                object retval = command.ExecuteScalar();
                if (aCon) Close();
                if (retval is null && Nullable.GetUnderlyingType(typeof(T)) == null && SkalarDefaultOnNull) return default(T);
                else return (T)Convert.ChangeType(retval, typeof(T));
            }
        }

        ///<inheritdoc/>
        protected override SQLiteDataReader ExecuteQueryImplement(string sqlQuery, params object[] parameters)
        {
            SQLiteCommand command = new SQLiteCommand(sqlQuery, (SQLiteConnection)Connection);
            foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
            return command.ExecuteReader();
        }

        ///<inheritdoc/>
        protected override SQLiteDataAdapter GetDataAdapterImplement(string sqlQuery, params object[] parameters)
        {
            using (SQLiteCommand command = new SQLiteCommand(sqlQuery, (SQLiteConnection)Connection))
            {
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                return new SQLiteDataAdapter(command);
            }
        }

        ///<inheritdoc/>
        protected override DataTable CreateDataTableImplement(string sqlQuery, params object[] parameters)
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