using System.Text;
using Shoy.ThirdPlatform.Entity;
using Shoy.ThirdPlatform.Entity.Config;
using Shoy.ThirdPlatform.Entity.Result;
using Shoy.Utility;
using Shoy.Utility.Extend;
using Shoy.Utility.Helper;

namespace Shoy.ThirdPlatform.Helper
{
    /// <summary> 腾讯QQ登录 </summary>
    internal class Tencent : HelperBase
    {
        private static string GetAccessToken(string code, string callBackUrl)
        {
            var url = Config.TokenUrl.FormatWith(Config.Partner, Config.Key, code, callBackUrl);
            string content = url.As<IHtml>().GetHtml(Encoding.UTF8);
            var val = PlatformUtility.GetContext(content);
            if (val["access_token"].IsNotNullOrEmpty())
                return val["access_token"];
            return string.Empty;
        }
        private static string GetOpenId(string accessToken)
        {
            string url = Config.OpenIdUrl.FormatWith(accessToken);
            string content = url.As<IHtml>().GetHtml(Encoding.UTF8);
            return RegexHelper.Match(content, "\"openid\":\"(?<t>.*)?\"}", 1, "t");
        }

        private static string GetUserInfoString(string accessToken, string openId)
        {
            string url = Config.UserUrl.FormatWith(accessToken, Config.Partner, openId);
            return url.As<IHtml>().GetHtml(Encoding.UTF8);
        }

        protected override void Init()
        {
            LoadPlatform(PlatformType.Tencent);
            Callback = string.Format(Callback, PlatformType.Tencent.GetValue());
        }

        public override string LoginUrl()
        {
            return Config.AuthorizeUrl.FormatWith(Config.Partner, Callback.UrlEncode());
        }

        public override DResult<UserResult> User()
        {
            var code = "code".Query(string.Empty);
            string accessToken = GetAccessToken(code, Callback);
            string openId = GetOpenId(accessToken);
            string userJson = GetUserInfoString(accessToken, openId);
            try
            {
                var result = userJson.JsonToObject<TencentResult>();
                if (result.ret == 0)
                {
                    return new DResult<UserResult>(true, new UserResult
                    {
                        Profile = result.figureurl_2,
                        Gender = result.gender,
                        Nick = result.nickname,
                        Id = openId,
                        AccessToken = accessToken
                    });
                }
                return new DResult<UserResult>(result.msg);
            }
            catch
            {
                return new DResult<UserResult>("获取用户数据失败！");
            }
        }
    }
}
