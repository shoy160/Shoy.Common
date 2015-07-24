using System.Collections.Specialized;
using System.Text;
using Shoy.ThirdPlatform.Entity;
using Shoy.ThirdPlatform.Entity.Config;
using Shoy.Utility;
using Shoy.Utility.Extend;

namespace Shoy.ThirdPlatform.Helper
{
    /// <summary> 微信登录 </summary>
    internal class Weixin : HelperBase
    {
        private NameValueCollection AccessToken(string code)
        {
            var url = Config.TokenUrl.FormatWith(Config.Partner, Config.Key, code);
            string content = url.As<IHtml>().GetHtml(Encoding.UTF8);
            return PlatformUtility.GetContext(content);
        }

        protected override void Init()
        {
            LoadPlatform(PlatformType.Weixin);
            Callback = string.Format(Callback, PlatformType.Weixin.GetValue());
        }

        public override string LoginUrl()
        {
            return string.Format(Config.AuthorizeUrl, Config.Partner, Callback.UrlEncode(), string.Empty);
        }

        public override DResult<UserResult> User()
        {
            var code = "code".Query(string.Empty);
            var col = AccessToken(code);
            if (string.IsNullOrWhiteSpace(col["access_token"]))
                return new DResult<UserResult>("授权失败！");
            var result = new UserResult
            {
                Id = col["openid"],
                AccessToken = col["access_token"]
            };
            var url = string.Format(Config.UserUrl, result.AccessToken, result.Id);
            var html = url.As<IHtml>().GetHtml(Encoding.UTF8);
            var userCollect = PlatformUtility.GetContext(html);
            if (!string.IsNullOrWhiteSpace(userCollect["errcode"]))
                return new DResult<UserResult>(userCollect["errmsg"]);
            result.Nick = userCollect["nickname"];
            result.Gender = (userCollect["sex"] == "1" ? "男" : "女");
            result.Profile = userCollect["headimgurl"];
            return new DResult<UserResult>(true, result);
        }
    }
}
