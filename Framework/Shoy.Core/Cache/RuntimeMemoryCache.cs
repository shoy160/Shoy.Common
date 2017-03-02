using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Shoy.Core.Cache
{
    /// <summary> 运行时缓存 </summary>
    public class RuntimeMemoryCache : BaseCache
    {
        private readonly string _region;
        private readonly ObjectCache _cache;

        public RuntimeMemoryCache(string region)
        {
            _region = region;
            _cache = MemoryCache.Default;
        }

        public override string Region
        {
            get { return _region; }
        }

        public override void Set(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            var cacheKey = GetKey(key);
            _cache.Add(cacheKey, new DictionaryEntry(key, value), new CacheItemPolicy());
        }

        public override void Set(string key, object value, TimeSpan expire)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            var cacheKey = GetKey(key);
            _cache.Add(cacheKey, new DictionaryEntry(key, value), new CacheItemPolicy
            {
                SlidingExpiration = expire
            });
        }

        public override void Set(string key, object value, DateTime expire)
        {
            if (string.IsNullOrWhiteSpace(key) || value == null)
                return;
            var cacheKey = GetKey(key);
            _cache.Add(cacheKey, new DictionaryEntry(key, value), new CacheItemPolicy
            {
                AbsoluteExpiration = expire
            });
        }

        public override object Get(string key)
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

        public override IEnumerable<object> GetAll()
        {
            var token = string.Concat(_region, ":");
            return
                _cache.Where(m => m.Key.StartsWith(token))
                    .Select(m => m.Value)
                    .Cast<DictionaryEntry>()
                    .Select(m => m.Value);
        }

        public override T Get<T>(string key)
        {
            var value = Get(key);
            return value == null ? default(T) : (T)value;
        }

        public override void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            string cacheKey = GetKey(key);
            _cache.Remove(cacheKey);
        }

        public override void Remove(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public override void Clear()
        {
            var token = _region + ":";
            var cacheKeys = _cache.Where(m => m.Key.StartsWith(token)).Select(m => m.Key).ToList();
            foreach (var cacheKey in cacheKeys)
            {
                _cache.Remove(cacheKey);
            }
        }
    }
}
