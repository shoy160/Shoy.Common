using Autofac;
using Moq;
using Shoy.Framework;

namespace Shoy.CoreTest.Context
{
    public abstract class TestBase
    {
        protected ShoyBootstrap Bootstrap { get; private set; }
        protected IContainer Container { get; private set; }

        protected TestBase()
        {
            Bootstrap = ShoyBootstrap.Instance;
            //Bootstrap.BuilderHandler += build =>
            //{
            //    build.RegisterGeneric(typeof(TestDbRepository<>)).As(typeof(ITestDbRepository<>));
            //    build.RegisterGeneric(typeof(TestDbRepository<,>)).As(typeof(ITestDbRepository<,>));
            //};
            Bootstrap.Initialize();
            Container = Bootstrap.Container;
        }
    }
}
