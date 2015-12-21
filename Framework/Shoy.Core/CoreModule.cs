using Shoy.Core.Dependency;
using Shoy.Core.Modules;

namespace Shoy.Core
{
    public class CoreModule : DModule
    {
        public override void Initialize()
        {
            CurrentIocManager.IocManager = IocManager;
            base.Initialize();
        }
    }
}
