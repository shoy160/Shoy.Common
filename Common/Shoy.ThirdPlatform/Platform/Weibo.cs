using System.Web;
using Shoy.ThirdPlatform.Api;
using Shoy.ThirdPlatform.Entity;

namespace Shoy.ThirdPlatform.Platform
{
    public class Weibo : PlatformFactory
    {
        public override string CreateLoginUrl(string callBackUrl)
        {
            return WeiboHelper.SinaLoginUrl(callBackUrl);
        }

        public override UserBase GetUserInfo(HttpContext httpContext, string callBackUrl)
        {
            
            string code = httpContext.Request["code"];
            if (string.IsNullOrEmpty(code))
            {
                return new UserBase {Msg = "未接受到请求参数"};
            }
            return WeiboHelper.GetUserInfo(code, callBackUrl);
        }
    }
}
