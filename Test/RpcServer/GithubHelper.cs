using System.Linq;
using ServiceStack;
using ServiceStack.Text;

namespace RpcServer
{
    public class GithubHelper
    {
        public class GithubRepository
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public int Watchers { get; set; }
            public int Forks { get; set; }
        }
        public void Get()
        {
            const string orgName = "Shoy160";
            var orgRepos = "https://api.github.com/orgs/{0}/repos".Fmt(orgName)
                .GetJsonFromUrl(req => req.UserAgent = "Gistlyn")
                .FromJson<GithubRepository[]>()
                .Take(5)
                .ToList();
            orgRepos.PrintDump();

        }
    }
}
