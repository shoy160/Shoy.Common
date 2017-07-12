using Shoy.Wpf.Helper;
using System;
using System.IO;

namespace Shoy.Wpf.Logging.Log4Net
{
    public class Log4NetAdapter : LoggerAdapterBase
    {
        private const string FileName = "log4net.config";

        private static string ConfigPath => ConfigHelper.GetAppSetting(defaultValue: string.Empty);
        private static string LogSite => ConfigHelper.GetAppSetting(defaultValue: "local");

        /// <summary>
        /// 初始化一个<see cref="Log4NetAdapter"/>类型的新实例
        /// </summary>k
        public Log4NetAdapter()
        {
            var configFile = Path.Combine(ConfigPath, FileName);
            if (File.Exists(configFile))
            {
                log4net.GlobalContext.Properties["LogSite"] = LogSite;
                XmlConfigurator.ConfigureAndWatch(new FileInfo(configFile));
                return;
            }
            var appender = new RollingFileAppender
            {
                Name = "root",
                File = $"_logs\\local\\{DateTime.Now:yyyyMM}\\",
                AppendToFile = true,
                LockingModel = new FileAppender.MinimalLock(),
                RollingStyle = RollingFileAppender.RollingMode.Date,
                DatePattern = "dd\".log\"",
                StaticLogFileName = false,
                MaxSizeRollBackups = 100,
                MaximumFileSize = "2MB",
                Layout = new PatternLayout("[%d{yyyy-MM-dd HH:mm:ss.fff}] %-5p %c %t %w %n%m%n")
                //Layout = new PatternLayout("[%d [%t] %-5p %c [%x] - %m%n]")
            };
            appender.ClearFilters();
            appender.AddFilter(new LevelRangeFilter
            {
                LevelMin = Level.Debug,
                LevelMax = Level.Fatal
            });
            BasicConfigurator.Configure(appender);
            appender.ActivateOptions();
        }

        protected override ILog CreateLogger(string name)
        {
            var log = log4net.LogManager.GetLogger(name);
            return new Log4NetLog(log);
        }
    }
}
