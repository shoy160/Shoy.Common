using System;

namespace Shoy.Test
{
    public class UserService : IUserService
    {
        public void WriteUser()
        {
            Console.WriteLine("user service 001");
        }
    }
}
