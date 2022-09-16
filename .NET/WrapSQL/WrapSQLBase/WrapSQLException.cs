using System;
using System.Text;

namespace WrapSql
{
    /// <summary>
    /// Exception-Class for WrapSQL. Internal use only.
    /// </summary>
    public class WrapSqlException : Exception
    {
        /// <summary>
        /// Creates a new WrapSQL-Exception
        /// </summary>
        public WrapSqlException() { }

        /// <summary>
        /// Creates a new WrapSQL-Exception
        /// </summary>
        /// <param name="message">Exception-message</param>
        public WrapSqlException(string message) : base(MessageFormat(message)) { }

        /// <summary>
        /// Creates a new WrapSQL-Exception
        /// </summary>
        /// <param name="message">Exception-message</param>
        /// <param name="inner">Inner exception</param>
        public WrapSqlException(string message, Exception inner) : base(MessageFormat(message), inner) { }

        /// <summary>
        /// Formats the given message and adds a custom exception-header.
        /// </summary>
        /// <param name="message">Exception-message</param>
        /// <returns>Formated exception message</returns>
        public static string MessageFormat(string message)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Warning: WrapSql-Exception thrown!");
            sb.AppendLine("WrapSql encountered an error. See below for further information:");
            if (string.IsNullOrEmpty(message)) sb.AppendLine("-- No information given. --");
            else sb.AppendLine(message);

            return sb.ToString();
        }
    }
}
