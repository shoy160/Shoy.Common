using System;
using System.Data.Entity;

namespace Shoy.Data.EntityFramework
{
    public interface IDbContextProvider<out TDbContext>
        where TDbContext : DbContext
    {
        TDbContext DbContext { get; }
    }
}
