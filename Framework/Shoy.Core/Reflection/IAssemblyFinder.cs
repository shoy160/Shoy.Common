
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Shoy.Core.Reflection
{
    /// <summary> 程序集查找器 </summary>
    public interface IAssemblyFinder : IDependency
    {
        /// <summary> 查找所有程序集 </summary>
        /// <returns></returns>
        IEnumerable<Assembly> FindAll();

        IEnumerable<Assembly> Find(Func<Assembly, bool> expression);
    }
}
