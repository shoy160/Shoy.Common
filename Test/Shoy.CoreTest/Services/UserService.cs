

namespace Shoy.CoreTest.Services
{
    public class UserService : IUserService
    {
        public string Hello(string name)
        {
            return string.Format("Hello {0}!", name);
        }
    }
}
