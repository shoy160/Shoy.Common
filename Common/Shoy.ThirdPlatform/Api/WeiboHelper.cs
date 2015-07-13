using System.Text;
using Shoy.ThirdPlatform.Entity;
using Shoy.ThirdPlatform.Entity.Config;
using Shoy.Utility;
using Shoy.Utility.Extend;

namespace Shoy.ThirdPlatform.Api
{
    internal class WeiboHelper : HelperBase
    {
        private WeiboHelper() { }
        private static string AccessToken(string code, string callBackUrl)
        {
            var url = Config.TokenUrl.FormatWith(Config.Partner, Config.Key, callBackUrl, code);
            string content = url.As<IHtml>().GetHtml(Encoding.UTF8, "POST");
            return content;
        }
        protected override void Init()
        {
            LoadPlatform(PlatformType.Weibo);
        }

        public override string LoginUrl(string callback)
        {
            return Config.AuthorizeUrl.FormatWith(Config.Partner, callback);
        }

        public override UserBase Login(string callbackUrl)
        {
            var code = "code".Query(string.Empty);

            var accessToken = AccessToken(code, callbackUrl);
            var info = new WeiboUser();
            if (accessToken.IsNullOrEmpty())
                info.Message = "授权失败！";
            else
            {
                var val = PlatformUtility.GetContext(accessToken);
                info.UserId = val["uid"];
                var token = val["access_token"];
                var json = Config.UserUrl.FormatWith(info.UserId, token).As<IHtml>().GetHtml(Encoding.UTF8);
                val = PlatformUtility.GetContext(json);
                info.Message = val["error"];
                if (info.Message.IsNotNullOrEmpty())
                    info.Status = false;
                else
                {
                    info.Gender = (val["gender"] == "m" ? "男" : "女");
                    info.Nick = val["name"];
                    info.Profile = val["profile_image_url"];
                }
            }
            return info;
        }
    }
}
