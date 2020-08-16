using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;

namespace WrapSQL
{
    /// <summary>
    /// MySQL-Port of WrapSQL.
    /// </summary>
    public class WrapMySQL : WrapSQLBase, IDisposable
    {
        /// <summary>
        /// Creates a new MySQL-Wrapper object.
        /// </summary>
        /// <param name="connectionString">Connection-string for the database</param>
        public WrapMySQL(string connectionString)
        {
            // Create connection
            connection = new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Creates a new MySQL-Wrapper object.
        /// </summary>
        /// <param name="server">Hostname or IP of the server</param>
        /// <param name="database">Target database</param>
        /// <param name="username">Login username</param>
        /// <param name="password">Login password</param>
        /// <param name="port">Server-port. Default: 3306</param>
        /// <param name="sslMode">SSL encryption mode</param>
        public WrapMySQL(string server, string database, string username, string password, int port = 3306, string sslMode = "none")
        {
            // Create connection
            connection = new MySqlConnection($"SERVER={server};Port={port};SslMode={sslMode};DATABASE={database};USER ID={username};PASSWORD={password}");
        }

        /// <summary>
        /// Creates a new MySQL-Wrapper object.
        /// </summary>
        /// <param name="mysqlData">MySQL connection data</param>
        public WrapMySQL(WrapMySQLData mysqlData)
        {
            // Create connection
            connection = new MySqlConnection($"SERVER={mysqlData.Hostname};Port={mysqlData.Port};SslMode={mysqlData.SSLMode};DATABASE={mysqlData.Database};USER ID={mysqlData.Username};PASSWORD={mysqlData.Password}");
        }

        ///<inheritdoc/>
        protected override int ExecuteNonQuery(string sqlQuery, bool aCon, params object[] parameters)
        {
            if (transactionActive && aCon) throw new WrapSQLException("AutoConnect-methods (ACon) are not allowed durring a transaction!");

            using (MySqlCommand command = new MySqlCommand(sqlQuery, (MySqlConnection)Connection))
            {
                if (transactionActive) command.Transaction = (MySqlTransaction)transaction;
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

            using (MySqlCommand command = new MySqlCommand(sqlQuery, (MySqlConnection)Connection))
            {
                if (transactionActive) command.Transaction = (MySqlTransaction)transaction;
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
            MySqlCommand command = new MySqlCommand(sqlQuery, (MySqlConnection)Connection);
            foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
            return command.ExecuteReader();
        }

        ///<inheritdoc/>
        public override DataAdapter GetDataAdapter(string sqlQuery, params object[] parameters)
        {
            using (MySqlCommand command = new MySqlCommand(sqlQuery, (MySqlConnection)Connection))
            {
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                return new MySqlDataAdapter(command);
            }
        }

        ///<inheritdoc/>
        public override DataTable CreateDataTable(string sqlQuery, params object[] parameters)
        {
            using (MySqlCommand command = new MySqlCommand(sqlQuery, (MySqlConnection)Connection))
            {
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                using (MySqlDataAdapter da = new MySqlDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
    }
}
