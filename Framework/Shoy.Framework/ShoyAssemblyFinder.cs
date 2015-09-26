using Shoy.Core;
using Shoy.Core.Reflection;
using Shoy.Utility;

namespace Shoy.Framework
{
    public class ShoyAssemblyFinder : DefaultAssemblyFinder
    {
        public ShoyAssemblyFinder()
            : base(Consts.AssemblyFinder)
        {
        }

        public static ShoyAssemblyFinder Instance
        {
            get
            {
                return
                    Singleton<ShoyAssemblyFinder>.Instance ??
                    (Singleton<ShoyAssemblyFinder>.Instance = new ShoyAssemblyFinder());
            }
        }
    }
}
