using System.Web;
using Shoy.ThirdPlatform.Api;
using Shoy.ThirdPlatform.Entity;

namespace Shoy.ThirdPlatform.Platform
{
    public class Alipay : PlatformFactory
    {
        public override string CreateLoginUrl(string callBackUrl)
        {
            return AlipayHelper.CreateUrl(callBackUrl);
        }

        public override UserBase GetUserInfo(HttpContext httpContext, string callBackUrl)
        {
            //return Api.ali_function.GetAlipayUserID();
            return AlipayHelper.GetUserInfo(httpContext, callBackUrl);
        }
    }
}
