using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Shoy.Core.Domain;
using Shoy.Core.Domain.Entities;
using Shoy.Core.Domain.Repositories;
using Shoy.Utility.Extend;

namespace Shoy.Data.EntityFramework
{
    public class EfRepository<TDbContext, TEntity, TKey> : DRepository<TEntity, TKey>
        where TEntity : class, IDEntity<TKey>
        where TDbContext : IUnitOfWork
    {
        private readonly DbSet<TEntity> _dbSet;

        public EfRepository(IDbContextProvider<TDbContext> unitOfWork)
            : base(unitOfWork.DbContext)
        {
            _dbSet = ((DbContext)UnitOfWork).Set<TEntity>();
        }

        private DbEntityEntry<TEntity> Entry(TEntity entity)
        {
            return ((DbContext)UnitOfWork).Entry(entity);
        }

        public override IQueryable<TEntity> Table
        {
            get { return _dbSet.AsNoTracking(); }
        }

        public override TKey Insert(TEntity entity)
        {
            var item = _dbSet.Add(entity);
            return SaveChanges() > 0 ? item.Id : default(TKey);
        }

        public override int Insert(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
            return SaveChanges();
        }

        public override int Delete(TEntity entity)
        {
            AttachIfNot(entity);
            Entry(entity).State = EntityState.Deleted;
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
            AttachIfNot(entity);
            Entry(entity).State = EntityState.Deleted;
            return SaveChanges();
        }

        public override int Delete(Expression<Func<TEntity, bool>> expression)
        {
            var entities = Where(expression);
            foreach (var entity in entities)
            {
                AttachIfNot(entity);
                Entry(entity).State = EntityState.Deleted;
            }
            return SaveChanges();
        }

        public override int Update(TEntity entity)
        {
            AttachIfNot(entity);
            var entry = Entry(entity);
            if (entry.State != EntityState.Modified)
                entry.State = EntityState.Modified;
            return SaveChanges();
        }

        private void Update(IEnumerable<TEntity> entities, ICollection<string> props)
        {
            entities.Foreach(t =>
            {
                AttachIfNot(t);
                var entry = Entry(t);
                entry.State = EntityState.Unchanged;
                foreach (var member in props)
                {
                    entry.Property(member).IsModified = true;
                }
            });
        }

        public override int Update(TEntity entity, params string[] parms)
        {
            if (entity == null || parms == null || parms.Length == 0)
                return 0;
            Update(new[] { entity }, parms);
            return SaveChanges();
        }

        public override int Update(TEntity entity, IQueryable<TEntity> entities, params string[] parms)
        {
            if (parms == null || parms.Length == 0)
                return 0;
            var props = entity.GetType().GetProperties().Where(p => parms.Contains(p.Name)).ToList();
            if (!props.Any())
                return 0;
            var list = entities.ToList();
            list.Foreach(t =>
            {
                foreach (var prop in props)
                {
                    prop.SetValue(t, prop.GetValue(entity, null));
                }
            });
            Update(list, parms);
            return SaveChanges();
        }

        public override int Update(Expression<Func<TEntity, dynamic>> propExpression, params TEntity[] entities)
        {
            if (entities == null || entities.Length == 0)
                return 0;
            ReadOnlyCollection<MemberInfo> memberInfos = ((dynamic)propExpression.Body).Members;
            if (memberInfos == null || !memberInfos.Any())
                return 0;
            Update(entities, memberInfos.Select(p => p.Name).ToList());
            return SaveChanges();
        }

        protected virtual void AttachIfNot(TEntity entity)
        {
            var dc = (IObjectContextAdapter)((DbContext)UnitOfWork);
            var oc = dc.ObjectContext;
            //DbSet名称要和实体名称一致
            var key = oc.CreateEntityKey(typeof(TEntity).Name, entity);
            ObjectStateEntry ose;
            if (oc.ObjectStateManager.TryGetObjectStateEntry(key, out ose))
            {
                var item = (TEntity)ose.Entity;
                Entry(item).State = EntityState.Detached;
            }
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
