using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.CoreTest.Context;
using Shoy.CoreTest.Context.Models;
using Shoy.Utility.Extend;

namespace Shoy.CoreTest
{
    [TestClass]
    public class EntityFrameworkTest : TestBase
    {
        [TestMethod]
        public void UserTest()
        {
            var regist = Container.IsRegistered<ITestDbRepository<User>>();
            Console.WriteLine(regist);

            var repository = Container.Resolve<ITestDbRepository<User>>();
            var user = repository.Load(1);
            Console.WriteLine(user.ToJson());

            Task.Factory.StartNew(() =>
            {
                var userRepository = Container.Resolve<ITestDbRepository<User>>();
                user = userRepository.Load(1);
                Console.WriteLine(user.ToJson());
            }).Wait();
        }
    }
}
