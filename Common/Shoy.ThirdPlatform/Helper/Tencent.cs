using System.Text;
using Shoy.ThirdPlatform.Entity;
using Shoy.ThirdPlatform.Entity.Config;
using Shoy.ThirdPlatform.Entity.Result;
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
        }

        public override string LoginUrl(string callback)
        {
            return Config.AuthorizeUrl.FormatWith(Config.Partner, callback.UrlEncode());
        }

        public override UserBase Login(string callbackUrl)
        {
            var code = "code".Query(string.Empty);
            string accessToken = GetAccessToken(code, callbackUrl);
            string openId = GetOpenId(accessToken);
            string userJson = GetUserInfoString(accessToken, openId);
            try
            {
                var result = userJson.JsonToObject<TencentResult>();
                return new TencentUser
                {
                    Status = (result.ret == 0),
                    Message = result.msg,
                    Profile = result.figureurl_2,
                    Gender = result.gender,
                    Nick = result.nickname,
                    Id = openId,
                    AccessToken = accessToken
                };
            }
            catch
            {
                return new TencentUser { Status = false, Message = "获取用户数据失败！" };
            }
        }
    }
}
