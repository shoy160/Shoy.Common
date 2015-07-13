using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Shoy.ThirdPlatform.Entity;
using Shoy.ThirdPlatform.Entity.Config;
using Shoy.Utility.Extend;

namespace Shoy.ThirdPlatform.Helper
{
    /// <summary> 支付宝登录 </summary>
    internal class Alipay : HelperBase
    {
        private static ArrayList ParaFilter(ArrayList sArray)
        {
            int count = sArray.Count;
            for (int i = 0; i < count; i++)
            {
                int index = sArray[i].ToString().IndexOf('=');
                int length = sArray[i].ToString().Length;
                string str = sArray[i].ToString().Substring(0, index);
                string str2 = "";
                if ((index + 1) < length)
                {
                    str2 = sArray[i].ToString().Substring(index + 1);
                }
                if (((str.ToLower() == "sign") || (str.ToLower() == "sign_type")) || (str2 == ""))
                {
                    sArray.RemoveAt(i);
                    count--;
                    i--;
                }
            }
            return sArray;
        }

        private static string GetMd5(string s, string inputCharset)
        {
            byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(Encoding.GetEncoding(inputCharset).GetBytes(s));
            var builder = new StringBuilder(0x20);
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(buffer[i].ToString("x").PadLeft(2, '0'));
            }
            return builder.ToString();
        }

        private static string CreateLinkstring(ArrayList sArray)
        {
            var builder = new StringBuilder();
            foreach (object t in sArray)
            {
                builder.Append(HttpUtility.UrlDecode(t.ToString()) + "&");
            }
            return builder.ToString().TrimEnd('&');
        }

        private static string CreateLinkstringEncode(ArrayList sArray)
        {
            var builder = new StringBuilder();
            foreach (string t in sArray)
            {
                if (t != null)
                {
                    string[] strArray = t.Split('=');
                    if (strArray.Length == 2)
                    {
                        builder.Append(strArray[0] + "=" + HttpUtility.UrlEncode(strArray[1]) + "&");
                    }
                }
            }
            return builder.ToString().TrimEnd('&');
        }

        protected override void Init()
        {
            LoadPlatform(PlatformType.Alipay);
        }

        public override string LoginUrl(string callBack)
        {

            var sArray = new ArrayList
            {
                "service=user_authentication",
                "partner=" + Config.Partner,
                "return_url=" + callBack,
                "email=",
                "_input_charset=" + Config.Charset
            };
            sArray = ParaFilter(sArray);
            sArray.Sort();
            string str2 = GetMd5(CreateLinkstring(sArray) + Config.Key, Config.Charset);
            return (Config.TokenUrl + CreateLinkstringEncode(sArray) + "&sign=" + str2 + "&sign_type=" + Config.SignType);
        }

        public override UserBase Login(string callbackUrl)
        {
            NameValueCollection coll = HttpContext.Current.Request.QueryString;

            if (coll.Count > 0)
            {
                string verifyUrl = Config.TokenUrl + "service=notify_verify&partner={0}&notify_id={1}";
                var result = verifyUrl.FormatWith(Config.Partner, coll["notify_id"]).As<IHtml>().GetHtml();
                if (result != "true") return new AlipayUser { Status = false, Message = "验证失败！" };
                var pams = new[] { "sign", "sign_type", "target", "subdomain" };
                var str = coll.AllKeys.Where(key => !key.In(pams)).Aggregate("",
                    (current, key) =>
                        current + (key + "=" + coll[key] + "&"));
                str = str.TrimEnd('&') + Config.Key;
                var sign = GetMd5(str, Config.Charset);
                if (sign == coll["sign"])
                {
                    return new AlipayUser
                    {
                        Id = coll["user_id"]
                    };
                }
            }
            return new AlipayUser { Status = false, Message = "获取登录信息失败！" };
        }
    }
}
