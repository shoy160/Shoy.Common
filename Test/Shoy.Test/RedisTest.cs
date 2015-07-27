using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Utility;
using Shoy.Utility.Extend;
using StackExchange.Redis;

namespace Shoy.Test
{
    [TestClass]
    public class RedisTest
    {
        [TestMethod]
        public void Test()
        {
            var c = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                Ssl = false,
                Password = "deyi@168",
                EndPoints = { { "127.0.0.1", 6379 } }
            });
            var shoy = c.GetDatabase().StringGet("shoy");
            Console.WriteLine(shoy);
        }

        [TestMethod]
        public void Test01()
        {
            const string key = "shoy";
            var client = RedisManager.Instance.GetClient();
            //            Console.WriteLine(client.Get<string>(key));
            //            client.Remove(key);
            client.Set(key, new DResult("测试阶段003"), TimeSpan.FromMinutes(20));
            var result = client.Get<DResult>(key);
            Console.WriteLine(result.ToJson());
        }
    }
}
