using System;
using System.Diagnostics;
using log4net.Core;
using Shoy.Utility.Extend;
using Shoy.Utility.Logging;
using ILogger = log4net.Core.ILogger;

namespace Shoy.Framework.Logging
{
    public class Log4NetLog : LogBase
    {
        private readonly ILogger _logger;

        public Log4NetLog(ILoggerWrapper wrapper)
        {
            _logger = wrapper.Logger;
        }

        private static LogInfo Format(string msg, Exception ex = null)
        {
            var f = new StackFrame(6, true);
            var method = f.GetMethod();
            var fileInfo = string.Format("{0}[{1}]", f.GetFileName(), f.GetFileLineNumber());
            if (ex != null)
            {
                method = ex.TargetSite;
            }
            var result = new LogInfo
            {
                Method = string.Format("{0} {1}",
                    method.DeclaringType,
                    method.Name),
                Message = msg,
                File = string.Empty,
                Detail = string.Empty
            };
            result.File = fileInfo;
            if (ex != null)
            {
                result.Detail = ex.Format();
            }
            return result;
        }

        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {
            _logger.Log(typeof(Log4NetLog), ParseLevel(level), Format(message.ToString(), exception),
                exception);
        }

        public override bool IsTraceEnabled
        {
            get { return _logger.IsEnabledFor(Level.Trace); }
        }

        public override bool IsDebugEnabled
        {
            get { return _logger.IsEnabledFor(Level.Debug); }
        }

        public override bool IsInfoEnabled
        {
            get { return _logger.IsEnabledFor(Level.Info); }
        }

        public override bool IsWarnEnabled
        {
            get { return _logger.IsEnabledFor(Level.Warn); }
        }

        public override bool IsErrorEnabled
        {
            get { return _logger.IsEnabledFor(Level.Error); }
        }

        public override bool IsFatalEnabled
        {
            get { return _logger.IsEnabledFor(Level.Fatal); }
        }

        private Level ParseLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.All:
                    return Level.All;
                case LogLevel.Trace:
                    return Level.Trace;
                case LogLevel.Debug:
                    return Level.Debug;
                case LogLevel.Info:
                    return Level.Info;
                case LogLevel.Warn:
                    return Level.Warn;
                case LogLevel.Error:
                    return Level.Error;
                case LogLevel.Fatal:
                    return Level.Fatal;
                case LogLevel.Off:
                    return Level.Off;
                default:
                    return Level.Off;
            }
        }
    }
}
