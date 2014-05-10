using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Web;
using System.Xml.Linq;
using System.Web.Caching;
using System.Text.RegularExpressions;

namespace Shoy.OnlinePay.Common
{
    public static class Utils
    {
        //获取远程服务器ATN结果，验证是否是支付宝服务器发来的请求
        public static string GetHttp(string aStrUrl, int timeout)
        {
            string strResult;
            try
            {
                var myReq = (HttpWebRequest)WebRequest.Create(aStrUrl);
                myReq.Timeout = timeout;
                var httpWResp = (HttpWebResponse)myReq.GetResponse();
                Stream myStream = httpWResp.GetResponseStream();
                var sr = new StreamReader(myStream, Encoding.Default);
                var strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }

                strResult = strBuilder.ToString();
            }
            catch (Exception exp)
            {

                strResult = "错误：" + exp.Message;
            }

            return strResult;
        }

        public static DateTime StrToDate(string str,DateTime def)
        {
            DateTime date;
            str = System.Text.RegularExpressions.Regex.Replace(str, "^(\\d{4})(\\d{2})(\\d{2})(\\d{2})(\\d{2})(\\d{2})$",
                                                               "$1-$2-$3 $4:$5:$6");
            if (!DateTime.TryParse(str, out date))
                date = def;
            return date;
        }

        public static int ToInt(object obj,int def)
        {
            if (obj == null) return def;
            int val;
            if (!int.TryParse(obj.ToString(), out val))
                val = def;
            return val;
        }

        internal static PartnerInfo GetPartnerInfos(PayType type)
        {
            const string key = "partnerInfo";
            var cache = HttpRuntime.Cache;
            if (cache[key] == null)
            {
                var path = HttpContext.Current.Server.MapPath("~/App_Data/xml/ShoyApp.config");
                cache.Insert(key, GetPartnerInfos(), new CacheDependency(path));
            }
            var rlist = cache[key] as List<PartnerInfo>;
            PartnerInfo item = null;
            if (rlist != null)
                item = rlist.FirstOrDefault(t => t.Type == type);
            return item;
        }

        private static List<PartnerInfo> GetPartnerInfos()
        {
            var plist = new List<PartnerInfo>();
            var path = HttpContext.Current.Server.MapPath("~/App_Data/xml/ShoyApp.config");
            if (File.Exists(path))
            {
                var xd = XDocument.Load(path);
                var list = xd.Descendants("onlinePay").Elements("item");
                if (list != null)
                {
                    foreach (var ele in list)
                    {
                        XAttribute pt = ele.Attribute("pid"),
                                   kt = ele.Attribute("key"),
                                   tp = ele.Attribute("type"),
                                   se = ele.Attribute("semail");
                        plist.Add(new PartnerInfo
                                      {
                                          PayType = ToInt(tp == null ? "" : tp.Value, 0),
                                          PartnerId = (pt == null ? "" : pt.Value),
                                          Key = (kt == null ? "" : kt.Value),
                                          SellerEmail = (se == null ? "" : se.Value)
                                      });
                    }
                }
            }
            return plist;
        }

        internal static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        internal static string GetRealIp()
        {
            var context = HttpContext.Current;
            string userHostAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = context.Request.UserHostAddress;
            }
            if (!(!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress)))
            {
                return "127.0.0.1";
            }
            return userHostAddress;
        }

        public static Dictionary<string,string> BuildParas(string paras)
        {
            var dict = new Dictionary<string, string>();
            foreach (var para in paras.Split('&'))
            {
                var item = para.Split('=');
                dict.Add(item[0], item[1]);
            }
            return dict;
        }

        public static string CreateLinkString(Dictionary<string, string> paras)
        {
            if (paras.Count() == 0) return "";
            var urlParas = paras.Aggregate("", (current, para) => current + (para.Key + "=" + para.Value + "&"));
            return urlParas.TrimEnd('&');
        }

        public static string CreateLinkString(Dictionary<string, string> paras,Encoding encoding)
        {
            if (paras.Count() == 0) return "";
            var urlParas = paras.Aggregate("",
                                           (current, para) =>
                                           current +
                                           (para.Key + "=" + HttpUtility.UrlEncode(para.Value, encoding) + "&"));
            return urlParas.TrimEnd('&');
        }

        /// <summary>
        /// 建立请求，以表单HTML形式构造（默认）
        /// </summary>
        /// <param name="gateWay">支付宝网关地址</param>
        /// <param name="sParaTemp">请求参数数组</param>
        /// <returns>提交表单HTML文本</returns>
        public static string BuildRequest(string gateWay, Dictionary<string, string> sParaTemp)
        {
            var sbHtml = new StringBuilder();

            sbHtml.Append("<form id='alipaysubmit' name='alipaysubmit' action='" + gateWay +
                          "_input_charset=utf-8' method='get'>");

            foreach (KeyValuePair<string, string> temp in sParaTemp)
            {
                sbHtml.Append("<input type='hidden' name='" + temp.Key + "' value='" + temp.Value + "'/>");
            }

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type='submit' value='确认' style='display:none;'></form>");

            sbHtml.Append("<script>document.forms['alipaysubmit'].submit();</script>");

            return sbHtml.ToString();
        }

        /// <summary>
        /// 除去数组中的空值和签名参数
        /// </summary>
        /// <param name="dicArrayPre">过滤前的参数组</param>
        /// <returns>过滤后的参数组</returns>
        public static Dictionary<string, string> FilterPara(Dictionary<string, string> dicArrayPre)
        {
            return dicArrayPre.Where(temp => temp.Key.ToLower() != "sign" && temp.Key.ToLower() != "sign_type" && !string.IsNullOrEmpty(temp.Value)).ToDictionary(temp => temp.Key, temp => temp.Value);
        }

        /// <summary>
        /// 根据字母a到z的顺序把参数排序
        /// </summary>
        /// <param name="dicArrayPre">排序前的参数组</param>
        /// <returns>排序后的参数组</returns>
        public static Dictionary<string, string> SortPara(Dictionary<string, string> dicArrayPre)
        {
            var dicTemp = new SortedDictionary<string, string>(dicArrayPre);
            var dicArray = new Dictionary<string, string>(dicTemp);

            return dicArray;
        }

        public static Dictionary<string,string> BuildParas(Dictionary<string,string> paras,string key,string charset)
        {
            paras = FilterPara(paras);
            paras = SortPara(paras);
            var sign = Digest.GetMd5(CreateLinkString(paras) + key, charset);
            paras.Add("sign", sign);
            return paras;
        }

        public static Dictionary<string, string> BuildNotifyParas(Dictionary<string, string> paras, string key, string charset)
        {
            paras = FilterPara(paras);
            //paras = SortPara(paras);
            var sPara = new Dictionary<string, string>
                            {
                                {"service", paras["service"]},
                                {"v", paras["v"]},
                                {"sec_id", paras["sec_id"]},
                                {"notify_data", paras["notify_data"]}
                            };
            var sign = Digest.GetMd5(CreateLinkString(sPara) + key, charset);
            paras.Add("sign", sign);
            return paras;
        }

        public static string GetRequestToken(string token,Encoding code)
        {
            var dict = BuildParas(token);
            if(dict.Keys.Contains("res_data"))
            {
                var xml = new Function.XmlDoc(HttpUtility.UrlDecode(dict["res_data"], code));
                return xml.GetNode("/direct_trade_create_res/request_token");
            }
            return "";
        }
    }
}
