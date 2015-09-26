using System;

namespace Shoy.Core.Reflection
{
    /// <summary> 类型查找器 </summary>
    public interface ITypeFinder : IDependency
    {
        Type[] Find(Func<Type, bool> expression);

        Type[] FindAll();
    }
}
