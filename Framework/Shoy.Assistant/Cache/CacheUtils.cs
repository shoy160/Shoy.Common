using CacheManager.Core;

namespace Shoy.Assistant.Cache
{
    public class CacheUtils
    {
        public CacheUtils()
        {
            var cache = CacheFactory.Build("", settings =>
            {
                settings.WithSystemRuntimeCacheHandle("");
            });
        }
    }
}
