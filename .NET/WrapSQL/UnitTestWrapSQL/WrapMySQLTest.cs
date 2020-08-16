using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WrapSQL;

namespace UnitTestWrapSQL
{
    [TestClass]
    public class WrapMySQLTest
    {
        readonly string dbHost = "";
        readonly string sbName = "";
        readonly string dbUser = "";
        readonly string dbPass = "";

        [TestMethod]
        public void WrapMySQL_ConnectionTest_ExpectSuccess()
        {
            using(WrapMySQL sql = new WrapMySQL(dbHost, sbName, dbUser, dbPass))
            {
                sql.Open();
                sql.Close();
            }
        }

        [TestMethod]
        public void WrapMySQL_SQLScalarTypedInt_Success()
        {
            using (WrapMySQL sql = new WrapMySQL(dbHost, sbName, dbUser, dbPass))
            {
                sql.Open();
                int res = sql.ExecuteScalar<int>("SELECT COUNT(*) FROM qd_drives");
                sql.Close();
                Assert.AreEqual(10, res);
            } 
        }

        [TestMethod]
        public void WrapMySQL_SQLScalarTypedString_Success()
        {
            using (WrapMySQL sql = new WrapMySQL(dbHost, sbName, dbUser, dbPass))
            {
                sql.Open();
                string res = sql.ExecuteScalar<string>("SELECT QDValue FROM qd_info WHERE QDKey = ?", "DefaultDomain");
                sql.Close();
                Assert.AreEqual("endevx", res);
            }
        }

        [TestMethod]
        public void WrapMySQL_SQLScalarUntypedInt_Success()
        {
            using (WrapMySQL sql = new WrapMySQL(dbHost, sbName, dbUser, dbPass))
            {
                sql.Open();
                object res = sql.ExecuteScalar("SELECT COUNT(*) FROM qd_drives");
                sql.Close();
                Assert.AreEqual(10, Convert.ToInt32(res));
            }
        }

        [TestMethod]
        public void WrapMySQL_SQLScalarUntypedString_Success()
        {
            using (WrapMySQL sql = new WrapMySQL(dbHost, sbName, dbUser, dbPass))
            {
                sql.Open();
                object res = sql.ExecuteScalar("SELECT QDValue FROM qd_info WHERE QDKey = ?", "DefaultDomain");
                sql.Close();
                Assert.AreEqual("endevx", Convert.ToString(res));
            }
        }

        [TestMethod]
        public void WrapMySQL_SQLScalarTypedIntACon_Success()
        {
            using (WrapMySQL sql = new WrapMySQL(dbHost, sbName, dbUser, dbPass))
            {
                int res = sql.ExecuteScalarACon<int>("SELECT COUNT(*) FROM qd_drives");
                Assert.AreEqual(10, res);
            }
        }

        [TestMethod]
        public void WrapMySQL_SQLScalarTypedStringACon_Success()
        {
            using (WrapMySQL sql = new WrapMySQL(dbHost, sbName, dbUser, dbPass))
            {
                string res = sql.ExecuteScalarACon<string>("SELECT QDValue FROM qd_info WHERE QDKey = ?", "DefaultDomain");
                Assert.AreEqual("endevx", res);
            }
        }

        [TestMethod]
        public void WrapMySQL_SQLScalarUntypedIntACon_Success()
        {
            using (WrapMySQL sql = new WrapMySQL(dbHost, sbName, dbUser, dbPass))
            {
                object res = sql.ExecuteScalarACon("SELECT COUNT(*) FROM qd_drives");
                Assert.AreEqual(10, Convert.ToInt32(res));
            }
        }

        [TestMethod]
        public void WrapMySQL_SQLScalarUntypedStringACon_Success()
        {
            using (WrapMySQL sql = new WrapMySQL(dbHost, sbName, dbUser, dbPass))
            {
                object res = sql.ExecuteScalarACon("SELECT QDValue FROM qd_info WHERE QDKey = ?", "DefaultDomain");
                Assert.AreEqual("endevx", Convert.ToString(res));
            }
        }
    }
}
