using System;
using System.Collections.Generic;

namespace Shoy.Core.Cache
{
    /// <summary> 缓存接口 </summary>
    public interface ICache
    {
        void Set(string key, object value);
        void Set(string key, object value, TimeSpan expire);
        void Set(string key, object value, DateTime expire);

        object Get(string key);

        IEnumerable<object> GetAll();

        T Get<T>(string key);

        void Remove(string key);

        void Remove(IEnumerable<string> keys);
        void Clear();
        void MemoryExpire(TimeSpan expire);
    }
}
