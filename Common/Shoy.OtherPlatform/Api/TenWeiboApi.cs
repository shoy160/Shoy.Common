﻿using System.Text;
using Shoy.OtherPlatform.Entity;
using Shoy.Utility.Extend;

namespace Shoy.OtherPlatform.Api
{
    public class TenWeiboApi
    {
        static string key = "";
        static string appId = "";

        private const string AuthorizeUrl =
            "https://open.t.qq.com/cgi-bin/oauth2/authorize?client_id={0}&response_type=code&redirect_uri={1}";

        private const string TokenUrl =
            "https://open.t.qq.com/cgi-bin/oauth2/access_token?client_id={0}&client_secret={1}&redirect_uri={2}&grant_type=authorization_code&code={3}";

        internal static string LoginLik(string callBackUrl)
        {

            var u = PlatformUtility.GetAppInfoFromCaching(PlatformType.TenWeibo);
            appId = u.AppId;
            key = u.Key;
            return AuthorizeUrl.FormatWith(appId, callBackUrl);
        }

        internal static UserInfo GetUserInfo(string code, string callBackUrl)
        {
            string context = GetAccessToken(code, callBackUrl);

            var info = PlatformUtility.GetContext(context);
            var uInfo = new UserInfo();
            if (info["openid"].IsNotNullOrEmpty())
            {
                uInfo.Uid = info["openid"];
                uInfo.NickName = info["nick"];
            }
            else
            {
                uInfo.State = info["errorCode"];
                uInfo.Msg = info["errorMsg"];
            }
            return uInfo;
        }

        private static string GetAccessToken(string code, string callBackUrl)
        {
            string url = TokenUrl.FormatWith(appId, key, callBackUrl, code);
            return url.As<IHtml>().GetHtml(Encoding.UTF8);//返回的不是单一accesstoken 带实体类。
        }
    }
}