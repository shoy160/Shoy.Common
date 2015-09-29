using System.ServiceModel;
using Shoy.Core.Wcf;

namespace Shoy.CoreTest.Services
{
    [ServiceContract]
    public interface IUserService : IWcfService
    {
        string Hello(string name);
    }
}
