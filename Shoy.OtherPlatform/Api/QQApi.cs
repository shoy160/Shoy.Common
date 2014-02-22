using System.Text;
using System.Text.RegularExpressions;
using Shoy.OtherPlatform.Entity;
using Shoy.Utility.Extend;

namespace Shoy.OtherPlatform.Api
{
    public static class QQApi
    {
        static string appId = "";
        static string key = "";

        private const string AuthorizeUrl =
            "https://graph.qq.com/oauth2.0/authorize?response_type=code&client_id={0}&redirect_uri={1}&scope=get_user_info";

        private const string TokenUrl =
            "https://graph.qq.com/oauth2.0/token?grant_type=authorization_code&client_id={0}&client_secret={1}&code={2}&state=libo&redirect_uri={3}";

        private const string OpenIdUrl = "https://graph.qq.com/oauth2.0/me?access_token={0}";

        private const string UserUrl =
            "https://graph.qq.com/user/get_user_info?access_token={0}&oauth_consumer_key={1}&openid={2}";

        private static string GetAccessToken(string code, string callBackUrl)
        {
            var u = PlatformUtility.GetAppInfoFromCaching(PlatformType.Tencent);
            appId = u.AppId;
            key = u.Key;
            var url = TokenUrl.FormatWith(appId, key, code, callBackUrl);
            string content = url.As<IHtml>().GetHtml(Encoding.UTF8);
            var val = PlatformUtility.GetContext(content);
            if (val["access_token"].IsNotNullOrEmpty())
                return val["access_token"];
            return "";
        }

        private static string GetOpenId(string accessToken)
        {
            string url = OpenIdUrl.FormatWith(accessToken);
            string content = url.As<IHtml>().GetHtml(Encoding.UTF8);
            return RegYourString("\"openid\":\"(?<t>.*)?\"}", content);
        }

        private static string GetUserInfoString(string accessToken, string openId)
        {
            string url = UserUrl.FormatWith(accessToken, appId, openId);
            return url.As<IHtml>().GetHtml(Encoding.UTF8);
        }

        /// <summary>
        /// 使用该方法分组名称必须为t
        /// </summary>
        /// <param name="regExpress">正则表达式</param>
        /// <param name="data">要匹配的内容</param>
        /// <returns>匹配结果</returns>
        private static string RegYourString(string regExpress, string data)
        {
            return Regex.Match(data, regExpress).Groups["t"].Value;
        }

        #region 外部获取信息和操作

        /// <summary>
        /// QQ登陆链接
        /// </summary>
        /// <param name="callBackUrl"></param>
        /// <param name="scopes">权限申请,默认权限只添加了userinfo</param>
        /// <returns>带权限申请的url地址</returns>
        internal static string QQLoginLik(string callBackUrl, string[] scopes)
        {
            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(key))
            {
                var u = PlatformUtility.GetKeyInfos(PlatformType.Tencent);
                appId = u.AppId;
                key = u.Key;
            }
            string url = AuthorizeUrl.FormatWith(appId, callBackUrl);
            if (scopes != null)
            {
                url += "," + string.Join(",", scopes);
            }
            return url;
        }

        internal static string QQLoginLik(string callBackUrl)
        {
            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(key))
            {
                var u = PlatformUtility.GetKeyInfos(PlatformType.Tencent);
                appId = u.AppId;
                key = u.Key;
            }
            return AuthorizeUrl.FormatWith(appId, callBackUrl);
        }

        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="code">回掉地址url参数code</param>
        /// <param name="callBackUrl"></param>
        /// <returns>这个里面必须检测ret是否为string类型的0，不为零提示msg信息</returns>
        internal static UserInfo GetUserInfo(string code, string callBackUrl)
        {
            string accessToken = GetAccessToken(code,callBackUrl);
            string openId = GetOpenId(accessToken);
            string userJson = GetUserInfoString(accessToken, openId);
            string msg = RegYourString("\"msg\":.*\"(?<t>.*?)\",", userJson),
                   nick = RegYourString("\"nickname\":.*\"(?<t>.*?)\",", userJson),
                   imgSrc = RegYourString("\"figureurl_2\":.*\"(?<t>.*?)\",", userJson),
                   gender = RegYourString("\"gender\":.*\"(?<t>.*?)\",", userJson);
            return new UserInfo { Msg=msg, ImgSrc=imgSrc, Gender=gender, NickName=nick, Uid=openId };
        }
        #endregion
    }
}
