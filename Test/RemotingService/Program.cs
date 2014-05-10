using System;
using System.Runtime.Remoting;

namespace RemotingService
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("RemotingService.exe.config", true);
            Console.WriteLine("服务已启动！");
            Console.ReadLine();
        }
    }
}
