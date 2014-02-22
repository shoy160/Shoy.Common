using System.Data.Entity;

namespace Shoy.MvcTest.Models
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}