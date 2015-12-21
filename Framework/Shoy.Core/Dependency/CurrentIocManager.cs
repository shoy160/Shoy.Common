using Shoy.Core.Domain;
using Shoy.Core.Domain.Entities;
using Shoy.Core.Domain.Repositories;

namespace Shoy.Core.Dependency
{
    public static class CurrentIocManager
    {
        public static IIocManager IocManager { get; internal set; }

        public static T Resolve<T>()
        {
            return IocManager.Resolve<T>();
        }

        public static IRepository<TEntity, string> Repository<TDbContext, TEntity>()
            where TDbContext : IUnitOfWork
            where TEntity : DEntity<string>
        {
            return IocManager.Resolve<IRepository<TEntity, string>>();
        }

        public static IRepository<TEntity, TKey> Repository<TDbContext, TEntity, TKey>()
            where TDbContext : IUnitOfWork
            where TEntity : DEntity<TKey>
        {
            return IocManager.Resolve<IRepository<TEntity, TKey>>();
        }
    }
}
