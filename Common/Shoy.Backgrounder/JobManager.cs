using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Shoy.Backgrounder
{
    /// <summary> 后台任务管理者 </summary>
    public class JobManager : IDisposable
    {
        private readonly IJobHost _host;
        private readonly Timer _timer;
        private readonly IJobCoordinator _coordinator;
        private readonly Scheduler _scheduler;
        private readonly IEnumerable<IJob> _jobs;
        private Action<Exception> _failHandler;
        private readonly Action<string> _logAction;

        /// <summary> 任务失败后是否重新启动 </summary>
        public bool RestartSchedulerOnFailure { private get; set; }

        public JobManager(IEnumerable<IJob> jobs, Action<string> logAction = null)
            : this(jobs, new JobHost(), new SingleServerJobCoordinator(), logAction)
        {
        }
        public JobManager(IEnumerable<IJob> jobs, IJobHost host)
            : this(jobs, host, new SingleServerJobCoordinator()) { }

        public JobManager(IEnumerable<IJob> jobs, IJobCoordinator coordinator)
            : this(jobs, new JobHost(), coordinator) { }

        public JobManager(IEnumerable<IJob> jobs, IJobHost host, IJobCoordinator coordinator,
            Action<string> logAction = null)
        {
            if (jobs == null)
            {
                throw new ArgumentNullException("jobs");
            }
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            if (coordinator == null)
            {
                throw new ArgumentNullException("coordinator");
            }

            _jobs = jobs;
            _scheduler = new Scheduler(jobs);
            _host = host;
            _coordinator = coordinator;
            _timer = new Timer(OnTimerElapsed);
            _logAction = logAction;
        }

        public void Start()
        {
            _timer.Next(TimeSpan.FromMilliseconds(1));
        }

        public void Stop()
        {
            _timer.Stop();
        }

        void OnTimerElapsed(object sender)
        {
            try
            {
                _timer.Stop();
                DoNextJob();
                _timer.Next(_scheduler.Next().GetIntervalToNextRun()); // Start up again.
            }
            catch (Exception e)
            {
                OnException(e); // Someone else's problem.

                if (RestartSchedulerOnFailure)
                {
                    _timer.Next(_scheduler.Next().GetIntervalToNextRun()); // Start up again.
                }
            }
        }

        void DoNextJob()
        {
            using (var schedule = _scheduler.Next())
            {
                var work = _coordinator.GetWork(schedule.Job);

                if (work != null)
                {
                    _host.DoWork(work);
                }
            }
            if (_logAction != null)
                _logAction(_scheduler.ToString());
        }

        public void Dispose()
        {
            Stop();
            foreach (var job in _jobs.OfType<IDisposable>())
            {
                job.Dispose();
            }
            _timer.Dispose();
            _coordinator.Dispose();
        }

        public void Fail(Action<Exception> failHandler)
        {
            _failHandler = failHandler;
        }

        private void OnException(Exception e)
        {
            var fail = _failHandler;
            if (fail != null)
            {
                fail(e);
            }
        }
    }
}
