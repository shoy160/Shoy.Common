using Shoy.Utility.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shoy.Core.Cache
{
    /// <summary> 缓存执行者 </summary>
    internal sealed class InternalCacher : ICache
    {
        private static readonly ILogger Logger = LogManager.Logger<InternalCacher>();

        private readonly Dictionary<CacheLevel, ICache> _caches;

        /// <summary> 本地缓存时间 </summary>
        private TimeSpan _memoryExpire = TimeSpan.FromMinutes(5);

        /// <summary> 初始化一个<see cref="InternalCacher"/>类型的新实例 </summary>
        public InternalCacher(string region)
        {
            _caches = CacheManager.Providers.Where(m => m.Value != null)
                .ToDictionary(k => k.Key, v => v.Value.GetCache(region));
            if (_caches.Count == 0)
            {
                Logger.Warn("没有设置缓存提供者！");
            }
        }

        public void Set(string key, object value)
        {
            foreach (var cache in _caches)
            {
                if (cache.Key == CacheLevel.First)
                    cache.Value.Set(key, value, _memoryExpire);
                else
                    cache.Value.Set(key, value);
            }
        }

        public void Set(string key, object value, TimeSpan expire)
        {
            foreach (var cache in _caches)
            {
                cache.Value.Set(key, value, cache.Key == CacheLevel.First ? _memoryExpire : expire);
            }
        }

        public void Set(string key, object value, DateTime expire)
        {
            foreach (var cache in _caches)
            {
                if (cache.Key == CacheLevel.First)
                    cache.Value.Set(key, value, _memoryExpire);
                else
                    cache.Value.Set(key, value, expire);
            }
        }

        public object Get(string key)
        {
            object value = null;
            if (_caches.ContainsKey(CacheLevel.First))
            {
                //先从一级缓存读取
                value = _caches[CacheLevel.First].Get(key);
                if (value != null)
                    return value;
            }
            if (_caches.ContainsKey(CacheLevel.Second))
            {
                value = _caches[CacheLevel.Second].Get(key);
                if (value != null && _caches.ContainsKey(CacheLevel.First))
                {
                    //设置一级缓存
                    _caches[CacheLevel.First].Set(key, value, _memoryExpire);
                }
            }
            return value;
        }

        public IEnumerable<object> GetAll()
        {
            var values = new List<object>();
            foreach (var cache in _caches.Values)
            {
                values = cache.GetAll().ToList();
                if (values.Count != 0)
                {
                    break;
                }
            }
            return values;
        }

        public T Get<T>(string key)
        {
            var value = default(T);
            if (_caches.ContainsKey(CacheLevel.First))
            {
                //先从一级缓存读取
                value = _caches[CacheLevel.First].Get<T>(key);
                if (value != null)
                    return value;
            }
            if (_caches.ContainsKey(CacheLevel.Second))
            {
                value = _caches[CacheLevel.Second].Get<T>(key);
                if (value != null && _caches.ContainsKey(CacheLevel.First))
                {
                    //设置一级缓存
                    _caches[CacheLevel.First].Set(key, value, _memoryExpire);
                }
            }
            return value;
        }

        public void Remove(string key)
        {
            foreach (var cache in _caches.Values)
            {
                cache.Remove(key);
            }
        }

        public void Remove(IEnumerable<string> keys)
        {
            var enumerable = keys as string[] ?? keys.ToArray();
            foreach (var cache in _caches.Values)
            {
                cache.Remove(enumerable);
            }
        }

        public void Clear()
        {
            foreach (var cache in _caches.Values)
            {
                cache.Clear();
            }
        }

        public void MemoryExpire(TimeSpan expire)
        {
            _memoryExpire = expire;
        }
    }
}
