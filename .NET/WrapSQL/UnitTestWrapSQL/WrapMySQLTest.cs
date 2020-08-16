using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace UnitTestWrapSQL
{
    [TestClass]
    public class WrapMySQLTest
    {
        [TestMethod]
        public void WrapSQLException_ExceptionExpected()
        {
            using(WrapMySQL sql = new WrapMySQL("localhost", "test", "root", ""))
            {

            }
        }
    }
}
