using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Core.Wcf;
using Shoy.CoreTest.Context;
using Shoy.CoreTest.Services;
using Shoy.Utility;
using Shoy.Utility.Extend;
using Shoy.Utility.Helper;
using Shoy.Utility.Logging;

namespace Shoy.CoreTest
{
    [TestClass]
    public class UnitTest1 : TestBase
    {
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        private readonly ILogger _logger = LogManager.Logger<UnitTest1>();

        static UnitTest1()
        {
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

        [TestMethod]
        public void ExecCommandTest()
        {
            Utils.ExecCommand(p =>
            {
                p(@"ping www.baidu.com");
                // 这里连续写入的命令将依次在控制台窗口中得到体现
                p("exit 0");
            }, Console.WriteLine);

            WcfHelper.Instance.Call<IUserService>(u =>
            {
                var word = u.Hello("shay");
                Console.WriteLine(word);
                var users = u.Users();
                Console.WriteLine(JsonHelper.ToJson(users));
            });
        }
    }
}
