using System;
using System.Collections.Generic;
using System.Text;

namespace WrapSQL
{
    public class WrapSQLException : Exception
    {
        public WrapSQLException() { }

        public WrapSQLException(string message) : base(MessageFormat(message)) { }

        public WrapSQLException(string message, Exception inner) : base(MessageFormat(message), inner) { }

        public static string MessageFormat(string message)
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
