using System;
using System.Linq;
using ServiceStack.CacheAccess;
using ServiceStack.Redis;
using Shoy.MemoryDb.Configs;
using Shoy.Utility;
using Shoy.Utility.Config;
using Shoy.Utility.Logging;

namespace Shoy.MemoryDb.Redis
{
    internal class RedisManager
    {
        private PooledRedisClientManager _manager;
        private readonly ILogger _logger = LogManager.Logger<RedisManager>();

        private RedisManager()
        {
            InitConfig();
            ConfigManager.Change += file =>
            {
                if (file == ConfigUtils<RedisConfig>.Instance.FileName)
                    InitConfig();
            };
        }

        private void InitConfig()
        {
            var config = ConfigUtils<RedisConfig>.Instance.Get();
            if (config == null)
            {
                const string msg = "Redis配置文件加载失败！";
                _logger.Error(msg);
                throw new Exception(msg);
            }
            var redisConfig = new RedisClientManagerConfig
            {
                MaxWritePoolSize = config.WritePoolSize,
                MaxReadPoolSize = config.ReadPoolSize,
                AutoStart = config.AutoStart,
                DefaultDb = config.DefaultDb
            };
            _manager = new PooledRedisClientManager(config.ReadAndWriteServers.Select(t => t.ToString()),
                config.ReadOnlyServers.Select(t => t.ToString()), redisConfig, 0, 50, 5);
            _manager.Start();
        }

        internal static RedisManager Instance
        {
            get
            {
                return Singleton<RedisManager>.Instance
                    ?? (Singleton<RedisManager>.Instance = new RedisManager());
            }
        }

        internal IRedisClient GetClient()
        {
            var client = _manager.GetClient();
            client.RetryCount = 5;
            return client;
        }

        internal IRedisClient ReadOnlyClient()
        {
            var client = _manager.GetReadOnlyClient();
            client.RetryCount = 5;
            return client;
        }

        internal ICacheClient CacheClient
        {
            get { return _manager.GetCacheClient(); }
        }
    }
}
