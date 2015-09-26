using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Shoy.Core;
using Shoy.Core.Domain;
using Shoy.Core.Domain.Entities;
using Shoy.Core.Domain.Repositories;

namespace Shoy.Data.EntityFramework
{
    public class EfRepository<TDbContext, TEntity, TKey>
        : DRepository<TEntity, TKey>, IRepository<TDbContext, TEntity, TKey>, IDependency
        where TEntity : DEntity<TKey>
        where TDbContext : IUnitOfWork
    {
        private readonly DbSet<TEntity> _dbSet;

        public EfRepository(TDbContext unitOfWork)
            : base(unitOfWork)
        {
            _dbSet = ((DbContext)UnitOfWork).Set<TEntity>();
        }

        public override IQueryable<TEntity> Table
        {
            get { return _dbSet; }
        }

        public override TKey Insert(TEntity entity)
        {
            var item = _dbSet.Add(entity);
            return SaveChanges() > 0 ? item.Id : default(TKey);
        }

        public override int Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            return SaveChanges();
        }

        public override int Delete(TKey key)
        {
            var entity = _dbSet.Local.FirstOrDefault(t => EqualityComparer<TKey>.Default.Equals(t.Id, key));
            if (entity == null)
            {
                entity = Load(key);
                if (entity == null)
                    return 0;
            }
            Delete(entity);
            return SaveChanges();
        }

        public override int Update(TEntity entity)
        {
            AttachIfNot(entity);
            ((DbContext)UnitOfWork).Entry(entity).State = EntityState.Modified;
            return SaveChanges();
        }

        public override int Update(TEntity entity, Expression<Func<TEntity, bool>> expression)
        {
            throw new NotImplementedException();
        }

        protected virtual void AttachIfNot(TEntity entity)
        {
            if (!_dbSet.Local.Contains(entity))
            {
                _dbSet.Attach(entity);
            }
        }

        private int SaveChanges()
        {
            return UnitOfWork.IsTransaction ? 0 : UnitOfWork.SaveChanges();
        }
    }
}
