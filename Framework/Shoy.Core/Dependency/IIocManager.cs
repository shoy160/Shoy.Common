
using System;

namespace Shoy.Core.Dependency
{
    public interface IIocManager : ILifetimeDependency
    {
        T Resolve<T>();
        object Resolve(Type type);
    }
}
