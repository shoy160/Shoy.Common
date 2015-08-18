﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Shoy.Core.Domain.Entities;
using Shoy.Core.Logging;

namespace Shoy.Data.EntityFramework.Extensions
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// 更新上下文中指定的实体的状态
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">主键类型</typeparam>
        /// <param name="dbContext">上下文对象</param>
        /// <param name="entities">要更新的实体类型</param>
        public static void Update<TEntity, TKey>(this DbContext dbContext, params TEntity[] entities) where TEntity : Entity<TKey>
        {
            foreach (TEntity entity in entities)
            {
                DbSet<TEntity> dbSet = dbContext.Set<TEntity>();
                try
                {
                    DbEntityEntry<TEntity> entry = dbContext.Entry(entity);
                    if (entry.State == EntityState.Detached)
                    {
                        dbSet.Attach(entity);
                        entry.State = EntityState.Modified;
                    }
                }
                catch (InvalidOperationException)
                {
                    TEntity oldEntity = dbSet.Find(entity.Id);
                    dbContext.Entry(oldEntity).CurrentValues.SetValues(entity);
                }
            }
        }

        /// <summary>
        /// 按实体属性更新上下文中指定实体的状态
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">实体主键类型</typeparam>
        /// <param name="dbContext">上下文对象</param>
        /// <param name="propertyExpression">实体属性表达式，提供要更新的实体属性</param>
        /// <param name="entities">附带新值的实体对象，必须包含主键</param>
        public static void Update<TEntity, TKey>(this DbContext dbContext,
            Expression<Func<TEntity, object>> propertyExpression,
            params TEntity[] entities)
            where TEntity : Entity<TKey>
        {
            ReadOnlyCollection<MemberInfo> memberInfos = ((dynamic)propertyExpression.Body).Members;
            foreach (TEntity entity in entities)
            {
                DbSet<TEntity> dbSet = dbContext.Set<TEntity>();
                try
                {
                    DbEntityEntry<TEntity> entry = dbContext.Entry(entity);
                    entry.State = EntityState.Unchanged;
                    foreach (MemberInfo memberInfo in memberInfos)
                    {
                        entry.Property(memberInfo.Name).IsModified = true;
                    }
                }
                catch (InvalidOperationException)
                {
                    TEntity originalEntity = dbSet.Local.Single(m => Equals(m.Id, entity.Id));
                    ObjectContext objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
                    ObjectStateEntry objectEntry = objectContext.ObjectStateManager.GetObjectStateEntry(originalEntity);
                    objectEntry.ApplyCurrentValues(entity);
                    objectEntry.ChangeState(EntityState.Unchanged);
                    foreach (MemberInfo memberInfo in memberInfos)
                    {
                        objectEntry.SetModifiedProperty(memberInfo.Name);
                    }
                }
            }
        }

        /// <summary>
        /// 获取数据上下文的变更日志信息
        /// </summary>
        public static IEnumerable<OperatingLog> GetEntityOperateLogs(this DbContext dbContext)
        {
            string[] nonLoggingTypeNames = { };

            ObjectContext objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
            ObjectStateManager manager = objectContext.ObjectStateManager;

            IEnumerable<ObjectStateEntry> entries = manager.GetObjectStateEntries(EntityState.Added)
                .Where(entry => entry.Entity != null && !nonLoggingTypeNames.Contains(entry.Entity.GetType().FullName));
            IEnumerable<OperatingLog> logs = entries.Select(GetAddedLog);

            entries = manager.GetObjectStateEntries(EntityState.Modified)
                .Where(entry => entry.Entity != null && !nonLoggingTypeNames.Contains(entry.Entity.GetType().FullName));
            logs = logs.Union(entries.Select(GetModifiedLog));

            entries = manager.GetObjectStateEntries(EntityState.Deleted)
                .Where(entry => entry.Entity != null && !nonLoggingTypeNames.Contains(entry.Entity.GetType().FullName));
            logs = logs.Union(entries.Select(GetDeletedLog));
            return logs;
        }

        /// <summary>
        /// 异步获取数据上下文的变更日志信息
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<OperatingLog>> GetEntityOperateLogsAsync(this DbContext dbContext)
        {
            return await Task.Run(() => dbContext.GetEntityOperateLogs());
        }

        /// <summary>
        /// 获取添加数据的日志信息
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private static OperatingLog GetAddedLog(ObjectStateEntry entry)
        {
            var log = new OperatingLog
            {
                EntityName = entry.EntitySet.ElementType.Name,
                OperateType = OperatingType.Insert
            };
            for (int i = 0; i < entry.CurrentValues.FieldCount; i++)
            {
                string name = entry.CurrentValues.GetName(i);
                if (name == "Timestamp")
                {
                    continue;
                }
                object value = entry.CurrentValues.GetValue(i);
                var logItem = new OperatingLogItem
                {
                    Field = name,
                    NewValue = value == null ? null : value.ToString()
                };
                log.LogItems.Add(logItem);
            }
            return log;
        }

        /// <summary>
        /// 获取修改数据的日志信息
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private static OperatingLog GetModifiedLog(ObjectStateEntry entry)
        {
            var log = new OperatingLog
            {
                EntityName = entry.EntitySet.ElementType.Name,
                OperateType = OperatingType.Update
            };
            for (int i = 0; i < entry.CurrentValues.FieldCount; i++)
            {
                string name = entry.CurrentValues.GetName(i);
                if (name == "Timestamp")
                {
                    continue;
                }
                object currentValue = entry.CurrentValues.GetValue(i);
                object originalValue = entry.OriginalValues[name];
                if (currentValue.Equals(originalValue))
                {
                    continue;
                }
                var logItem = new OperatingLogItem
                {
                    Field = name,
                    NewValue = currentValue.ToString(),
                    OriginalValue = originalValue == null ? null : originalValue.ToString()
                };
                log.LogItems.Add(logItem);
            }
            return log;
        }

        /// <summary>
        /// 获取删除数据的日志信息
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private static OperatingLog GetDeletedLog(ObjectStateEntry entry)
        {
            var log = new OperatingLog
            {
                EntityName = entry.EntitySet.ElementType.Name,
                OperateType = OperatingType.Delete
            };
            for (int i = 0; i < entry.OriginalValues.FieldCount; i++)
            {
                string name = entry.OriginalValues.GetName(i);
                if (name == "Timestamp")
                {
                    continue;
                }
                object originalValue = entry.OriginalValues[i];
                var logItem = new OperatingLogItem
                {
                    Field = name,
                    OriginalValue = originalValue == null ? null : originalValue.ToString()
                };
                log.LogItems.Add(logItem);
            }
            return log;
        }
    }
}
