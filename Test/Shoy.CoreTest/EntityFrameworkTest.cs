using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shoy.CoreTest.Context;
using Shoy.CoreTest.Context.Models;
using Shoy.Utility.Extend;

namespace Shoy.CoreTest
{
    [TestClass]
    public class EntityFrameworkTest : TestBase
    {
        private readonly ITestDbRepository<User> _userRepository;
        public EntityFrameworkTest()
        {
            _userRepository = Container.Resolve<ITestDbRepository<User>>();
        }

        [TestMethod]
        public void UserTest()
        {
            var regist = Container.IsRegistered<ITestDbRepository<User>>();
            Console.WriteLine(regist);
            var user = _userRepository.Load(1);
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
