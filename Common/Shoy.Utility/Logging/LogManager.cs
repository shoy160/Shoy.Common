
using Shoy.Utility.Extend;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Shoy.Utility.Logging
{
    public static class LogManager
    {
        public static LogLevel Level { get; private set; }
        private static readonly ConcurrentDictionary<string, Logger> LoggerDictionary;
        private static readonly object LockObj = new object();
        /// <summary>
        /// 日志适配器集合
        /// </summary>
        internal static ICollection<ILoggerAdapter> LoggerAdapters;

        /// <summary>
        /// 静态构造
        /// </summary>
        static LogManager()
        {
            Level = ConfigurationManager.AppSettings.Get("LogLevel").CastTo(LogLevel.Off);
            LoggerDictionary = new ConcurrentDictionary<string, Logger>();
            LoggerAdapters = new List<ILoggerAdapter>();
        }

        /// <summary>
        /// 添加适配器
        /// </summary>
        /// <param name="adapter"></param>
        public static void AddAdapter(ILoggerAdapter adapter)
        {
            lock (LockObj)
            {
                if (LoggerAdapters.Any(t => t == adapter))
                    return;
                LoggerAdapters.Add(adapter);
            }
        }

        /// <summary>
        /// 移除适配器
        /// </summary>
        /// <param name="adapter"></param>
        public static void RemoveAdapter(ILoggerAdapter adapter)
        {
            lock (LockObj)
            {
                if (LoggerAdapters.Any(t => t == adapter))
                    LoggerAdapters.Remove(adapter);
            }
        }

        /// <summary>
        /// 获取日志记录实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Logger GetLogger(string name)
        {
            Logger logger;
            if (LoggerDictionary.TryGetValue(name, out logger))
            {
                return logger;
            }
            logger = new Logger(name);
            LoggerDictionary[name] = logger;
            return logger;
        }

        /// <summary>
        /// 获取日志记录实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Logger GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }

        private static IEnumerable<ILog> GetAdapters(string name)
        {
            return LoggerAdapters.Select(t => t.GetLogger(name));
        }

        internal static void EachAdapter(this string loggerName, LogLevel level, Action<ILog> action)
        {
            if (!IsEnableLevel(level))
                return;
            foreach (ILog log in GetAdapters(loggerName))
            {
                action(log);
            }
        }

        /// <summary>
        /// 是否启用日志级别
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static bool IsEnableLevel(LogLevel level)
        {
            return level >= Level;
        }
    }
}
