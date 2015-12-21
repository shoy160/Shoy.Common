using Shoy.Core.Domain.Entities;
using Shoy.Core.Domain.Repositories;
using Shoy.Data.EntityFramework;

namespace Shoy.Wiki.Contracts.Services
{
    public interface IWikiRepository<TEntity> : IRepository<TEntity, long>
        where TEntity : class, IDEntity<long> { }

    public interface IWikiRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IDEntity<TPrimaryKey> { }

    public class WikiRepository<TEntity, TKey>
        : EfRepository<WikiDbContext, TEntity, TKey>, IWikiRepository<TEntity, TKey>
        where TEntity : class,IDEntity<TKey>
    {
        public WikiRepository(IDbContextProvider<WikiDbContext> unitOfWork)
            : base(unitOfWork)
        {
        }
    }

    public class WikiRepository<TEntity>
        : WikiRepository<TEntity, long>, IWikiRepository<TEntity>
        where TEntity : class, IDEntity<long>
    {
        public WikiRepository(IDbContextProvider<WikiDbContext> unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}