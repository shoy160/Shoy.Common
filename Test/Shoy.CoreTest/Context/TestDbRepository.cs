using Shoy.Core;
using Shoy.Core.Domain.Entities;
using Shoy.Core.Domain.Repositories;
using Shoy.Data.EntityFramework;

namespace Shoy.CoreTest.Context
{
    public interface ITestDbRepository<TEntity> : IRepository<TEntity, long>
        where TEntity : class, IDEntity<long> { }

    public interface ITestDbRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IDEntity<TPrimaryKey> { }

    public class TestDbRepository<TEntity, TKey>
        : EfRepository<TestDbContext, TEntity, TKey>, ITestDbRepository<TEntity, TKey>
        where TEntity : class,IDEntity<TKey>
    {
        public TestDbRepository(IDbContextProvider<TestDbContext> unitOfWork)
            : base(unitOfWork)
        {
        }
    }

    public class TestDbRepository<TEntity>
        : TestDbRepository<TEntity, long>, ITestDbRepository<TEntity>
        where TEntity : class, IDEntity<long>
    {
        public TestDbRepository(IDbContextProvider<TestDbContext> unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
