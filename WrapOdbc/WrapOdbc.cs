using System.Data;
using System.Data.Odbc;

namespace WrapSql
{
    /// <summary>
    /// ODBC-Port of WrapSQL.
    /// </summary>
    public class WrapOdbc : WrapSqlBase<OdbcDataReader, OdbcDataAdapter>
    {
        /// <summary>
        /// Creates a new ODBC-Wrapper object.
        /// </summary>
        /// <param name="connectionString">Connection-string for the database</param>
        public WrapOdbc(string connectionString)
        {
            // Create connection
            Connection = new OdbcConnection(connectionString);
        }

        ///<inheritdoc/>
        protected override int ExecuteNonQueryImplement(string sqlQuery, bool aCon, params object[] parameters)
        {
            if (transactionActive && aCon) throw new WrapSqlException("AutoConnect-methods (ACon) are not allowed durring a transaction!");

            using (OdbcCommand command = new OdbcCommand(sqlQuery, (OdbcConnection)Connection))
            {
                if (transactionActive) command.Transaction = (OdbcTransaction)transaction;
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

            using (OdbcCommand command = new OdbcCommand(sqlQuery, (OdbcConnection)Connection))
            {
                if (transactionActive) command.Transaction = (OdbcTransaction)transaction;
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                if (aCon) Open();
                object retval = command.ExecuteScalar();
                if (aCon) Close();
                if (retval is null && Nullable.GetUnderlyingType(typeof(T)) == null && SkalarDefaultOnNull) return default(T);
                else return (T)Convert.ChangeType(retval, typeof(T));
            }
        }

        ///<inheritdoc/>
        protected override OdbcDataReader ExecuteQueryImplement(string sqlQuery, params object[] parameters)
        {
            OdbcCommand command = new OdbcCommand(sqlQuery, (OdbcConnection)Connection);
            foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
            return command.ExecuteReader();
        }

        ///<inheritdoc/>
        protected override OdbcDataAdapter GetDataAdapterImplement(string sqlQuery, params object[] parameters)
        {
            using (OdbcCommand command = new OdbcCommand(sqlQuery, (OdbcConnection)Connection))
            {
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                return new OdbcDataAdapter(command);
            }
        }

        ///<inheritdoc/>
        protected override DataTable CreateDataTableImplement(string sqlQuery, params object[] parameters)
        {
            using (OdbcCommand command = new OdbcCommand(sqlQuery, (OdbcConnection)Connection))
            {
                foreach (object parameter in parameters) command.Parameters.AddWithValue(string.Empty, parameter);
                using (OdbcDataAdapter da = new OdbcDataAdapter(command))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
    }
}