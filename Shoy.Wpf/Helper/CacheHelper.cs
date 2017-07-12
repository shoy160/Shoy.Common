using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Shoy.Wpf.Helper
{
    /// <summary> 缓存辅助类 </summary>
    public class CacheHelper
    {
        private readonly ObjectCache _cache;

        public CacheHelper(string region)
        {
            Region = region;
            _cache = MemoryCache.Default;
        }

        /// <summary> 缓存区域 </summary>
        public string Region { get; }

        public void Set(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            var cacheKey = GetKey(key);
            _cache.Add(cacheKey, new DictionaryEntry(key, value), new CacheItemPolicy());
        }

        public void Set(string key, object value, TimeSpan expire)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            var cacheKey = GetKey(key);
            _cache.Add(cacheKey, new DictionaryEntry(key, value), new CacheItemPolicy
            {
                SlidingExpiration = expire
            });
        }

        public void Set(string key, object value, DateTime expire)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            var cacheKey = GetKey(key);
            _cache.Add(cacheKey, new DictionaryEntry(key, value), new CacheItemPolicy
            {
                AbsoluteExpiration = expire
            });
        }

        public object Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            var cacheKey = GetKey(key);
            var value = _cache.Get(cacheKey);
            if (value == null)
                return null;
            var entry = (DictionaryEntry)value;
            return key.Equals(entry.Key) ? entry.Value : null;
        }

        public IEnumerable<object> GetAll()
        {
            var token = string.Concat(Region, ":");
            return
                _cache.Where(m => m.Key.StartsWith(token))
                    .Select(m => m.Value)
                    .Cast<DictionaryEntry>()
                    .Select(m => m.Value);
        }

        public T Get<T>(string key)
        {
            var value = Get(key);
            return value == null ? default(T) : (T)value;
        }

        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            string cacheKey = GetKey(key);
            _cache.Remove(cacheKey);
        }

        public void Remove(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public void Clear()
        {
            var token = Region + ":";
            var cacheKeys = _cache.Where(m => m.Key.StartsWith(token)).Select(m => m.Key).ToList();
            foreach (var cacheKey in cacheKeys)
            {
                _cache.Remove(cacheKey);
            }
        }

        public void ExpireEntryIn(string key, TimeSpan timeSpan)
        {
            var value = Get(key);
            if (value != null)
                Set(key, value, timeSpan);
        }

        public void ExpireEntryAt(string key, DateTime dateTime)
        {
            var value = Get(key);
            if (value != null)
                Set(key, value, dateTime);
        }

        /// <summary> 获取缓存键 </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetKey(string key)
        {
            return string.Concat(Region, ":", key);//, "@", key.GetHashCode()
        }
    }
}
