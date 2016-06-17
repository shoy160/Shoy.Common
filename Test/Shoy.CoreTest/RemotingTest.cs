using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Shoy.CoreTest
{
    public class RemotingTest
    {
        public void StartService()
        {
            ChannelServices.RegisterChannel(new TcpChannel(9932), false);
            RemotingConfiguration.ApplicationName = "RemotingService";
            RemotingConfiguration.RegisterActivatedServiceType(typeof (int));
        }
    }
}
