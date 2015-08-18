using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Core.Domain.Entities;
using Shoy.Data.EntityFramework;

namespace Shoy.CoreTest
{
    [TestClass]
    public class EntityFrameworkTest
    {
        public EntityFrameworkTest()
        {
        }

        public class User : Entity<long>, ISoftDelete, IAudited<long?>
        {
            public virtual string Account { get; set; }
            public virtual string Name { get; set; }
            public virtual bool IsDeleted { get; set; }
            public virtual DateTime CreationTime { get; set; }
            public virtual long? CreatorId { get; set; }
            public virtual string CreationIp { get; set; }
            public virtual DateTime? LastModificationTime { get; set; }
            public virtual long? LastModifierUserId { get; set; }
        }

        public void UserTest()
        {
            var context = new CodeFirstDbContext("");
            var provider = new SimpleDbContextProvider<CodeFirstDbContext>(context);
            var user = new EfRepositoryBase<CodeFirstDbContext, User, long>(provider);
            user.Insert(new User());
        }
    }
}
