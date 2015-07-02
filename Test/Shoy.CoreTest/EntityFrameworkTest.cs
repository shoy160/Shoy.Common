using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shoy.Core.Domain.Entities;
using Shoy.EntityFramework;

namespace Shoy.CoreTest
{
    [TestClass]
    public class EntityFrameworkTest
    {
        public class User : Entity<long>, ISoftDelete, IAudited<long?>
        {
            public string Account { get; set; }
            public string Name { get; set; }
            public bool IsDeleted { get; set; }
            public DateTime CreationTime { get; set; }
            public long? CreatorId { get; set; }
            public string CreationIp { get; set; }
            public DateTime? LastModificationTime { get; set; }
            public long? LastModifierUserId { get; set; }
        }

        public void UserTest()
        {
            var context = new CodeFirstDbContext("");
            var user =
                new EfRepositoryBase<CodeFirstDbContext, User, long>(
                    new SimpleDbContextProvider<CodeFirstDbContext>(context));
        }
    }
}
