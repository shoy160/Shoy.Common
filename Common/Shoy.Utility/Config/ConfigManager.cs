using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Shoy.Utility.Extend;
using Shoy.Utility.Helper;

namespace Shoy.Utility.Config
{
    /// <summary>
    /// 配置文件管理
    /// </summary>
    public class ConfigManager
    {
        private static readonly IDictionary<string, object> ConfigCache = new ConcurrentDictionary<string, object>();
        private static readonly string ConfigPath;
        private static readonly object LockObj = new object();

        static ConfigManager()
        {
            ConfigPath = ConfigurationManager.AppSettings.Get("configPath");
            if (!Directory.Exists(ConfigPath)) return;
            //文件监控
            var watcher = new FileSystemWatcher(ConfigPath)
                {
                    IncludeSubdirectories = true,
                    Filter = "*.config", //"*.config|*.xml"多个扩展名不受支持！
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size
                };
            watcher.Changed += Reset;
            watcher.Deleted += Reset;
            watcher.Renamed += Reset;
            watcher.Created += Reset;
            watcher.EnableRaisingEvents = true;
        }

        public static T GetConfig<T>(string fileName)
            where T : ConfigBase
        {
            lock (LockObj)
            {
                if (ConfigCache.ContainsKey(fileName))
                {
                    return ConfigCache[fileName].CastTo<T>();
                }
                var path = Path.Combine(ConfigPath, fileName);
                if (!File.Exists(path))
                    return null;
                var config = XmlHelper.XmlDeserialize<T>(path);
                ConfigCache.Add(fileName, config);
                return config;
            }
        }

        public static void SetConfig<T>(string fileName, T config)
            where T : ConfigBase
        {
            var path = Path.Combine(ConfigPath, fileName);
            XmlHelper.XmlSerialize(path, config);
        }

        private static void Reset(object sender, FileSystemEventArgs e)
        {
            if (ConfigCache.ContainsKey(e.Name))
                ConfigCache.Remove(e.Name);
        }
    }
}
