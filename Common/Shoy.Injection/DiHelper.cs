using Autofac;
using System.Web.Mvc;

namespace Shoy.Injection
{
    /// <summary>
    /// 依赖注入辅助类
    /// </summary>
    public class DiHelper
    {
        private static IContainer _instance;

        public static void Register(IContainer container)
        {
            _instance = container;
        }

        public static T Resolve<T>()
        {
            using (var scope = _instance.BeginLifetimeScope())
            {
                return scope.Resolve<T>();
            }
        }

        public static T ResolvePerRequest<T>()
        {
            var current = DependencyResolver.Current;
            if (current != null)
            {
                var service = current.GetService<T>();
                if (service != null)
                    return service;
            }

            return Resolve<T>();
        }
    }
}
