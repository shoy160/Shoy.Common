using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Data;
using Shoy.Utility.Extend;
using Npgsql;

namespace Shoy.Test
{
    /// <summary>
    /// ShoyDataTest 的摘要说明
    /// </summary>
    [TestClass]
    public class ShoyDataTest
    {
        public ShoyDataTest()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性:
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        public class Product
        {
            public int B2BProductId { get; set; }
            public int V9MemberId { get; set; }
            public string Title { get; set; }
        }

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Info { get; set; }
        }

        [TestMethod]
        public void TestMethod1()
        {
            SQL sql = "select top 5 * from B2BProduct";
            var list = sql.List<Product>(DbContext.Get());
            Console.Write(list.ToJson());
        }

        public class PostgreDriver :
            DriverTemplate<NpgsqlConnection, NpgsqlCommand, NpgsqlDataAdapter, NpgsqlParameter, PostgreBuilder>
        { }

        [TestMethod]
        public void PostgreTest()
        {
            const string connect =
                "Server=192.168.157.130;Port=5432;Database=ShoyDB;Userid=postgres;Password=123456; Protocol=3;Pooling=true;MinPoolSize=1;MaxPoolSize=50;ConnectionLifeTime=30;";
            DbContext.AddConnection("postgre", connect, new PostgreDriver());
            SQL sql = "select [Id],[Info]->'name' as [Name] from [User]";
            var list = sql.List<User>(DbContext.Get("postgre"));
            Console.Write(list.ToJson());
        }
    }
}
