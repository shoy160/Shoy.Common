using System.Collections.Concurrent;
using Shoy.ThirdPlatform.Entity.Config;
using Shoy.ThirdPlatform.Helper;
using Shoy.Utility.Config;

namespace Shoy.ThirdPlatform
{
    public class PlatformFactory
    {
        private static readonly ConcurrentDictionary<string, HelperBase> HelperCache;
        private static readonly object LockObj = new object();

        static PlatformFactory()
        {
            HelperCache = new ConcurrentDictionary<string, HelperBase>();
            //添加监听
            ConfigManager.Change += file =>
            {
                if (file == ConfigUtils<PlatformConfig>.Instance().FileName)
                {
                    HelperCache.Clear();
                }
            };
        }

        public static HelperBase GetInstance(PlatformType type)
        {
            return GetInstance(type.ToString());
        }

        public static HelperBase GetInstance(string type)
        {
            lock (LockObj)
            {
                if (HelperCache.ContainsKey(type))
                    return HelperCache[type];
                var instance = HelperBase.GetInstance(type);
                if (instance != null)
                {
                    HelperCache.TryAdd(type, instance);
                }
                return instance;
            }
        }
    }
}
