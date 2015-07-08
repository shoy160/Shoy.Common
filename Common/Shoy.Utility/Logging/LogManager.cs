
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Shoy.Utility.Extend;

namespace Shoy.Utility.Logging
{
    public static class LogManager
    {
        private static readonly ConcurrentDictionary<string, Logger> LoggerDictionary;
        private const string MessageFormat = "Method:{1}({2}) {3}:{4}{0}Msg:{5}{0}";
        private static readonly object LockObj = new object();
        /// <summary>
        /// 日志适配器集合
        /// </summary>
        private static readonly ICollection<ILoggerAdapter> LoggerAdapters;

        private static LogLevel _logLevel;
        private static readonly bool CanSetLevel = true;

        /// <summary>
        /// 静态构造
        /// </summary>
        static LogManager()
        {
            LoggerDictionary = new ConcurrentDictionary<string, Logger>();
            LoggerAdapters = new List<ILoggerAdapter>();
            var level = "LogLevel".Config(string.Empty);
            if (string.IsNullOrWhiteSpace(level))
                return;
            _logLevel = level.CastTo(LogLevel.Off);
            CanSetLevel = false;
        }
        /// <summary>
        /// 是否启用日志级别
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private static bool IsEnableLevel(LogLevel level)
        {
            return level >= _logLevel;
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
        public static Logger Logger(string name)
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
        public static Logger Logger(Type type)
        {
            return Logger(type.FullName);
        }

        /// <summary>
        /// 获取日志记录实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Logger Logger<T>()
        {
            return Logger(typeof(T));
        }

        public static void SetLevel(LogLevel level)
        {
            if (!CanSetLevel)
                return;
            _logLevel = level;
        }

        internal static IEnumerable<ILog> GetAdapters(string name)
        {
            lock (LockObj)
            {
                return LoggerAdapters.Select(t => t.GetLogger(name));
            }
        }

        internal static void EachAdapter(this string loggerName, LogLevel level, Action<ILog> action)
        {
            if (String.IsNullOrWhiteSpace(loggerName) || !IsEnableLevel(level))
                return;
            foreach (ILog log in GetAdapters(loggerName))
            {
                action(log);
            }
        }

        public static string Format(string msg)
        {
            var f = new StackFrame(1, true);
            return String.Format(MessageFormat, Environment.NewLine,
                f.GetMethod().DeclaringType, f.GetFileName(),
                f.GetMethod().Name, f.GetFileLineNumber(), msg);
        }
    }
}
