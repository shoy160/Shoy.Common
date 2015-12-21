using Shoy.Core;
using Shoy.Core.Modules;
using Shoy.CoreTest.Context;
using Shoy.Data.EntityFramework;

namespace Shoy.CoreTest
{
    [DependsOn(typeof(CoreModule))]
    public class TestModule : DModule
    {
        public override void Initialize()
        {
            DatabaseInitializer.Initialize(IocManager.Resolve<IDbContextProvider<TestDbContext>>().DbContext);
        }
    }
}
