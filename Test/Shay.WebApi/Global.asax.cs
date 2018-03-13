using Shoy.Utility.Helper;
using System.Web.Http;

namespace Shay.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //网盘配置
            var localPath = ConfigHelper.GetConfigString("net_local");
            var netPath = ConfigHelper.GetConfigString("net_path");
            if (!string.IsNullOrWhiteSpace(localPath) && !string.IsNullOrWhiteSpace(netPath))
            {
                var user = ConfigHelper.GetConfigString("net_user");
                var pwd = ConfigHelper.GetConfigString("net_pwd");
                NetStorageHelper.Connect(localPath, netPath, user, pwd);
            }

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
