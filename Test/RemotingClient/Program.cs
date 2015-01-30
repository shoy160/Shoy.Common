using System;
using System.Configuration;
using System.Runtime.Remoting.Channels.Tcp;
using RemotingModels;
using System.Runtime.Remoting.Channels;

namespace RemotingClient
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                ChannelServices.RegisterChannel(new TcpClientChannel(), true);
                var url = ConfigurationManager.AppSettings.Get("ServiceURL");
                Console.WriteLine(url);
                var app = (Person) Activator.GetObject(typeof (Person), url);
                Console.WriteLine(app.Add(1, 2));
                Console.WriteLine(app.GetInfo());
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
