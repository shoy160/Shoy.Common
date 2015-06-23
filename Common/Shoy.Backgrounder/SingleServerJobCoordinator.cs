using System.Threading.Tasks;

namespace Shoy.Backgrounder
{
    public class SingleServerJobCoordinator : IJobCoordinator
    {
        public Task GetWork(IJob job)
        {
            return job.Execute();
        }

        public void Dispose()
        {
        }
    }
}
