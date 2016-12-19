using Grpc.Core;
using RpcContracts;
using System;

namespace RpcServer
{
    class Program
    {
        private const int Port = 9007;
        static void Main(string[] args)
        {
            new GithubHelper().Get();
            //var server = new Server
            //{
            //    Services = { gRPC.BindService(new UserImpl()) },
            //    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            //};
            //server.Start();
            //Console.WriteLine("gRPC server listening on port " + Port);
            //Console.ReadKey();

            //server.ShutdownAsync().Wait();
        }
    }
}
