using System.Threading.Tasks;

namespace Shoy.Core.Data
{
    /// <summary>
    /// 业务单元操作接口
    /// </summary>
    public interface IUnitOfWork : IDependency
    {
        /// <summary>
        /// 获取或设置 是否开启事务提交
        /// </summary>
        bool TransactionEnabled { get; set; }

        /// <summary>
        /// 提交当前单元操作的更改。
        /// </summary>
        /// <returns>操作影响的行数</returns>
        int SaveChanges();

        /// <summary>
        /// 异步提交当前单元操作的更改。
        /// </summary>
        /// <returns>操作影响的行数</returns>
        Task<int> SaveChangesAsync();
    }
}
