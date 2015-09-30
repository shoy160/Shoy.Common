using System;
using System.Collections.Generic;
using System.Linq;

namespace Shoy.MemoryDb.Redis
{
    /// <summary> Redis常用辅助 </summary>
    public static class RedisUtils
    {
        private const string Prefix = "shoy_";

        private static RedisManager Redis
        {
            get { return RedisManager.Instance; }
        }

        public static bool Contains(string key)
        {
            using (var client = Redis.ReadOnlyClient())
            {
                return client.ContainsKey(key.Key());
            }
        }

        public static IEnumerable<string> SearchKeys(string pattern)
        {
            using (var client = Redis.ReadOnlyClient())
            {
                return client.SearchKeys(pattern);
            }
        }

        public static void Set<T>(string key, T obj)
        {
            using (var client = Redis.CacheClient)
            {
                client.Set(key.Key(), obj);
            }
        }

        public static void Set<T>(string key, T obj, TimeSpan expire)
        {
            using (var client = Redis.CacheClient)
            {
                client.Set(key.Key(), obj, expire);
            }
        }

        public static void Set<T>(string key, T obj, DateTime expire)
        {
            using (var client = Redis.CacheClient)
            {
                client.Set(key.Key(), obj, expire);
            }
        }

        public static void Replace<T>(string key, T obj)
        {
            using (var client = Redis.CacheClient)
            {
                client.Replace(key.Key(), obj);
            }
        }

        public static void Replace<T>(string key, T obj, TimeSpan expire)
        {
            using (var client = Redis.CacheClient)
            {
                client.Replace(key.Key(), obj, expire);
            }
        }

        public static void Replace<T>(string key, T obj, DateTime expire)
        {
            using (var client = Redis.CacheClient)
            {
                client.Replace(key.Key(), obj, expire);
            }
        }

        public static void SetAll<T>(IDictionary<string, T> values)
        {
            var dict = new Dictionary<string, T>();
            foreach (var key in values.Keys)
            {
                dict.Add(key.Key(), values[key]);
            }
            using (var client = Redis.CacheClient)
            {
                client.SetAll(dict);
            }
        }

        public static T Get<T>(string key)
        {
            using (var client = Redis.CacheClient)
            {
                return client.Get<T>(key.Key());
            }
        }

        public static bool Delete(string key)
        {
            using (var client = Redis.CacheClient)
            {
                return client.Remove(key.Key());
            }
        }

        public static void DeleteAll(IEnumerable<string> keys)
        {
            using (var client = Redis.CacheClient)
            {
                client.RemoveAll(keys);
            }
        }

        public static void DeleteAll(string pattern)
        {
            var keys = SearchKeys(pattern).ToList();
            if (!keys.Any())
                return;
            using (var client = Redis.CacheClient)
            {
                client.RemoveAll(keys);
            }
        }

        public static string Key(this string key, string prefix = Prefix)
        {
            return string.Concat(prefix, key);
        }
    }
}
