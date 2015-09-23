using System.IO;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using Shoy.Utility.Helper;
using Shoy.Utility.Logging;

namespace Shoy.Framework.Logging
{
    public class Log4NetAdapter : LoggerAdapterBase
    {
        private const string FileName = "log4net.config";

        private static string ConfigPath
        {
            get { return ConfigHelper.GetAppSetting(defaultValue: string.Empty); }
        }

        private static string SiteName
        {
            get { return ConfigHelper.GetAppSetting(defaultValue: string.Empty); }
        }

        /// <summary>
        /// 初始化一个<see cref="Log4NetAdapter"/>类型的新实例
        /// </summary>k
        public Log4NetAdapter()
        {
//            var configFile = Path.Combine(ConfigPath, FileName);
//            if (File.Exists(configFile))
//            {
//                XmlConfigurator.ConfigureAndWatch(new FileInfo(configFile));
//                return;
//            }
            var appender = new RollingFileAppender
            {
                Name = "root",
                File = "logs\\log_",
                AppendToFile = true,
                LockingModel = new FileAppender.MinimalLock(),
                RollingStyle = RollingFileAppender.RollingMode.Date,
                DatePattern = "yyyyMMdd-HH\".log\"",
                StaticLogFileName = false,
                MaxSizeRollBackups = 10,
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
