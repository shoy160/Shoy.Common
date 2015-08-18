
using System.Data.Entity;
using Shoy.Data.EntityFramework;

namespace Shoy.MvcDemo.Models
{
    public class UserDbContext : CodeFirstDbContext
    {
        public UserDbContext()
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        /// <summary>
        /// 使用连接名称或连接字符串 初始化一个<see cref="CodeFirstDbContext"/>类型的新实例
        /// </summary>
        public UserDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<MClass> Classes { get; set; }
    }
}