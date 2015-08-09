using System;

namespace Shoy.Test
{
    public class AnotherService:IUserService
    {
        public void WriteUser()
        {
            Console.WriteLine("another service !");
        }
    }
}
