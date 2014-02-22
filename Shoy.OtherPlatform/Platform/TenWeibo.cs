using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shoy.OtherPlatform.Api;
using Shoy.OtherPlatform.Entity;

namespace Shoy.OtherPlatform.Platform
{
    public class TenWeibo:PlatformFactory
    {
        public override string CreateLoginUrl(string callBackUrl)
        {
          return  TenWeiboApi.LoginLik(callBackUrl);
        }

        public override Shoy.OtherPlatform.Entity.UserInfo GetUserInfo(System.Web.HttpContext httpContent, string callBackUrl)
        {
            string code=httpContent.Request.QueryString["code"];
            if (string.IsNullOrEmpty(code))
            {
                return new UserInfo() {Msg="未接受到请求参数" };
            }
           return TenWeiboApi.GetUserInfo(code, callBackUrl);
        }
    }
}
