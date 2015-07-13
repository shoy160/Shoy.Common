using System.Text;
using Shoy.ThirdPlatform.Entity;
using Shoy.ThirdPlatform.Entity.Config;
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
        }

        public override string LoginUrl(string callback)
        {
            return Config.AuthorizeUrl.FormatWith(Config.Partner, callback.UrlEncode());
        }

        public override UserBase Login(string callbackUrl)
        {
            var code = "code".Query(string.Empty);
            string context = GetAccessToken(code, callbackUrl);

            var info = PlatformUtility.GetContext(context);
            var uInfo = new TencentWeiboUser();
            if (info["openid"].IsNotNullOrEmpty())
            {
                uInfo.Id = info["openid"];
                uInfo.Nick = info["nick"];
            }
            else
            {
                uInfo.Status = info["errorCode"] == "0";
                uInfo.Message = info["errorMsg"];
            }
            return uInfo;
        }
    }
}
