using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Shoy.Utility.License
{
    /// <summary> 编码/激活码 实例管理类 </summary>
    public static class LicenseManager
    {
        private static readonly object LockObj = new object();
        private static ConcurrentDictionary<LicenseType, LicenseHelper> _helperCache;
        /// <summary> 初始化缓存委托 </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public delegate List<string> InitCache(LicenseType type);

        /// <summary> 初始化缓存事件 </summary>
        public static event InitCache InitCacheHandler;

        public static LicenseHelper Instance(LicenseType type)
        {
            lock (LockObj)
            {
                if (_helperCache == null)
                    _helperCache = new ConcurrentDictionary<LicenseType, LicenseHelper>();
                LicenseHelper helper;
                if (_helperCache.ContainsKey(type) && _helperCache.TryGetValue(type, out helper))
                    return helper;
                helper = new LicenseHelper(type);
                if (InitCacheHandler != null)
                {
                    var list = InitCacheHandler(type);
                    if (list != null && list.Any())
                    {
                        helper.SetCache(list);
                    }
                }
                _helperCache.TryAdd(type, helper);
                return helper;
            }
        }

        /// <summary> 设置编码/激活码生成规则 </summary>
        /// <param name="type"></param>
        /// <param name="role">总长度，缓存列表，尝试次数</param>
        public static void SetGenerateRole(LicenseType type, Func<int, List<string>, int, string> role)
        {
            var helper = Instance(type);
            helper.SetGenerateRole(role);
        }
    }
}
