using System.Text;
using Shoy.ThirdPlatform.Entity;
using Shoy.ThirdPlatform.Entity.Config;
using Shoy.Utility;
using Shoy.Utility.Extend;

namespace Shoy.ThirdPlatform.Helper
{
    /// <summary> 腾讯微博登录 </summary>
    internal class TencentWeibo : HelperBase
    {
        private static string GetAccessToken(string code, string callBackUrl)
        {
            string url = Config.TokenUrl.FormatWith(Config.Partner, Config.Key, callBackUrl, code);
            return url.As<IHtml>().GetHtml(Encoding.UTF8); //返回的不是单一accesstoken 带实体类。
        }

        protected override void Init()
        {
            LoadPlatform(PlatformType.TencentWeibo);
            Callback = string.Format(Callback, PlatformType.TencentWeibo.GetValue());
        }

        public override string LoginUrl()
        {
            return Config.AuthorizeUrl.FormatWith(Config.Partner, Callback.UrlEncode());
        }

        public override DResult<UserResult> User()
        {
            var code = "code".Query(string.Empty);
            string context = GetAccessToken(code, Callback);

            var info = PlatformUtility.GetContext(context);
            if (info["errorCode"] == "0")
            {
                return new DResult<UserResult>(true, new UserResult
                {
                    Id = info["openid"],
                    Nick = info["nick"]
                });
            }
            return new DResult<UserResult>(info["errorMsg"]);
        }
    }
}
