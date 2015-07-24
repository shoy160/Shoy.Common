using System;
using System.Threading.Tasks;

namespace Shoy.Demo.Jobs
{
    public class UpdateJob : JobBase
    {
        public UpdateJob(TimeSpan interval, TimeSpan timeout)
            : base("UpdateJob", interval, timeout)
        {
        }

        public override Task Execute()
        {
            return Task.Factory.StartNew(() => Write(Name));
        }
    }
}