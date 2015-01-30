using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Shoy.Core.Data.Migrations
{
    /// <summary>
    /// 在数据库不存在时使用种子数据创建数据库
    /// </summary>
    public class CreateDatabaseIfNotExistsWithSeed : CreateDatabaseIfNotExists<CodeFirstDbContext>
    {
        static CreateDatabaseIfNotExistsWithSeed()
        {
            SeedActions = new List<ISeedAction>();
        }

        /// <summary>
        /// 获取 数据库创建时的种子数据操作信息集合，各个模块可以添加自己的初始化数据
        /// </summary>
        public static ICollection<ISeedAction> SeedActions { get; private set; }

        protected override void Seed(CodeFirstDbContext context)
        {
            IEnumerable<ISeedAction> seedActions = SeedActions.OrderBy(m => m.Order);
            foreach (ISeedAction seedAction in seedActions)
            {
                seedAction.Action(context);
            }
        }
    }
}