using DayEasy.God.Services.OnlinePay.Factory;
using DayEasy.Utility;
using DayEasy.Utility.Extend;
using DayEasy.Utility.Helper;
using DayEasy.Utility.Timing;
using ServiceStack.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DayEasy.God.Services.OnlinePay
{
    public static class OnlinePayHelper
    {
        public static DPay Instance(PaidType type)
        {
            switch (type)
            {
                case PaidType.Alipay:
                    return new Alipay();
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
        public static Dictionary<string, object> BaseParams(this PlatConfig config, string method = null)
        {
            if (config == null)
                return null;
            var dict = new Dictionary<string, object>();
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
                    dict.Add("mch_id", "1230000109");
                    dict.Add("nonce_str", IdHelper.Instance.Guid32);
                    dict.Add("notify_url", config.Notify);
                    dict.Add("trade_type", "APP");
                    dict.Add("spbill_create_ip", Utils.GetRealIp());
                    break;
            }
            return dict;
        }

        /// <summary> 将字典转换为form-data </summary>
        /// <param name="dict"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ParamsUrl(this IDictionary<string, object> dict, bool encoding = true)
        {
            var paramList = new List<string>();
            foreach (var item in dict)
            {
                var value = item.Value?.ToString() ?? string.Empty;

                paramList.Add(encoding ? $"{item.Key}={StringExtensions.UrlEncode(value)}" : $"{item.Key}={value}");
            }
            return string.Join("&", paramList);
        }
        /// <summary> 将字典转换为xml </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ParamsXml(this IDictionary<string, object> dict)
        {
            var sb = new StringBuilder();
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
        public static string RsaSign(this IDictionary<string, object> dict, string key, string charset)
        {
            var orderDict = dict.Where(t => t.Key != "sign").OrderBy(t => t.Key).ToDictionary(k => k.Key, v => v.Value);
            var data = orderDict.ParamsUrl(false);
            return AlipaySignature.RsaSign(data, key, charset);
        }

        /// <summary> MD5签名 </summary>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Md5Sign(this IDictionary<string, object> dict, string key = null)
        {
            var orderDict = dict.Where(t => t.Key != "sign").OrderBy(t => t.Key).ToDictionary(k => k.Key, v => v.Value);
            if (!string.IsNullOrWhiteSpace(key))
                orderDict.Add("key", key);
            var data = orderDict.ParamsUrl(false);
            return data.Md5().ToUpper();
        }
    }
}
