using System.Text;

namespace WrapSQL
{
    /// <summary>
    /// Object containing any data relevant for establishing MySQL-connections
    /// </summary>
    public class WrapMySQLData
    {
        /// <summary>
        /// Default MySQL-server port
        /// </summary>
        public const int DefaultMySQLPort = 3306;

        /// <summary>
        /// Default MySQL-SSL mode (none).
        /// </summary>
        public const string DefaultSSLMode = "none";

        /// <summary>
        /// Hostname of the MySQL-Database (localhost / www.sample.org / 10.0.0.123)
        /// </summary>
        public string Hostname { get; set; } = string.Empty;

        /// <summary>
        /// Name of the MySQL-Database
        /// </summary>
        public string Database { get; set; } = string.Empty;

        /// <summary>
        /// Username for the MySQL-Authentication
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password for the MySQL-Authentication
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// SSL-Mode of the MySQL-connection
        /// </summary>
        public string SSLMode { get; set; } = "none";

        /// <summary>
        /// Port of the MySQL-server
        /// </summary>
        public int Port { get; set; } = 3306;

        /// <summary>
        /// Enable/Disable connection pooling
        /// </summary>
        public bool Pooling { get; set; } = true;

        /// <summary>
        /// Sets the connection timeout
        /// </summary>
        public int ConnectionTimeout { get; set; } = -1;

        /// <summary>
        /// Sets the command execution timeout
        /// </summary>
        public int CommandTimeout { get; set; } = -1;

        /// <summary>
        /// Specifies the charset which should be used
        /// </summary>
        public string CharSet { get; set; } = string.Empty;

        /// <summary>
        /// Enable/Disable prepared statements
        /// </summary>
        public bool IgnorePrepare { get; set; } = false;

        /// <summary>
        /// Creates a new MySQL-data object
        /// </summary>
        public WrapMySQLData()
            : this(string.Empty, string.Empty, string.Empty, string.Empty) { }

        /// <summary>
        /// Creates a new MySQL-data object
        /// </summary>
        /// <param name="Hostname">Hostname or IP of the server</param>
        /// <param name="Database">Target database</param>
        /// <param name="Username">Login username</param>
        /// <param name="Password">Login password</param>
        public WrapMySQLData(string Hostname, string Database, string Username, string Password)
        {
            this.Hostname = Hostname;
            this.Database = Database;
            this.Username = Username;
            this.Password = Password;
        }

        /// <summary>
        /// Creates a MySQL connection string
        /// </summary>
        /// <returns>MySQL connection string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"Server={Hostname};");

            if (Port != -1) sb.Append($"Port={Port};");

            sb.Append($"Database={Database};");
            sb.Append($"Uid={Username};");
            sb.Append($"Pwd={Password};");

            if (!string.IsNullOrEmpty(SSLMode)) sb.Append($"SslMode={SSLMode};");

            if (!Pooling) sb.Append("Pooling=False;");

            if (ConnectionTimeout != -1) sb.Append($"Connection Timeout={ConnectionTimeout};");

            if (CommandTimeout != -1) sb.Append($"default command timeout={CommandTimeout};");

            if (IgnorePrepare) sb.Append("Ignore Prepared=True;");

            if (!string.IsNullOrEmpty(CharSet)) sb.Append($"CharSet={CharSet};");

            return sb.ToString();
        }
    }
}
