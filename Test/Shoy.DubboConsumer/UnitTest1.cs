using System;
using com.shoy.dubbo.api;
using Damai.Dubbo.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Utility.Helper;

namespace Shoy.DubboConsumer
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var client = new DubboClient("net-consumer", "zookeeper", "zookeeper://192.168.10.14:2181", 2181);
            using (var reference = new ServiceReference<ShoyService>())
            {
                reference.DubboClient = client;
                reference.Registry = client.Registry;
                reference.Timeout = 1000;
                var service = reference.Get();
                var word = service.sayHello("shoy");
                Console.WriteLine(word);
                var number = service.add(4, 34);
                Console.WriteLine(number);
                var user = service.getUser();
                Console.WriteLine(user.getId());
                Console.WriteLine(JsonHelper.ToJson(user, NamingType.CamelCase, true));
                Assert.AreNotEqual(user, null);
            }
        }
    }
}
