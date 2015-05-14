using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Core.Logging;
using Shoy.Utility;
using Shoy.Utility.Extend;
using Shoy.Utility.Helper;
using Shoy.Utility.Logging;
using System;

namespace Shoy.CoreTest
{
    [TestClass]
    public class UnitTest1
    {
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        //private readonly ILogger _logger = LogManager.GetLogger<UnitTest1>();

        static UnitTest1()
        {
            var adapter = new Log4NetAdapter();
            LogManager.AddAdapter(adapter);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var user = XmlHelper.XmlDeserialize<User>(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><Userdfd><Id>1002</Id><Name>dfsdf</Name></User>",
                Encoding.UTF8);
            Console.WriteLine((user ?? new User()).ToJson());

            //_logger.Trace("Log test!");
            //_logger.Debug("Log Debug!");
            //_logger.Info("Log Info!");
            //_logger.Warn("Log Warn!");
            //var msg = LogManager.Format("Log Error!");
            //_logger.Error(msg + Utils.GetIp());
            //_logger.Error("Log Error!", new Exception("Log Error!"));
            //_logger.Fatal("Log Fatal!");
            //_logger.Fatal("Log Fatal!", new Exception("Log Fatal!"));

        }
    }
}
