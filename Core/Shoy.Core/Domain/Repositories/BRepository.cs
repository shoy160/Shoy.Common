using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Shoy.Core.Domain.Entities;

namespace Shoy.Core.Domain.Repositories
{
    /// <summary> IRespository的基础实现 </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class BRepository<TEntity>
        : BRepository<TEntity, int>
        where TEntity : class,IEntity<int> { }
    public abstract class BRepository<TEntity, TKey>
        : IRepository<TEntity, TKey>
        where TEntity : class,IEntity<TKey>
    {
        public abstract IQueryable<TEntity> Tabel();

        public virtual List<TEntity> TabelList()
        {
            return Tabel().ToList();
        }

        public virtual Task<List<TEntity>> TabelListAsync()
        {
            return Task.FromResult(TabelList());
        }

        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate)
        {
            return Tabel().Where(predicate);
        }

        public virtual Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Query(predicate));
        }

        public virtual T Get<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return queryMethod(Tabel());
        }

        public virtual TEntity Get(TKey id)
        {
            var entity = FirstOrDefault(id);
            return entity;
        }

        public virtual async Task<TEntity> GetAsync(TKey id)
        {
            var entity = await FirstOrDefaultAsync(id);
            return entity;
        }

        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return Tabel().Single(predicate);
        }

        public virtual Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Single(predicate));
        }

        public virtual TEntity FirstOrDefault(TKey id)
        {
            return Tabel().FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(TKey id)
        {
            return Task.FromResult(FirstOrDefault(id));
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Tabel().FirstOrDefault(predicate);
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(FirstOrDefault(predicate));
        }

        public virtual TEntity Load(TKey id)
        {
            return Get(id);
        }

        public abstract TEntity Insert(TEntity entity);

        public virtual Task<TEntity> InsertAsync(TEntity entity)
        {
            return Task.FromResult(Insert(entity));
        }

        public virtual TKey InsertAndGetId(TEntity entity)
        {
            return Insert(entity).Id;
        }

        public virtual Task<TKey> InsertAndGetIdAsync(TEntity entity)
        {
            return Task.FromResult(InsertAndGetId(entity));
        }

        public virtual TEntity InsertOrUpdate(TEntity entity)
        {
            return EqualityComparer<TKey>.Default.Equals(entity.Id, default(TKey))
                ? Insert(entity)
                : Update(entity);
        }

        public virtual async Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            return EqualityComparer<TKey>.Default.Equals(entity.Id, default(TKey))
                ? await InsertAsync(entity)
                : await UpdateAsync(entity);
        }

        public virtual TKey InsertOrUpdateAndGetId(TEntity entity)
        {
            return InsertOrUpdate(entity).Id;
        }

        public virtual Task<TKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
        {
            return Task.FromResult(InsertOrUpdateAndGetId(entity));
        }

        public abstract TEntity Update(TEntity entity);

        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(Update(entity));
        }

        public virtual TEntity Update(TKey id, Action<TEntity> updateAction)
        {
            var entity = Get(id);
            updateAction(entity);
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TKey id, Func<TEntity, Task> updateAction)
        {
            var entity = await GetAsync(id);
            await updateAction(entity);
            return entity;
        }

        public abstract void Delete(TEntity entity);

        public virtual async Task DeleteAsync(TEntity entity)
        {
            Delete(entity);
        }

        public abstract void Delete(TKey id);

        public virtual async Task DeleteAsync(TKey id)
        {
            Delete(id);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in Tabel().Where(predicate).ToList())
            {
                Delete(entity);
            }
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Delete(predicate);
        }

        public virtual int Count()
        {
            return Tabel().Count();
        }

        public virtual Task<int> CountAsync()
        {
            return Task.FromResult(Count());
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Tabel().Where(predicate).Count();
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Count(predicate));
        }

        public virtual long LongCount()
        {
            return Tabel().LongCount();
        }

        public virtual Task<long> LongCountAsync()
        {
            return Task.FromResult(LongCount());
        }

        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return Tabel().Where(predicate).LongCount();
        }

        public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(LongCount(predicate));
        }

        public abstract List<TEntity> SqlQuery(string sql, params object[] parameters);

        public virtual Task<List<TEntity>> SqlQueryAsync(string sql, params object[] parameters)
        {
            return Task.FromResult(SqlQuery(sql, parameters));
        }

        public abstract int SqlExecute(string sql, params object[] parameters);

        public virtual Task<int> SqlExecuteAsync(string sql, params object[] parameters)
        {
            return Task.FromResult(SqlExecute(sql, parameters));
        }

        protected static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TKey))
                );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
    }
}
