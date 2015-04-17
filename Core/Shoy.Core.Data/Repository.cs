﻿using AutoMapper;
using Shoy.Core.Data.Extensions;
using Shoy.Utility.Extend;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shoy.Core.Data
{
    /// <summary>
    /// EntityFramework的仓储实现
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : EntityBase<TKey>
    {
        private readonly DbSet<TEntity> _dbSet;
        private readonly IUnitOfWork _unitOfWork;

        public Repository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _dbSet = ((DbContext)unitOfWork).Set<TEntity>();
        }

        /// <summary>
        /// 获取 当前单元操作对象
        /// </summary>
        public IUnitOfWork UnitOfWork { get { return _unitOfWork; } }

        /// <summary>
        /// 获取 当前实体类型的查询数据集
        /// </summary>
        public IQueryable<TEntity> Entities { get { return _dbSet; } }

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>操作影响的行数</returns>
        public int Insert(TEntity entity)
        {
            if (entity == null)
                return 0;
            _dbSet.Add(entity);
            return SaveChanges();
        }

        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <returns>操作影响的行数</returns>
        public int Insert(IEnumerable<TEntity> entities)
        {
            entities = entities as TEntity[] ?? entities.ToArray();
            _dbSet.AddRange(entities);
            return SaveChanges();
        }

        /// <summary>
        /// 以DTO为载体批量插入实体
        /// </summary>
        /// <typeparam name="TAddDto">添加DTO类型</typeparam>
        /// <param name="dtos">添加DTO信息集合</param>
        /// <param name="checkAction">添加信息合法性检查委托</param>
        /// <param name="updateFunc">由DTO到实体的转换委托</param>
        /// <returns>业务操作结果</returns>
        public OperateResult Insert<TAddDto>(ICollection<TAddDto> dtos, Action<TAddDto> checkAction = null, Func<TAddDto, TEntity, TEntity> updateFunc = null) where TAddDto : IAddDto
        {
            if (dtos == null)
                return new OperateResult(OperateResultType.NoChanged);
            var names = new List<string>();
            foreach (var dto in dtos)
            {
                var entity = Mapper.Map<TEntity>(dto);
                try
                {
                    if (checkAction != null)
                    {
                        checkAction(dto);
                    }
                    if (updateFunc != null)
                    {
                        entity = updateFunc(dto, entity);
                    }
                }
                catch (Exception e)
                {
                    return new OperateResult(OperateResultType.Error, e.Message);
                }
                _dbSet.Add(entity);
                string name = GetNameValue(dto);
                if (name != null)
                {
                    names.Add(name);
                }
            }
            int count = SaveChanges();
            return count > 0
                ? new OperateResult(OperateResultType.Success,
                    names.Count > 0
                        ? "信息“{0}”添加成功".FormatWith(names.Join())
                        : "{0}个信息添加成功".FormatWith(dtos.Count))
                : new OperateResult(OperateResultType.NoChanged);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>操作影响的行数</returns>
        public int Delete(TEntity entity)
        {
            if (entity == null)
                return 0;
            _dbSet.Remove(entity);
            return SaveChanges();
        }

        public virtual int Delete(TKey key)
        {
            TEntity entity = _dbSet.Find(key);
            return entity == null ? 0 : Delete(entity);
        }

        /// <summary>
        /// 删除所有符合特定条件的实体
        /// </summary>
        /// <param name="condition">查询条件谓语表达式</param>
        /// <returns>操作影响的行数</returns>
        public int Delete(Expression<Func<TEntity, bool>> condition)
        {
            TEntity[] entities = _dbSet.Where(condition).ToArray();
            return entities.Length == 0 ? 0 : Delete(entities);
        }

        /// <summary>
        /// 批量删除删除实体
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <returns>操作影响的行数</returns>
        public int Delete(IEnumerable<TEntity> entities)
        {
            entities = entities as TEntity[] ?? entities.ToArray();
            _dbSet.RemoveRange(entities);
            return SaveChanges();
        }

        /// <summary>
        /// 以标识集合批量删除实体
        /// </summary>
        /// <param name="ids">标识集合</param>
        /// <param name="checkAction">删除前置检查委托</param>
        /// <param name="deleteFunc">删除委托，用于删除关联信息</param>
        /// <returns>业务操作结果</returns>
        public OperateResult Delete(ICollection<TKey> ids, Action<TEntity> checkAction = null, Func<TEntity, TEntity> deleteFunc = null)
        {
            if (ids == null)
                return new OperateResult(OperateResultType.NoChanged);
            var names = new List<string>();
            foreach (var id in ids)
            {
                TEntity entity = _dbSet.Find(id);
                try
                {
                    if (checkAction != null)
                    {
                        checkAction(entity);
                    }
                    if (deleteFunc != null)
                    {
                        entity = deleteFunc(entity);
                    }
                }
                catch (Exception e)
                {
                    return new OperateResult(OperateResultType.Error, e.Message);
                }
                _dbSet.Remove(entity);
                string name = GetNameValue(entity);
                if (name != null)
                {
                    names.Add(name);
                }
            }
            int count = SaveChanges();
            return count > 0
                ? new OperateResult(OperateResultType.Success,
                    names.Count > 0
                        ? "信息“{0}”删除成功".FormatWith(names.Join())
                        : "{0}个信息删除成功".FormatWith(ids.Count))
                : new OperateResult(OperateResultType.NoChanged);
        }

        /// <summary>
        /// 更新实体对象
        /// </summary>
        /// <param name="entity">更新后的实体对象</param>
        /// <returns>操作影响的行数</returns>
        public int Update(TEntity entity)
        {
            ((DbContext)_unitOfWork).Update<TEntity, TKey>(entity);
            return SaveChanges();
        }

        /// <summary>
        /// 以DTO为载体批量更新实体
        /// </summary>
        /// <typeparam name="TEditDto">更新DTO类型</typeparam>
        /// <param name="dtos">更新DTO信息集合</param>
        /// <param name="checkAction">更新信息合法性检查委托</param>
        /// <param name="updateFunc">由DTO到实体的转换委托</param>
        /// <returns>业务操作结果</returns>
        public OperateResult Update<TEditDto>(ICollection<TEditDto> dtos, Action<TEditDto> checkAction = null, Func<TEditDto, TEntity, TEntity> updateFunc = null) where TEditDto : IEditDto<TKey>
        {
            var names = new List<string>();
            foreach (var dto in dtos)
            {
                TEntity entity = _dbSet.Find(dto.Id);
                if (entity == null)
                {
                    return new OperateResult(OperateResultType.QueryNull);
                }
                entity = Mapper.Map(dto, entity);
                try
                {
                    if (checkAction != null)
                    {
                        checkAction(dto);
                    }
                    if (updateFunc != null)
                    {
                        entity = updateFunc(dto, entity);
                    }
                }
                catch (Exception e)
                {
                    return new OperateResult(OperateResultType.Error, e.Message);
                }
                ((DbContext)_unitOfWork).Update<TEntity, TKey>(entity);
                string name = GetNameValue(dto);
                if (name != null)
                {
                    names.Add(name);
                }
            }
            int count = SaveChanges();
            return count > 0
                ? new OperateResult(OperateResultType.Success,
                    names.Count > 0
                        ? "信息“{0}”更新成功".FormatWith(names.Join())
                        : "{0}个信息更新成功".FormatWith(dtos.Count))
                : new OperateResult(OperateResultType.NoChanged);
        }

        /// <summary>
        /// 实体存在性检查
        /// </summary>
        /// <param name="condition">查询条件谓语表达式</param>
        /// <param name="id">编辑的实体标识</param>
        /// <returns>是否存在</returns>
        public bool ExistsCheck(Expression<Func<TEntity, bool>> condition, TKey id = default(TKey))
        {
            TKey defaultId = default(TKey);
            var entity = _dbSet.Where(condition).Select(m => new { m.Id }).SingleOrDefault();
            bool exists = id.Equals(defaultId) ? entity != null : entity != null && entity.Id.Equals(defaultId);
            return exists;
        }

        /// <summary>
        /// 查找指定主键的实体
        /// </summary>
        /// <param name="key">实体主键</param>
        /// <returns>符合主键的实体，不存在时返回null</returns>
        public TEntity GetByKey(TKey key)
        {
            return _dbSet.Find(key);
        }

        /// <summary>
        /// 获取贪婪加载导航属性的查询数据集
        /// </summary>
        /// <param name="path">属性表达式，表示要贪婪加载的导航属性</param>
        /// <returns>查询数据集</returns>
        public IQueryable<TEntity> GetInclude<TProperty>(Expression<Func<TEntity, TProperty>> path)
        {
            return _dbSet.Include(path);
        }

        /// <summary>
        /// 获取贪婪加载多个导航属性的查询数据集
        /// </summary>
        /// <param name="paths">要贪婪加载的导航属性名称数组</param>
        /// <returns>查询数据集</returns>
        public IQueryable<TEntity> GetIncludes(params string[] paths)
        {
            IQueryable<TEntity> source = _dbSet;
            foreach (var path in paths)
            {
                source = source.Include(path);
            }
            return source;
        }

        /// <summary>
        /// 异步插入实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>操作影响的行数</returns>
        public async Task<int> InsertAsync(TEntity entity)
        {
            _dbSet.Add(entity);
            return await SaveChangesAsync();
        }

        /// <summary>
        /// 异步批量插入实体
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <returns>操作影响的行数</returns>
        public async Task<int> InsertAsync(IEnumerable<TEntity> entities)
        {
            entities = entities as TEntity[] ?? entities.ToArray();
            _dbSet.AddRange(entities);
            return await SaveChangesAsync();
        }

        /// <summary>
        /// 异步删除实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>操作影响的行数</returns>
        public async Task<int> DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            return await SaveChangesAsync();
        }

        /// <summary>
        /// 异步删除指定编号的实体
        /// </summary>
        /// <param name="key">实体编号</param>
        /// <returns>操作影响的行数</returns>
        public async Task<int> DeleteAsync(TKey key)
        {
            TEntity entity = await _dbSet.FindAsync(key);
            return entity == null ? 0 : await DeleteAsync(entity);
        }

        /// <summary>
        /// 异步删除所有符合特定条件的实体
        /// </summary>
        /// <param name="condition">查询条件谓语表达式</param>
        /// <returns>操作影响的行数</returns>
        public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> condition)
        {
            TEntity[] entities = await _dbSet.Where(condition).ToArrayAsync();
            return entities.Length == 0 ? 0 : await DeleteAsync(entities);
        }

        /// <summary>
        /// 异步批量删除删除实体
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <returns>操作影响的行数</returns>
        public async Task<int> DeleteAsync(IEnumerable<TEntity> entities)
        {
            entities = entities as TEntity[] ?? entities.ToArray();
            _dbSet.RemoveRange(entities);
            return await SaveChangesAsync();
        }

        /// <summary>
        /// 异步更新实体对象
        /// </summary>
        /// <param name="entity">更新后的实体对象</param>
        /// <returns>操作影响的行数</returns>
        public async Task<int> UpdateAsync(TEntity entity)
        {
            ((DbContext)_unitOfWork).Update<TEntity, TKey>(entity);
            return await SaveChangesAsync();
        }

        /// <summary>
        /// 异步使用附带新值的实体更新指定实体属性的值，此方法不支持事务
        /// </summary>
        /// <param name="propertyExpresion">属性表达式，提供要更新的实体属性</param>
        /// <param name="entities">附带新值的实体属性，必须包含主键</param>
        /// <returns>操作影响的行数</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public async Task<int> UpdateAsync(Expression<Func<TEntity, object>> propertyExpresion, params TEntity[] entities)
        {
            var context = new CodeFirstDbContext();
            context.Update<TEntity, TKey>(propertyExpresion, entities);
            bool fail;
            try
            {
                return await context.SaveChangesAsync(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                fail = true;
            }
            TKey[] ids = entities.Select(m => m.Id).ToArray();
            context.Set<TEntity>().Where(m => ids.Contains(m.Id)).Load();
            context.Update<TEntity, TKey>(propertyExpresion, entities);
            return await context.SaveChangesAsync(false);
        }

        /// <summary>
        /// 异步查找指定主键的实体
        /// </summary>
        /// <param name="key">实体主键</param>
        /// <returns>符合主键的实体，不存在时返回null</returns>
        public async Task<TEntity> GetByKeyAsync(TKey key)
        {
            return await _dbSet.FindAsync(key);
        }

        #region 私有方法

        private int SaveChanges()
        {
            return _unitOfWork.TransactionEnabled ? 0 : _unitOfWork.SaveChanges();
        }

        private async Task<int> SaveChangesAsync()
        {
            return _unitOfWork.TransactionEnabled ? 0 : await _unitOfWork.SaveChangesAsync();
        }

        private static string GetNameValue(object value)
        {
            dynamic obj = value;
            try
            {
                return obj.Name;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
