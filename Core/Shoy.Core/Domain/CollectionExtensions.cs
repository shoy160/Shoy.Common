using System;
using System.Linq;
using System.Linq.Expressions;
using Shoy.Core.Data;
using Shoy.Core.Domain.Entities;

namespace Shoy.Core.Domain
{
    /// <summary>
    /// IQueryable 扩展
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// 从指定<see cref="IQueryable{T}"/>集合 中查询指定分页条件的子数据集
        /// </summary>
        /// <typeparam name="TEntity">动态实体类型</typeparam>
        /// <typeparam name="TKey">实体主键类型</typeparam>
        /// <param name="source">要查询的数据集</param>
        /// <param name="predicate">查询条件谓语表达式</param>
        /// <param name="pageIndex">分页索引（从1开始）</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="total">输出符合条件的总记录数</param>
        /// <param name="sortConditions">排序条件集合</param>
        /// <returns></returns>
        public static IQueryable<TEntity> Where<TEntity, TKey>(this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate,
            int pageIndex,
            int pageSize,
            out int total,
            SortCondition[] sortConditions = null) where TEntity : Entity<TKey>
        {
            total = source.Count(predicate);
            source = source.Where(predicate);
            if (sortConditions == null || sortConditions.Length == 0)
            {
                source = source.OrderBy(m => m.Id);
            }
            else
            {
                int count = 0;
                IOrderedQueryable<TEntity> orderSource = null;
                foreach (SortCondition sortCondition in sortConditions)
                {
                    orderSource = count == 0
                        ? QueryablePropertySorter<TEntity>.OrderBy(source, sortCondition.SortField,
                            sortCondition.Sort)
                        : QueryablePropertySorter<TEntity>.ThenBy(orderSource, sortCondition.SortField,
                            sortCondition.Sort);
                    count++;
                }
                source = orderSource;
            }
            return source != null
                ? source.Skip((pageIndex - 1)*pageSize).Take(pageSize)
                : Enumerable.Empty<TEntity>().AsQueryable();
        }
    }
}
