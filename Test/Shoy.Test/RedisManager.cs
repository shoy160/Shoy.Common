using ServiceStack.Redis;
using Shoy.Utility;

namespace Shoy.Test
{
    public class RedisManager
    {
        private readonly PooledRedisClientManager _manager;

        private RedisManager()
        {
            var config = new RedisClientManagerConfig
            {
                MaxWritePoolSize = 5,
                MaxReadPoolSize = 5,
                AutoStart = true
            };
            _manager = new PooledRedisClientManager(
                new[] { "deyi@168@192.168.10.20:6379" },
                new[] { "deyi@123@192.168.10.20:6380", "deyi@123@192.168.10.20:6381" }, config);
//            _manager = new PooledRedisClientManager(
//                new[] { "954f12aab80205ec@211.149.229.128:6379" },
//                new[] { "862c53a755e16cc@211.149.229.128:6380", "40ca4419f50407cc@211.149.229.128:6381" }, config);
        }

        public static RedisManager Instance
        {
            get { return Singleton<RedisManager>.Instance ?? (Singleton<RedisManager>.Instance = new RedisManager()); }
        }

        public IRedisClient GetClient()
        {
            return _manager.GetClient();
        }

        public IRedisClient ReadOnlyClient()
        {
            return _manager.GetReadOnlyClient();
        }
    }
}
