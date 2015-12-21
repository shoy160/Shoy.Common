using System;
using Autofac;
using Shoy.Core.Dependency;

namespace Shoy.Framework
{
    public class IocManager : IIocManager
    {
        private readonly ShoyBootstrap _bootstrap;

        public IocManager()
        {
            _bootstrap = ShoyBootstrap.Instance;
        }
        public T Resolve<T>()
        {
            return _bootstrap.Container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return _bootstrap.Container.Resolve(type);
        }
    }
}
