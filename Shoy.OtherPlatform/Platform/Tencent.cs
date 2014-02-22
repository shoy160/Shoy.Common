using System.Web;
using Shoy.OtherPlatform.Api;
using Shoy.OtherPlatform.Entity;

namespace Shoy.OtherPlatform.Platform
{
    public class Tencent : PlatformFactory
    {
        public override string CreateLoginUrl(string callBackUrl)
        {
            return QQApi.QQLoginLik(callBackUrl);
        }

        public override UserInfo GetUserInfo(HttpContext httpContext, string callBackUrl)
        {
            string code = httpContext.Request.QueryString["code"];
            if (string.IsNullOrEmpty(code))
            {
                return new UserInfo { Msg = "未接受到请求参数" };
            }
            return QQApi.GetUserInfo(code, callBackUrl);
        }
    }
}
