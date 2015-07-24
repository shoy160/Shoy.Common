

//[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Shoy.Backgrounder.BackgrounderSetup), "Start")]
//[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Shoy.Backgrounder.BackgrounderSetup), "ShutDown")]

using System;

namespace Shoy.Backgrounder
{
    /// <summary> Setup示例，需程序中定义 </summary>
    [Obsolete]
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
            var jobs = new IJob[]
            {

            };
            //var coordinator = new SingleServerJobCoordinator();
            var coordinator = new SingleServerJobCoordinator();
            //new WebFarmJobCoordinator(new )
            var manager = new JobManager(jobs, coordinator);
            //manager.Fail(ex => );
            return manager;
        }
    }
}
