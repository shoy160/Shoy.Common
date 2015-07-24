using System;
using System.Threading.Tasks;

namespace Shoy.Demo.Jobs
{
    public class TimedJob : JobBase
    {
        public TimedJob(TimeSpan interval, TimeSpan timeOut, DateTime start, DateTime? expire = null)
            : base("TimedJob", interval, timeOut, start, expire)
        {
        }

        public override Task Execute()
        {
            return Task.Factory.StartNew(() => Write(Name));
        }
    }
}