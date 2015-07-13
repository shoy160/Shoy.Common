using Shoy.ThirdPlatform.Api;
using Shoy.ThirdPlatform.Entity;

namespace Shoy.ThirdPlatform.Platform
{
    public class TencentWeibo:PlatformFactory
    {
        public override string CreateLoginUrl(string callBackUrl)
        {
          return  TencentWeiboHelper.LoginLik(callBackUrl);
        }

        public override UserBase GetUserInfo(System.Web.HttpContext httpContent, string callBackUrl)
        {
            string code=httpContent.Request.QueryString["code"];
            if (string.IsNullOrEmpty(code))
            {
                return new UserBase() {Msg="未接受到请求参数" };
            }
           return TencentWeiboHelper.GetUserInfo(code, callBackUrl);
        }
    }
}
