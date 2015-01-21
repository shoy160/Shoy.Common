using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using Shoy.Data;
using Shoy.Utility.Extend;

namespace Shoy.Test
{
    /// <summary>
    ///     ShoyDataTest 的摘要说明
    /// </summary>
    [TestClass]
    public class ShoyDataTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var db = DbContext.Get())
            {
                SQL sql = "select * from PackageCategory";
                var list = sql.List<PackageCategory>(db, new Region(1, 3));
                Console.Write(list.ToJson());
                Assert.AreNotEqual(list, null);
            }
        }

        [TestMethod]
        public void PostgreTest()
        {
            DbContext.DriversCache.Add("Postgre", new PostgreDriver());
            //const string connect =
            //    "Server=192.168.157.130;Port=5432;Database=ShoyDB;Userid=postgres;Password=123456; Protocol=3;Pooling=true;MinPoolSize=1;MaxPoolSize=50;ConnectionLifeTime=30;";
            //DbContext.AddConnection("postgre", connect, new PostgreDriver());
            using (var db = DbContext.Get("postgre"))
            {
                SQL sql = "select [Id],[Info]->'name' as [Name] from [User]";

                var list = sql.List<User>();
                Console.Write(list.ToJson());
                Assert.AreNotEqual(list, null);
            }
        }

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

        public class PackageCategory
        {
            public int CategoryId { get; set; }
            public string Name { get; set; }
            public byte State { get; set; }
            public DateTime CreateOn { get; set; }
        }

        public class PostgreDriver :
            DriverTemplate<NpgsqlConnection, NpgsqlCommand, NpgsqlDataAdapter, NpgsqlParameter, PostgreBuilder>
        {
        }
    }
}