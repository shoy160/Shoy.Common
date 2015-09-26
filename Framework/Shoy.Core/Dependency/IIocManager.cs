
using System;

namespace Shoy.Core.Dependency
{
    public interface IIocManager : IDependency
    {
        T Resolve<T>();
        object Resolve(Type type);
    }
}
