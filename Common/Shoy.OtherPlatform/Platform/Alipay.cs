using System.Web;
using Shoy.OtherPlatform.Api;
using Shoy.OtherPlatform.Entity;
 

namespace Shoy.OtherPlatform.Platform
{
    public class Alipay : PlatformFactory
    {
        public override string CreateLoginUrl(string callBackUrl)
        {
            return AlipayUtils.CreateUrl(callBackUrl);
        }

        public override UserInfo GetUserInfo(HttpContext httpContext, string callBackUrl)
        {
            //return Api.ali_function.GetAlipayUserID();
            return AlipayUtils.GetUserInfo(httpContext, callBackUrl);
        }
    }
}
