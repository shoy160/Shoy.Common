using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Core.Logging;
using Shoy.Utility;
using Shoy.Utility.Logging;
using System;

namespace Shoy.CoreTest
{
    [TestClass]
    public class UnitTest1
    {
        private readonly ILogger _logger = LogManager.GetLogger<UnitTest1>();

        static UnitTest1()
        {
            var adapter = new Log4NetAdapter();
            LogManager.AddAdapter(adapter);
        }

        [TestMethod]
        public void TestMethod1()
        {
            _logger.Trace("Log test!");
            _logger.Debug("Log Debug!");
            _logger.Info("Log Info!");
            _logger.Warn("Log Warn!");
            var msg = LogManager.Format("Log Error!");
            _logger.Error(msg + Utils.GetIp());
            _logger.Error("Log Error!", new Exception("Log Error!"));
            _logger.Fatal("Log Fatal!");
            _logger.Fatal("Log Fatal!", new Exception("Log Fatal!"));

        }
    }
}
