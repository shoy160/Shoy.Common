
using Shoy.Core;
using Shoy.Core.Domain.Repositories;
using Shoy.Utility;
using Shoy.Wiki.Models;

namespace Shoy.Wiki.Contracts
{
    public interface IUserContract : IDependency
    {
        string CreateUser(string account, string pwd, string name, int role);
        DResult<User> Login(string account, string pwd);
    }
}