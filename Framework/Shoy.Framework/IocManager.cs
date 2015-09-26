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
            using (var scope = _bootstrap.Container.BeginLifetimeScope())
            {
                return scope.Resolve<T>();
            }
        }

        public object Resolve(Type type)
        {
            using (var scope = _bootstrap.Container.BeginLifetimeScope())
            {
                return scope.Resolve(type);
            }
        }
    }
}
