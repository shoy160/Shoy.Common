using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Data;
using Shoy.Data.Builder;
using Shoy.Utility.Extend;
using System;
using System.Data.OracleClient;

namespace ShoyTest
{
    [TestClass]
    public class CommonTest
    {
        public class OracalTemplate :
            DriverTemplate<OracleConnection, OracleCommand, OracleDataAdapter, OracleParameter, OracalBuilder>
        {
        }

        [TestMethod]
        public void TestMethod1()
        {
            DbContext.AddConnection("", "", new OracalTemplate());
            using (var db = DbContext.Get())
            {
                SQL sql = "";
                var tb = sql.ExecuteDataTable(db);
                Console.WriteLine(tb.ToJson());
            }
        }
    }
}
