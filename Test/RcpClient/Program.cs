using Grpc.Core;
using RpcContracts;
using System;

namespace RcpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var channel = new Channel("127.0.0.1:9007", ChannelCredentials.Insecure);
            var client = new gRPC.gRPCClient(channel);
            var replay = client.SayHello(new HelloRequest { Name = "shoy" });
            Console.WriteLine(replay.Message);
            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}
