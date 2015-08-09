using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IContainer = Autofac.IContainer;

namespace Shoy.Test
{
    [TestClass]
    public class DependencyTest
    {
        public IUserService Servive { get; set; }
        private static IContainer Container { get; set; }

        private static void AutofacRegisters()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            ContainerBuilder builder = new ContainerBuilder();
            //            builder.RegisterGeneric(typeof(Repository<,>)).As(typeof(IRepository<,>));

            Type baseType = typeof(IDependency);
            string path = Directory.GetCurrentDirectory();
            Assembly[] assemblies = Directory.GetFiles(path, "Shoy.*.dll").Select(Assembly.LoadFrom)
                .Union(new[] { Assembly.GetExecutingAssembly() }).ToArray();
            builder.RegisterAssemblyTypes(assemblies)
                .Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract)
                .AsSelf() //自身服务，用于没有接口的类
                .AsImplementedInterfaces() //接口服务
                .PropertiesAutowired() //属性注入
                .InstancePerLifetimeScope(); //保证生命周期基于请求

            Container = builder.Build();
            watch.Stop();
            Console.WriteLine("register use {0}ms", watch.ElapsedMilliseconds);
        }

        public DependencyTest()
        {
            AutofacRegisters();
            Servive = Container.Resolve<IUserService>();
        }

        [TestMethod]
        public void Test()
        {
            Servive.WriteUser();
        }
    }
}
