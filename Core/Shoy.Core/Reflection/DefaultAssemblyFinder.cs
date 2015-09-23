
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shoy.Utility;

namespace Shoy.Core.Reflection
{
    public class DefaultAssemblyFinder : IAssemblyFinder
    {
        private const string PrefixName = "dayeasy";
        public static DefaultAssemblyFinder Instance
        {
            get
            {
                return Singleton<DefaultAssemblyFinder>.Instance ??
                       (Singleton<DefaultAssemblyFinder>.Instance = new DefaultAssemblyFinder());
            }
        }

        public IEnumerable<Assembly> FindAll()
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(t => t.FullName.StartsWith(PrefixName, StringComparison.CurrentCultureIgnoreCase));
        }

        public IEnumerable<Assembly> Find(Func<Assembly, bool> expression)
        {
            return FindAll().Where(expression);
        }
    }
}
