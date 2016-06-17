using System.Collections.Generic;
using System.ServiceModel;
using Shoy.Core.Wcf;

namespace Shoy.CoreTest.Services
{
    [ServiceContract]
    public interface IUserService : IWcfService
    {
        [OperationContract]
        string Hello(string name);

        [OperationContract]
        Dictionary<string, string> Users();
    }
}
