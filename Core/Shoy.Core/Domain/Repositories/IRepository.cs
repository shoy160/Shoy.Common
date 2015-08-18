using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Shoy.Core.Domain.Entities;

namespace Shoy.Core.Domain.Repositories
{
    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class, IEntity<int>
    { }

    /// <summary>
    /// 实体仓储模型的数据标准操作
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public interface IRepository<TEntity, TKey> : IDependency where TEntity : class ,IEntity<TKey>
    {
        #region Select/Get/Query
        IQueryable<TEntity> Table();

        List<TEntity> TableList();
        Task<List<TEntity>> TableListAsync();

        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate);

        Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> predicate);

        T Get<T>(Func<IQueryable<TEntity>, T> queryMethod);

        TEntity Get(TKey id);

        Task<TEntity> GetAsync(TKey id);

        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity FirstOrDefault(TKey id);

        Task<TEntity> FirstOrDefaultAsync(TKey id);

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity Load(TKey id);

        #endregion

        #region Insert

        TEntity Insert(TEntity entity);

        Task<TEntity> InsertAsync(TEntity entity);

        TKey InsertAndGetId(TEntity entity);

        Task<TKey> InsertAndGetIdAsync(TEntity entity);

        TEntity InsertOrUpdate(TEntity entity);

        Task<TEntity> InsertOrUpdateAsync(TEntity entity);

        TKey InsertOrUpdateAndGetId(TEntity entity);

        Task<TKey> InsertOrUpdateAndGetIdAsync(TEntity entity);

        #endregion

        #region Update

        TEntity Update(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        TEntity Update(TKey id, Action<TEntity> updateAction);

        Task<TEntity> UpdateAsync(TKey id, Func<TEntity, Task> updateAction);

        #endregion

        #region Delete

        void Delete(TEntity entity);

        Task DeleteAsync(TEntity entity);

        void Delete(TKey id);

        Task DeleteAsync(TKey id);

        void Delete(Expression<Func<TEntity, bool>> predicate);

        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Aggregates

        int Count();

        Task<int> CountAsync();

        int Count(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        long LongCount();

        Task<long> LongCountAsync();

        long LongCount(Expression<Func<TEntity, bool>> predicate);

        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region sql
        List<TEntity> SqlQuery(string sql, params object[] parameters);

        Task<List<TEntity>> SqlQueryAsync(string sql, params object[] parameters);

        int SqlExecute(string sql, params object[] parameters);
        Task<int> SqlExecuteAsync(string sql, params object[] parameters);

        #endregion
    }
}