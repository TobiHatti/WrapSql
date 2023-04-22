using MySql.Data.MySqlClient;
using System.Data;

namespace WrapSql
{
    /// <summary>
    /// MySQL-Port of WrapSQL.
    /// </summary>
    public sealed class WrapMySql : WrapSqlBase<MySqlDataReader, MySqlDataAdapter>
    {
        /// <summary>
        /// Creates a new MySQL-Wrapper object.
        /// </summary>
        /// <param name="connectionString">Connection-string for the database</param>
        public WrapMySql(string connectionString)
        {
            // Create connection
            Connection = new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Creates a new MySQL-Wrapper object.
        /// </summary>
        /// <param name="server">Hostname or IP of the server</param>
        /// <param name="database">Target database</param>
        /// <param name="username">Login username</param>
        /// <param name="password">Login password</param>
        public WrapMySql(string server, string database, string username, string password)
        {
            // Create connection
            Connection = new MySqlConnection($"Server={server};Database={database};Uid={username};Pwd={password}");
        }

        /// <summary>
        /// Creates a new MySQL-Wrapper object.
        /// </summary>
        /// <param name="mysqlData">MySQL connection data</param>
        public WrapMySql(IWrapSqlConnector mysqlData)
        {
            // Create connection
            Connection = new MySqlConnection(mysqlData.ToString());
        }

        ///<inheritdoc/>
        protected override int ExecuteNonQueryImplement(string sqlQuery, bool aCon, params object[] parameters)
        {
            if (transactionActive && aCon) throw new WrapSqlException("AutoConnect-methods (ACon) are not allowed durring a transaction!");

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
        protected override T ExecuteScalarImplement<T>(string sqlQuery, bool aCon, params object[] parameters)
        {
            if (transactionActive && aCon) throw new WrapSqlException("AutoConnect-methods (ACon) are not allowed durring a transaction!");

            using (MySqlCommand command = new MySqlCommand(sqlQuery, (MySqlConnection)Connection))
            {
                if (transactionActive) command.Transaction = (MySqlTransaction)transaction;
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                if (aCon) Open();
                object retval = command.ExecuteScalar();
                if (aCon) Close();
                if (retval is null && Nullable.GetUnderlyingType(typeof(T)) == null && SkalarDefaultOnNull) return default(T);
                else return (T)Convert.ChangeType(retval, typeof(T));
            }
        }

        ///<inheritdoc/>
        protected override MySqlDataReader ExecuteQueryImplement(string sqlQuery, params object[] parameters)
        {
            MySqlCommand command = new MySqlCommand(sqlQuery, (MySqlConnection)Connection);
            foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
            return command.ExecuteReader();
        }

        ///<inheritdoc/>
        protected override MySqlDataAdapter GetDataAdapterImplement(string sqlQuery, params object[] parameters)
        {
            using (MySqlCommand command = new MySqlCommand(sqlQuery, (MySqlConnection)Connection))
            {
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                return new MySqlDataAdapter(command);
            }
        }

        ///<inheritdoc/>
        protected override DataTable CreateDataTableImplement(string sqlQuery, params object[] parameters)
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