using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using Shoy.Utility.Logging;

namespace Shoy.Core.Logging
{
    public class Log4NetAdapter : LoggerAdapterBase
    {
        /// <summary>
        /// 初始化一个<see cref="Log4NetAdapter"/>类型的新实例
        /// </summary>
        public Log4NetAdapter()
        {
            var appender = new RollingFileAppender
            {
                Name = "root",
                File = "logs\\log_",
                AppendToFile = true,
                LockingModel = new FileAppender.MinimalLock(),
                RollingStyle = RollingFileAppender.RollingMode.Date,
                DatePattern = "yyyyMMdd-HH\".log\"",
                StaticLogFileName = false,
                Threshold = Level.Debug,
                MaxSizeRollBackups = 10,
                Layout = new PatternLayout("%n[%d{yyyy-MM-dd HH:mm:ss.fff}] %-5p %c %t %w %n%m%n")
            };
            appender.ClearFilters();
            appender.AddFilter(new LevelMatchFilter { LevelToMatch = Level.Info });
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
