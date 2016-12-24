using Shoy.OnlinePay.App.Factory;
using Shoy.OnlinePay.App.Utils;
using Shoy.Utility.Extend;
using Shoy.Utility.Helper;
using Shoy.Utility.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Shoy.OnlinePay.App
{
    public static class OnlinePayHelper
    {
        public static DPay Instance(PaidType type)
        {
            switch (type)
            {
                case PaidType.Alipay:
                    return new Factory.Alipay();
                case PaidType.Weixin:
                    return new Weixin();
                default:
                    return null;
            }
        }

        /// <summary> 各平台基础参数 </summary>
        /// <param name="config"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Dictionary<string, string> BaseParams(this PlatConfig config, string method = null)
        {
            if (config == null)
                return null;
            var dict = new Dictionary<string, string>();
            switch (config.Type)
            {
                case PaidType.Alipay:
                    dict.Add("app_id", config.AppId);
                    dict.Add("method", method);
                    dict.Add("format", config.Format);
                    dict.Add("charset", config.Charset);
                    dict.Add("sign_type", "RSA");
                    dict.Add("timestamp", Clock.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    dict.Add("version", "1.0");
                    dict.Add("notify_url", config.Notify);
                    break;
                case PaidType.Weixin:
                    dict.Add("appid", config.AppId);
                    dict.Add("mch_id", config.PartnerId);
                    dict.Add("nonce_str", IdHelper.Instance.Guid32);
                    dict.Add("notify_url", config.Notify);
                    dict.Add("trade_type", "APP");
                    dict.Add("spbill_create_ip", Utility.Utils.GetRealIp());
                    break;
            }
            return dict;
        }

        /// <summary> 将字典转换为form-data </summary>
        /// <param name="dict"></param>
        /// <param name="sorted"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ParamsUrl(this IDictionary<string, string> dict, bool sorted = true, bool encoding = true)
        {
            if (sorted)
            {
                dict = new SortedDictionary<string, string>(dict);
            }
            var paramList = (from item in dict
                             let value = item.Value ?? string.Empty
                             select encoding ? $"{item.Key}={value.UrlEncode()}" : $"{item.Key}={value}").ToList();
            return string.Join("&", paramList);
        }

        /// <summary> 将字典转换为xml </summary>
        /// <param name="dict"></param>
        /// <param name="sorted"></param>
        /// <returns></returns>
        public static string ParamsXml(this IDictionary<string, string> dict, bool sorted = true)
        {
            var sb = new StringBuilder();
            if (sorted)
            {
                dict = new SortedDictionary<string, string>(dict);
            }
            foreach (var item in dict)
            {
                if (item.Key == "attach" || item.Key == "body" || item.Key == "sign")
                {
                    sb.Append("<" + item.Key + "><![CDATA[" + item.Value + "]]></" + item.Key + ">");
                }
                else
                {
                    sb.Append("<" + item.Key + ">" + item.Value + "</" + item.Key + ">");
                }
            }
            return $"<xml>{sb}</xml>";
        }

        /// <summary> 获取xml格式 </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static IDictionary<string, string> XmlToDict(this string xml)
        {
            var dict = new Dictionary<string, string>();
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                var root = doc.DocumentElement;
                if (root == null)
                    return dict;
                var len = root.ChildNodes.Count;
                for (var i = 0; i < len; i++)
                {
                    var ele = root.ChildNodes[i];
                    var name = ele.Name.Trim();
                    if (dict.ContainsKey(name))
                        continue;
                    dict.Add(name, ele.InnerText.Trim());
                }
                return dict;
            }
            catch
            {
                return dict;
            }
        }

        /// <summary> Rsa签名 </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="charset"></param>
        /// <returns></returns>
        public static string RsaSign(this IDictionary<string, string> dict, string key, string charset)
        {
            if (dict.ContainsKey("sign"))
                dict.Remove("sign");
            var data = dict.ParamsUrl(true, false);
            return AlipaySignature.RsaSign(data, key, charset);
        }

        /// <summary> MD5签名 </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Md5Sign(this IDictionary<string, string> dict, string key = null)
        {
            dict.Remove("sign");
            if (!string.IsNullOrWhiteSpace(key))
                dict.Add("key", key);
            var data = dict.ParamsUrl(true, false);
            dict.Remove("key");
            return data.Md5().ToUpper();
        }

        /// <summary> 获取所有请求参数 </summary>
        /// <returns></returns>
        public static IDictionary<string, string> GetParams()
        {
            var dict = new Dictionary<string, string>();
            var context = System.Web.HttpContext.Current;
            if (context == null)
                return dict;
            var values = context.Request.QueryString;
            if (string.Equals(context.Request.HttpMethod, "post", StringComparison.CurrentCultureIgnoreCase))
                values = context.Request.Form;
            foreach (var key in values.AllKeys)
            {
                var value = values[key] ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(value) && key != "sign")
                    value = value.UrlDecode();
                dict.Add(key, value);
            }
            return dict;
        }

        public static T GetValue<T>(this IDictionary<string, string> dict, string key, T def = default(T))
        {
            if (dict == null || !dict.ContainsKey(key))
                return def;
            return dict[key].CastTo(def);
        }
    }
}
