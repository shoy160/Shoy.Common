using System;
using System.Threading.Tasks;

namespace Shoy.Demo.Jobs
{
    public class InsertJob : JobBase
    {
        public InsertJob(TimeSpan interval, TimeSpan timeout)
            : base("InsertJob", interval, timeout)
        {
        }

        public override Task Execute()
        {
            return Task.Factory.StartNew(() => Write(Name));
        }
    }
}