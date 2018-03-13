using Shoy.Utility;
using Shoy.Utility.Extend;
using Shoy.Utility.Helper;
using System;
using System.IO;
using System.Web.Http;

namespace Shay.WebApi.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet, Route("~/net")]
        public DResult NetTest(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return DResult.Success;
                var path = $"X:\\{name}";
                Directory.CreateDirectory(path);
                File.WriteAllText(Path.Combine(path, $"{name}.txt"), IdHelper.Instance.Guid32);
                return DResult.Success;
            }
            catch (Exception e)
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.txt"), e.Format());
                return DResult.Error(e.Message);
            }
        }

        [HttpGet, Route("~/net_read")]
        public DResult<string> NetRead(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return DResult.Error<string>("null");
                var path = $"X:\\{name}";
                var txtPath = Path.Combine(path, $"{name}.txt");
                var word = File.ReadAllText(txtPath);
                return DResult.Succ(word);
            }
            catch (Exception e)
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.txt"), e.Format());
                return DResult.Error<string>(e.Message);
            }
        }
    }
}
