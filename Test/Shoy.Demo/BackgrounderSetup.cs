using Shoy.Backgrounder;
using Shoy.Demo.Jobs;
using Shoy.Utility.Extend;
using Shoy.Utility.Helper;
using System;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Shoy.Demo.BackgrounderSetup), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Shoy.Demo.BackgrounderSetup), "ShutDown")]
namespace Shoy.Demo
{
    public static class BackgrounderSetup
    {
        private static readonly JobManager JobManager = CreateJobWorkersManager();

        public static void Start()
        {
            JobManager.Start();
        }

        public static void ShutDown()
        {
            JobManager.Dispose();
        }

        private static JobManager CreateJobWorkersManager()
        {
            var start = DateTime.Parse("2015-06-23 15:10:00");
            var expire = start.AddYears(2);
            var interval = start.FromMonths(1);

            IJob timedJob = new TimedJob(interval, TimeSpan.MaxValue, start, expire),
                insertJob = new InsertJob(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(50)),
                updateJob = new UpdateJob(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(50));
            var jobs = new[] { insertJob, updateJob, timedJob };
            //new WebFarmJobCoordinator(new )
            var manager = new JobManager(jobs,
                s => FileHelper.WriteFile("FilePath".Config(string.Empty), s));
            manager.Fail(FileHelper.WriteException);
            manager.RestartSchedulerOnFailure = true;
            return manager;
        }
    }
}