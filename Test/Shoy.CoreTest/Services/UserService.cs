

using System.Collections.Generic;

namespace Shoy.CoreTest.Services
{
    public class UserService : IUserService
    {
        public string Hello(string name)
        {
            return string.Format("Hello {0}!", name);
        }

        public Dictionary<string, string> Users()
        {
            return new Dictionary<string, string>
            {
                {"shay", "luoyong"},
                {"dd", "dsds"}
            };
        }
    }
}
