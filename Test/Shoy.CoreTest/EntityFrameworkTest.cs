using System;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Core.Domain.Repositories;
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
            using (var scope = Container.BeginLifetimeScope())
            {
                var repository = scope.Resolve<IRepository<TestDbContext, User, long>>();
                var user = repository.Load(1);
                Console.WriteLine(user.ToJson());
            }
        }
    }
}
