using System.Data.Entity;

namespace Shoy.Data.EntityFramework
{
    public sealed class SimpleDbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
        where TDbContext : DbContext
    {
        public TDbContext DbContext { get; private set; }

        public SimpleDbContextProvider(TDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}
