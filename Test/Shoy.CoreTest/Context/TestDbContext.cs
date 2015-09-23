using System.Data.Entity;
using Shoy.CoreTest.Context.Models;
using Shoy.Data.EntityFramework;

namespace Shoy.CoreTest.Context
{
    public class TestDbContext : CodeFirstDbContext
    {
        public TestDbContext()
            : base(GetConnection("user"), true)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            Database.SetInitializer<TestDbContext>(null);
        }

        public virtual DbSet<User> Users { get; set; }
    }
}
