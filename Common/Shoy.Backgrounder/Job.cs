using System;
using System.Threading.Tasks;

namespace Shoy.Backgrounder
{
    /// <summary> 后台任务基类 </summary>
    public abstract class Job : IJob
    {
        /// <summary>
        /// 任务构造函数
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="interval">间隔</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="start">开始时间</param>
        /// <param name="expire">失效时间</param>
        protected Job(string name, TimeSpan interval, TimeSpan timeout,
            DateTime? start = null, DateTime? expire = null)
        {
            Name = name;
            Interval = interval;
            TimeOut = timeout;
            StartTime = start;
            ExpireTime = expire;
        }

        protected Job(string name, TimeSpan interval)
            : this(name, interval, TimeSpan.MaxValue)
        {
        }

        public string Name { get; private set; }

        public abstract Task Execute();

        public DateTime? StartTime { get; private set; }
        public DateTime? ExpireTime { get; private set; }

        public TimeSpan Interval { get; private set; }

        public TimeSpan TimeOut { get; private set; }
    }
}
