using System;
using System.Collections.Generic;
using System.Text;

namespace WrapSQL
{
    public class WrapSQLException : Exception
    {
        internal WrapSQLException() { }

        internal WrapSQLException(string message) : base(MessageFormat(message)) { }

        internal WrapSQLException(string message, Exception inner) : base(MessageFormat(message), inner) { }

        private static string MessageFormat(string message)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Warning: WrapSQL-Exception thrown!");
            sb.AppendLine("WrapSQL encountered an error. See below for further information:");
            if (string.IsNullOrEmpty(message)) sb.AppendLine("-- No information given. --");
            else sb.AppendLine(message);

            return sb.ToString();
        }
    }
}
