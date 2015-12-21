using System;
using System.Collections.Concurrent;
using System.Runtime.Remoting.Messaging;
using Shoy.Core.Dependency;
using Shoy.Core.Domain;

namespace Shoy.Data.EntityFramework
{
    public class UnitOfWorkDbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
        where TDbContext : IUnitOfWork
    {
        private static readonly ConcurrentDictionary<string, IUnitOfWork> UnitOfWorkDictionary;
        private static readonly object LockObj = new object();

        static UnitOfWorkDbContextProvider()
        {
            UnitOfWorkDictionary = new ConcurrentDictionary<string, IUnitOfWork>();
        }

        public TDbContext DbContext
        {
            get
            {
                lock (LockObj)
                {
                    //首先去线程数据槽里去取数据
                    var key = typeof(TDbContext).FullName;
                    var unitOfWorkKey = CallContext.GetData(key) as string;
                    if (!string.IsNullOrWhiteSpace(unitOfWorkKey))
                    {
                        IUnitOfWork unitOfWork;
                        if (UnitOfWorkDictionary.TryGetValue(unitOfWorkKey, out unitOfWork))
                        {
                            return (TDbContext)unitOfWork;
                        }
                    }
                    //多数据库添加链接
                    var context = CurrentIocManager.Resolve<TDbContext>();

                    //放入线程数据槽
                    unitOfWorkKey = Guid.NewGuid().ToString();

                    if (!UnitOfWorkDictionary.TryAdd(unitOfWorkKey, context))
                    {
                        throw new Exception("Can not set unit of work!");
                    }

                    CallContext.SetData(key, unitOfWorkKey);

                    return context;
                }
            }
        }
    }
}
