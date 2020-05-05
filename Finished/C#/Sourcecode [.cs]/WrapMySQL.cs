using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace WrapSQL
{
    public class WrapMySQL : IDisposable
    {
        #region Fields and Properties

        private readonly string connectionString = string.Empty;
        private readonly MySqlConnection connection = null;
        private MySqlTransaction transaction = null;
        private bool transactionActive = false;

        /// <summary>
        /// SQL-Connection object.
        /// </summary>
        public MySqlConnection Connection
        {
            get => connection;
        }

        #endregion

        #region Constructors, Destructors, Interfaces

        /// <summary>
        /// Creates a new SQL-Wrapper object.
        /// </summary>
        /// <param name="connectionString">Connection-string for the database</param>
        public WrapMySQL(string connectionString)
        {
            // Set connection-string
            this.connectionString = connectionString;

            // Create connection
            connection = new MySqlConnection(this.connectionString);
        }

        /// <summary>
        /// Creates a new SQL-Wrapper object.
        /// </summary>
        /// <param name="server">Hostname or IP of the server</param>
        /// <param name="database">Target database</param>
        /// <param name="username">Login username</param>
        /// <param name="password">Login password</param>
        /// <param name="port">Server-port. Default: 3306</param>
        /// <param name="sslMode">SSL encryption mode</param>
        public WrapMySQL(string server, string database, string username, string password, int port = 3306, string sslMode = "none")
        {
            // Assemble connection-string
            this.connectionString = $"SERVER={server};Port={port};SslMode={sslMode};DATABASE={database};USER ID={username};PASSWORD={password}";

            // Create connection
            connection = new MySqlConnection(this.connectionString);
        }

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
            if (this.connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
        }

        /// <summary>
        /// Closes the SQL-Connection, if the connection is open.
        /// </summary>
        public void Close()
        {
            if (this.connection.State == System.Data.ConnectionState.Open)
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

        /// <summary>
        /// Executes a non-query statement. 
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="aCon">Manage connection states (AutoConnect)</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>NonQuery result</returns>
        private int ExecuteNonQuery(string sqlQuery, bool aCon, params object[] parameters)
        {
            if (transactionActive && aCon) throw new Exception("AutoConnect-methods (ACon) are not allowed durring a transaction!");

            using (MySqlCommand command = new MySqlCommand(sqlQuery, Connection))
            {
                if (transactionActive) command.Transaction = transaction;
                int result;
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                if (aCon) Open();
                result = command.ExecuteNonQuery();
                if (aCon) Close();
                return result;
            }
        }

        #endregion

        #region ExecuteQuery

        /// <summary>
        /// Executes a query-statement.
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>DataReader fetching the query-results</returns>
        public MySqlDataReader ExecuteQuery(string sqlQuery, params object[] parameters)
        {
            MySqlCommand command = new MySqlCommand(sqlQuery, Connection);
            foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
            return command.ExecuteReader();
        }

        #endregion

        #region ExecuteScalar

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

        /// <summary>
        /// Executes a execute-scalar statement.
        /// </summary>
        /// <typeparam name="T">Target-datatype of the result</typeparam>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="aCon">Manage connection states (AutoConnect)</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Result of the scalar-query</returns>
        private T ExecuteScalar<T>(string sqlQuery, bool aCon, params object[] parameters)
        {
            if (transactionActive && aCon) throw new Exception("AutoConnect-methods (ACon) are not allowed durring a transaction!");

            using (MySqlCommand command = new MySqlCommand(sqlQuery, Connection))
            {
                if (transactionActive) command.Transaction = transaction;
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                if (aCon) Open();
                object retval = command.ExecuteScalar();
                if (aCon) Close();
                return (T)Convert.ChangeType(retval, typeof(T));
            }
        }

        #endregion

        #region DataAdapter

        /// <summary>
        /// Fills a DataTable with the results of a query-statement.
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>Results of a query-statement</returns>
        public DataTable FillDataTable(string sqlQuery, params object[] parameters)
        {
            using(MySqlCommand command = new MySqlCommand(sqlQuery, Connection))
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

        /// <summary>
        /// Creates a DataAdapter on the given query-statement.
        /// </summary>
        /// <param name="sqlQuery">SQL-query</param>
        /// <param name="parameters">Query-parameters</param>
        /// <returns>DataAdapter of the given query-statement</returns>
        public MySqlDataAdapter GetDataAdapter(string sqlQuery, params object[] parameters)
        {
            using (MySqlCommand command = new MySqlCommand(sqlQuery, Connection))
            {
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                return new MySqlDataAdapter(command);
            }
        }

        #endregion
    }
}
