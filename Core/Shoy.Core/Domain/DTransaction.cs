using System;
using System.Transactions;

namespace Shoy.Core.Domain
{
    /// <summary> 分布式事务辅助类 </summary>
    public class DTransaction : IDisposable
    {
        private readonly TransactionScope _transaction;

        private DTransaction(IsolationLevel level = IsolationLevel.ReadUncommitted, TimeSpan? timeout = null)
        {
            var options = new TransactionOptions
            {
                IsolationLevel = level
            };
            if (timeout.HasValue)
                options.Timeout = timeout.Value;
            _transaction = new TransactionScope(TransactionScopeOption.Required, options);
        }

        private void SaveChanges()
        {
            if (_transaction != null)
                _transaction.Complete();
        }

        /// <summary> 使用分布式事务 </summary>
        /// <param name="action"></param>
        /// <param name="level"></param>
        /// <param name="timeout"></param>
        public static void Use(Action action, IsolationLevel level = IsolationLevel.ReadUncommitted,
            TimeSpan? timeout = null)
        {
            using (var trans = new DTransaction(level, timeout))
            {
                action();
                trans.SaveChanges();
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
                _transaction.Dispose();
        }
    }
}
