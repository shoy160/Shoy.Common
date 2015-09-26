using Autofac;
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
            Bootstrap.Initialize();
            Container = Bootstrap.Container;
        }
    }
}
