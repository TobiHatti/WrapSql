﻿namespace WrapSQL
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
        public string Hostname { get; set; }

        /// <summary>
        /// Name of the MySQL-Database
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Username for the MySQL-Authentication
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password for the MySQL-Authentication
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// SSL-Mode of the MySQL-connection
        /// </summary>
        public string SSLMode { get; set; }

        /// <summary>
        /// Port of the MySQL-server
        /// </summary>
        public int Port { get; set; }

        public bool Pooling { get; set; }

        public int ConnectionTimeout { get; set; }

        public int CommandTimeout { get; set; }
       
        public string CharSet { get; set; }

        public bool IgnorePrepare { get; set; }

        /// <summary>
        /// Creates a new MySQL-data object
        /// </summary>
        public WrapMySQLData()
            : this(string.Empty, string.Empty, string.Empty, string.Empty, DefaultMySQLPort, DefaultSSLMode) { }

        /// <summary>
        /// Creates a new MySQL-data object
        /// </summary>
        /// <param name="Hostname">Hostname or IP of the server</param>
        /// <param name="Database">Target database</param>
        /// <param name="Username">Login username</param>
        /// <param name="Password">Login password</param>
        public WrapMySQLData(string Hostname, string Database, string Username, string Password)
            : this(Hostname, Database, Username, Password, DefaultMySQLPort, DefaultSSLMode) { }

        /// <summary>
        /// Creates a new MySQL-data object
        /// </summary>
        /// <param name="Hostname">Hostname or IP of the server</param>
        /// <param name="Database">Target database</param>
        /// <param name="Username">Login username</param>
        /// <param name="Password">Login password</param>
        /// <param name="Port">MySQL Server-port</param>
        /// <param name="SSLMode">SSL encryption mode</param>
        public WrapMySQLData(string Hostname, string Database, string Username, string Password, int Port, string SSLMode)
        {
            this.Hostname = Hostname;
            this.Database = Database;
            this.Username = Username;
            this.Password = Password;

            this.Port = Port;
            this.SSLMode = SSLMode;
        }
    }
}
