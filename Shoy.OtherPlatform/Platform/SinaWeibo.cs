using System.Web;
using Shoy.OtherPlatform.Api;
using Shoy.OtherPlatform.Entity;

namespace Shoy.OtherPlatform.Platform
{
    public class SinaWeibo : PlatformFactory
    {
        public override string CreateLoginUrl(string callBackUrl)
        {
            return SinaApi.SinaLoginUrl(callBackUrl);
        }

        public override UserInfo GetUserInfo(HttpContext httpContext, string callBackUrl)
        {
            
            string code = httpContext.Request["code"];
            if (string.IsNullOrEmpty(code))
            {
                return new UserInfo {Msg = "未接受到请求参数"};
            }
            return SinaApi.GetUserInfo(code, callBackUrl);
        }
    }
}
