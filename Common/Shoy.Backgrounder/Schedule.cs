using System;

namespace Shoy.Backgrounder
{
    /// <summary> 后台任务进度 </summary>
    public class Schedule : IDisposable
    {
        readonly Func<DateTime> _nowThunk;

        public Schedule(IJob job)
            : this(job, () => DateTime.UtcNow)
        {
        }

        public Schedule(IJob job, Func<DateTime> nowThunk)
        {
            if (job == null)
            {
                throw new ArgumentNullException("job");
            }
            Job = job;
            _nowThunk = nowThunk;
            _lastRunTime = (Job.StartTime.HasValue ? Job.StartTime.Value : nowThunk());
        }

        public IJob Job { get; private set; }

        private DateTime _lastRunTime;

        public DateTime NextRunTime
        {
            get
            {
                var next = _lastRunTime.Add(Job.Interval);
                if (Job.ExpireTime.HasValue && Job.ExpireTime < next)
                    return DateTime.MaxValue;
                return next;
            }
        }

        public TimeSpan GetIntervalToNextRun()
        {
            var now = _nowThunk();
            if (NextRunTime < now)
            {
                return TimeSpan.FromMilliseconds(1);
            }
            return NextRunTime - now;
        }

        private void SetRunComplete()
        {
            _lastRunTime = _nowThunk();
        }

        void IDisposable.Dispose()
        {
            SetRunComplete();
        }
    }
}
