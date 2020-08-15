using System;
using System.Data;
using System.Data.Common;

namespace WrapMySQL
{
    public class WrapMySQL : WrapSQL.WrapSQLBase
    {
        protected override int ExecuteNonQuery(string sqlQuery, bool aCon, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        protected override T ExecuteScalar<T>(string sqlQuery, bool aCon, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public override DbDataReader ExecuteQuery(string sqlQuery, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public override DataAdapter GetDataAdapter(string sqlQuery, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public override DataTable CreateDataTable(string sqlQuery, params object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
