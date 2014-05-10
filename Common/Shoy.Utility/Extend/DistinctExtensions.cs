using System;
using System.Collections.Generic;
using System.Linq;

namespace Shoy.Utility.Extend
{
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

        public static IEnumerable<T> RandomSort<T>(this IEnumerable<T> array)
        {
            int len = array.Count();
            var list = new List<int>();
            var ret = new T[len];
            var rand = Utils.GetRandom();
            int i = 0;
            while (list.Count < len)
            {
                int iter = rand.Next(0, len);
                if (!list.Contains(iter))
                {
                    list.Add(iter);
                    ret[i] = array.ToArray()[iter];
                    i++;
                }
            }
            return ret;
        }
    }
}
