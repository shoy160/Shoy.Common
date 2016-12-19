using Grpc.Core;
using RpcContracts;
using Shoy.Utility;
using System;
using System.Threading.Tasks;

namespace RpcServer
{
    public class UserImpl : gRPC.gRPCBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            Console.WriteLine($"{context.Method}:{context.Peer} time:{Utils.GetTimeNow()}");
            foreach (var header in context.RequestHeaders)
            {
                Console.WriteLine($"{header.Key}:{header.Value}");
            }
            return Task.FromResult(new HelloReply { Message = "holle " + request.Name });
        }
    }
}
