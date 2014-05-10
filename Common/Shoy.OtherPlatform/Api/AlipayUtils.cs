using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Shoy.OtherPlatform.Entity;
using Shoy.Utility.Extend;

namespace Shoy.OtherPlatform.Api
{
    public class AlipayUtils
    {
        private static string _partnerId = "";
        private static string _key = "";
        private const string Charset = "utf-8";
        private const string SignType = "MD5";

        private const string Gateway = "https://mapi.alipay.com/gateway.do?";

        internal static string CreateUrl(string callBack)
        {
            if(_partnerId.IsNullOrEmpty() || _key.IsNullOrEmpty())
            {
                var info = PlatformUtility.GetKeyInfos(PlatformType.Alipay);
                _partnerId = info.AppId;
                _key = info.Key;
            }
            var sArray = new ArrayList
                             {
                                 "service=user_authentication",
                                 "partner=" + _partnerId,
                                 "return_url=" + callBack,
                                 "email=",
                                 "_input_charset="+Charset
                             };
            sArray = ParaFilter(sArray);
            sArray.Sort();
            string str2 = GetMd5(CreateLinkstring(sArray) + _key, Charset);
            return (Gateway + CreateLinkstringEncode(sArray) + "&sign=" + str2 + "&sign_type=" + SignType);
        }

        internal static UserInfo GetUserInfo(HttpContext context, string callBack)
        {
            NameValueCollection coll = context.Request.QueryString;

            if (coll.Count > 0)
            {
                const string verifyUrl = Gateway + "service=notify_verify&partner={0}&notify_id={1}";
                var result = verifyUrl.FormatWith(_partnerId, coll["notify_id"]).As<IHtml>().GetHtml();
                if (result != "true") return new UserInfo {State = "-1", Msg = "验证失败！"};
                var pams = new[] { "sign", "sign_type", "target", "subdomain" };
                var str = coll.AllKeys.Where(key => !key.In(pams)).Aggregate("",
                                                                             (current, key) =>
                                                                             current + (key + "=" + coll[key] + "&"));
                str = str.TrimEnd('&') + _key;
                var sign = GetMd5(str, Charset);
                if (sign == coll["sign"])
                {
                    return new UserInfo
                               {
                                   Uid = coll["user_id"],
                                   NickName = "",
                                   Gender = "",
                                   ImgSrc = "",
                                   Msg = "",
                               };
                }
            }
            return new UserInfo { State = "-1", Msg = "获取登录信息失败！" }; ;
        }

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
    }
}
