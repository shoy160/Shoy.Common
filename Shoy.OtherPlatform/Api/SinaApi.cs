using System.Text;
using Shoy.OtherPlatform.Entity;
using Shoy.Utility.Extend;

namespace Shoy.OtherPlatform.Api
{
    internal static class SinaApi
    {
        private static string _appId = "3737024472";
        private static string _key = "5224ea86e06ea8d1d0b6f437d3835847";

        private const string TokenUrl =
            "https://api.weibo.com/oauth2/access_token?client_id={0}&client_secret={1}&grant_type=authorization_code&redirect_uri={2}&code={3}";

        private const string AuthorizeUrl = "https://api.weibo.com/oauth2/authorize?client_id={0}&response_type=code&redirect_uri={1}";

        private const string UserUrl = "https://api.weibo.com/2/users/show.json?uid={0}&access_token={1}";

        private static string AccessToken(string code, string callBackUrl)
        {
            var url = TokenUrl.FormatWith(_appId, _key, callBackUrl, code);
            string content = url.As<IHtml>().GetHtml("POST", "", Encoding.UTF8);
            return content;
        }

        internal static string SinaLoginUrl(string callBackUrl)
        {
            var u = PlatformUtility.GetAppInfoFromCaching(PlatformType.SinaWeibo);
            _appId = u.AppId;
            _key = u.Key;
            return AuthorizeUrl.FormatWith(_appId, callBackUrl);
        }

        internal static UserInfo GetUserInfo(string code, string callBackUrl)
        {
            string accessToken = AccessToken(code, callBackUrl);
            var info = new UserInfo();
            if (accessToken.IsNullOrEmpty())
                info.Msg = "授权失败！";
            else
            {
                var val = PlatformUtility.GetContext(accessToken);
                info.Uid = val["uid"];
                var token = val["access_token"];
                var json = UserUrl.FormatWith(info.Uid, token).As<IHtml>().GetHtml(Encoding.UTF8);
                val = PlatformUtility.GetContext(json);
                info.Msg = val["error"];
                if (info.Msg.IsNotNullOrEmpty())
                    info.State = "-1";
                else
                {
                    info.Gender = (val["gender"] == "m" ? "男" : "女");
                    info.NickName = val["name"];
                    info.ImgSrc = val["profile_image_url"];
                }
            }
            return info;
        }
    }
}
