using System;
using System.Collections.Generic;
using System.Linq;

namespace Shoy.Utility.Extend
{
    /// <summary>
    /// Linq Distinct扩展
    /// </summary>
    public static class DistinctExtensions
    {
        public static IEnumerable<T> Distinct<T,V>(this IEnumerable<T> source,Func<T,V> keySelector)
        {
            return source.Distinct(new CommonEqualityComparer<T, V>(keySelector));
        }

        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector,IEqualityComparer<V> comparer)
        {
            return source.Distinct(new CommonEqualityComparer<T, V>(keySelector, comparer));
        }

        /// <summary>
        /// 随机排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static IEnumerable<T> RandomSort<T>(this IEnumerable<T> array)
        {
            return array.OrderBy(t => Utils.GetRandom().Next());
        }
    }
}
