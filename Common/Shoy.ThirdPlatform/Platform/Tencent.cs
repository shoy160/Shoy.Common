using System.Web;
using Shoy.ThirdPlatform.Api;
using Shoy.ThirdPlatform.Entity;

namespace Shoy.ThirdPlatform.Platform
{
    public class Tencent : PlatformFactory
    {
        public override string CreateLoginUrl(string callBackUrl)
        {
            return TencentHelper.QQLoginLik(callBackUrl);
        }

        public override UserBase GetUserInfo(HttpContext httpContext, string callBackUrl)
        {
            string code = httpContext.Request.QueryString["code"];
            if (string.IsNullOrEmpty(code))
            {
                return new UserBase { Msg = "未接受到请求参数" };
            }
            return TencentHelper.GetUserInfo(code, callBackUrl);
        }
    }
}
