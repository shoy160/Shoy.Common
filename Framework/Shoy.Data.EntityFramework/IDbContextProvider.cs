using Shoy.Core.Domain;

namespace Shoy.Data.EntityFramework
{
    public interface IDbContextProvider<out TDbContext>
        where TDbContext : IUnitOfWork
    {
        TDbContext DbContext { get; }
    }
}
