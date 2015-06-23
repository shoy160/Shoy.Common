using System.Threading.Tasks;

namespace Shoy.Backgrounder
{
    public interface IJobHost
    {
        void DoWork(Task worker);
    }
}
