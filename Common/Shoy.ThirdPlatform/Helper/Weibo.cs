using System.Text;
using Shoy.ThirdPlatform.Entity;
using Shoy.ThirdPlatform.Entity.Config;
using Shoy.Utility;
using Shoy.Utility.Extend;

namespace Shoy.ThirdPlatform.Helper
{
    /// <summary> 微博登录 </summary>
    internal class Weibo : HelperBase
    {
        private static string AccessToken(string code, string callBackUrl)
        {
            var url = Config.TokenUrl.FormatWith(Config.Partner, Config.Key, callBackUrl, code);
            string content = url.As<IHtml>().GetHtml(Encoding.UTF8, "POST");
            return content;
        }
        protected override void Init()
        {
            LoadPlatform(PlatformType.Weibo);
            Callback = string.Format(Callback, PlatformType.Weibo.GetValue());
        }

        public override string LoginUrl()
        {
            return Config.AuthorizeUrl.FormatWith(Config.Partner, Callback.UrlEncode());
        }

        public override DResult<UserResult> User()
        {
            var code = "code".Query(string.Empty);

            var accessToken = AccessToken(code, Callback);
            if (accessToken.IsNullOrEmpty())
                return new DResult<UserResult>("授权失败！");

            var val = PlatformUtility.GetContext(accessToken);
            if (!string.IsNullOrWhiteSpace(val["error"]))
                return new DResult<UserResult>(val["error"]);
            var info = new UserResult();
            info.Id = val["uid"];
            var token = val["access_token"];
            var json = Config.UserUrl.FormatWith(info.Id, token).As<IHtml>().GetHtml(Encoding.UTF8);
            val = PlatformUtility.GetContext(json);
            info.Gender = (val["gender"] == "m" ? "男" : "女");
            info.Nick = val["name"];
            info.Profile = val["profile_image_url"];
            return new DResult<UserResult>(true, info);
        }
    }
}
