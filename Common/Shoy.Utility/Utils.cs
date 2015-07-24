using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web;
using Shoy.Utility.Extend;
using Shoy.Utility.Helper;
using Shoy.Utility.Logging;

namespace Shoy.Utility
{
    /// <summary>
    /// 常用类 create by shoy
    /// </summary>
    public class Utils
    {
        private const string DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        private static readonly ILogger Logger = LogManager.Logger<Utils>();
        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            strPath = strPath.Replace("/", "\\");
            if (strPath.StartsWith("\\"))
            {
                strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
        }

        /// <summary>
        /// 获取当前时间yyyy-MM-dd HH:mm:ss
        /// </summary>
        /// <returns></returns>
        public static string GetTimeNow()
        {
            return GetTimeNow(DefaultDateTimeFormat);
        }

        public static string GetTimeNow(string format)
        {
            try
            {
                return DateTime.Now.ToString(format);
            }
            catch
            {
                return DateTime.Now.ToString(DefaultDateTimeFormat);
            }
        }

        private static Mutex _mut;



        //2012-08-16
        /// <summary>
        /// 获取绝对地址
        /// </summary>
        /// <param name="host">网站根地址</param>
        /// <param name="url">当前地址</param>
        /// <returns></returns>
        public static string GetAbsoluteUrl(string host, string url)
        {
            if (!host.StartsWith("http://") && !host.StartsWith("https://"))
                host = "http://" + host;
            var t = new Uri(new Uri(host), url);
            return t.AbsoluteUri;
        }

        public static bool IsOutUrl(string url, string[] domains)
        {
            if (domains.Length == 0) return true;
            var host = "www" + domains[0];
            if (!host.StartsWith("http://") && !host.StartsWith("https://"))
                host = "http://" + host;
            var t = new Uri(new Uri(host), url);
            var domain = RegexHelper.GetDomain(t.Host);
            return !domains.Contains(domain);
        }

        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding"></param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str, Encoding encoding)
        {
            return HttpUtility.UrlEncode(str, encoding);
        }

        public static string UrlEncode(string str)
        {
            return UrlEncode(str, Encoding.Default);
        }

        public static string UrlDecode(string str, Encoding encoding)
        {
            return HttpUtility.UrlDecode(str, encoding);
        }

        public static string UrlDecode(string str)
        {
            return UrlDecode(str, Encoding.Default);
        }

        /// <summary>
        /// PageSize型
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pageQ"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static string GetNextPageUrl(string url, string pageQ, int pageSize)
        {
            if (string.IsNullOrEmpty(url))
                return url;
            var pageStr = RegexHelper.Match(url, "[?&]" + pageQ + "=(\\d+)");
            int page = 1;
            if (!string.IsNullOrEmpty(pageStr))
            {
                page = ConvertHelper.StrToInt(pageStr, 0);
                url = url.Replace(RegexHelper.Match(url, "([?&]" + pageQ + "=\\d+)"), string.Empty);
            }
            page += pageSize;
            return url.IndexOf('?') >= 0 ? (url + "&" + pageQ + "=" + page) : (url + "?" + pageQ + "=" + page);
        }

        /// <summary>
        /// Page型,从1开始
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pageQ"></param>
        /// <param name="startOne"></param>
        /// <returns></returns>
        public static string GetNextPageUrl(string url, string pageQ, bool startOne)
        {
            if (string.IsNullOrEmpty(url))
                return url;
            var pageStr = RegexHelper.Match(url, "[?&]" + pageQ + "=(\\d+)");
            int page = ConvertHelper.StrToInt(pageStr, (startOne ? 1 : 0));
            if (!string.IsNullOrEmpty(pageStr))
            {
                url = url.Replace(RegexHelper.Match(url, "([?&]" + pageQ + "=\\d+)"), string.Empty);
            }
            page++;
            return url.IndexOf('?') >= 0 ? (url + "&" + pageQ + "=" + page) : (url + "?" + pageQ + "=" + page);
        }

        /// <summary>
        /// Page型简化(默认参数page)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetNextPageUrl(string url)
        {
            return GetNextPageUrl(url, "page", true);
        }

        public static string GetJsonTime()
        {
            return RegexHelper.Match(DateTime.Now.ToJson(), "(\\d+)");
        }

        /// <summary>
        /// 获取流的字符信息
        /// </summary>
        /// <returns></returns>
        public static string GetTxtFromStream(Stream stream, Encoding encoding)
        {
            if (stream == null)
                return string.Empty;
            _mut = (_mut ?? new Mutex());
            _mut.WaitOne();
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(stream, encoding);
                return sr.ReadToEnd();
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
        }

        /// <summary>
        /// 获取流的字符信息
        /// </summary>
        /// <returns></returns>
        public static List<string> GetListFromStream(Stream stream, Encoding encoding)
        {
            var list = new List<string>();
            if (stream == null)
                return list;
            _mut = (_mut ?? new Mutex());
            _mut.WaitOne();
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(stream, encoding);
                var str = sr.ReadToEnd();
                if (!string.IsNullOrEmpty(str))
                {
                    list = str.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                return list;
            }
            catch
            {
                return list;
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static string GetHtmlFromFile(string path, Encoding encoding)
        {
            string html;
            if (!File.Exists(path))
                return string.Empty;
            StreamReader sr = null;
            _mut = (_mut ?? new Mutex());
            _mut.WaitOne();
            try
            {
                sr = new StreamReader(path, encoding);
                html = sr.ReadToEnd();
            }
            catch
            {
                html = string.Empty;
            }
            finally
            {
                if (sr != null)
                    sr.Close();
                _mut.ReleaseMutex();
            }
            return html;
        }

        /// <summary>
        /// 获取txt文本文件数据
        /// </summary>
        /// <param name="path">txt文件路径</param>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public static IEnumerable<string> GetListFromFile(string path, Encoding code)
        {
            var list = new List<string>();
            var txt = GetHtmlFromFile(path, code);
            if (!string.IsNullOrEmpty(txt))
            {
                list = txt.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            return list;
        }

        /// <summary>
        /// 获取文件内容(简化)
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static IEnumerable<string> GetListFromFile(string path)
        {
            return GetListFromFile(path, Encoding.Default);
        }

        ///<summary>
        /// 判断List值相等
        ///</summary>
        ///<param name="l1"></param>
        ///<param name="l2"></param>
        ///<typeparam name="T"></typeparam>
        ///<returns></returns>
        public static bool ListEquals<T>(IEnumerable<T> l1, List<T> l2)
        {
            if (l1 == l2) return true;
            if (l1 == null || l2 == null || l1.Count() != l2.Count()) return false;
            return !l1.Where((t, i) => !t.Equals(l2[i])).Any();
        }

        public static string GetCurrentDir()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            // 或者 AppDomain.CurrentDomain.SetupInformation.ApplicationBase
            return dir;
        }



        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int connectionDescription, int reservedValue);

        /// <summary>
        /// 是否有网络链接
        /// </summary>
        public static bool IsNetConnected
        {
            get
            {
                int i;
                return InternetGetConnectedState(out i, 0);
            }
        }

        public static string GetIp()
        {
            //var adapters = NetworkInterface.GetAllNetworkInterfaces();
            //foreach (NetworkInterface adapter in adapters)
            //{
            //    var addresses = adapter.GetIPProperties().UnicastAddresses;
            //    if (addresses != null && addresses.Any())
            //    {
            //        var address = addresses.FirstOrDefault(t => t.Address.AddressFamily == AddressFamily.InterNetwork);
            //        if (address != null)
            //            return address.ToString();
            //    }
            //}
            //return "127.0.0.1";
            var ips = Dns.GetHostEntry(Dns.GetHostName());
            if (ips != null && ips.AddressList.Any())
            {
                foreach (IPAddress address in ips.AddressList)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                        return address.ToString();
                }
            }
            return "127.0.0.1";
        }

        /// <summary>
        /// 获取真实IP
        /// </summary>
        /// <returns></returns>
        public static string GetRealIp()
        {
            return GetRealIp(HttpContext.Current);
        }

        /// <summary>
        /// 获取真是IP
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetRealIp(HttpContext context)
        {
            if (context == null) return "127.0.0.1";
            string userHostAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = context.Request.UserHostAddress;
            }
            if (!(!string.IsNullOrEmpty(userHostAddress) && RegexHelper.IsIp(userHostAddress)))
            {
                return "127.0.0.1";
            }
            return userHostAddress;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static string SetQuery(string url, string key, object value)
        {
            if (key.IsNullOrEmpty())
                return url;
            if (value == null)
                value = string.Empty;
            if (url.IsNullOrEmpty())
                url = "http://" + HttpContext.Current.Request.ServerVariables["HTTP_HOST"] +
                      HttpContext.Current.Request.RawUrl;
            var qs = url.Split('?');
            string search;
            var list = new System.Collections.Specialized.NameValueCollection();
            if (qs.Length < 2)
            {
                list.Add(key, UrlEncode(value.ToString()));
            }
            else
            {
                search = qs[1];
                foreach (var query in search.Split('&'))
                {
                    var item = query.Split('=');
                    list.Add(item[0], item[1]);
                }
                list[key] = UrlEncode(value.ToString());
            }
            search = string.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                search += list.AllKeys[i] + "=" + list[i];
                if (i < list.Count - 1)
                    search += "&";
            }
            return qs[0] + "?" + search;
        }

        /// <summary>
        /// 获得拼音缩写
        /// </summary>
        /// <param name="cnStr"></param>
        /// <returns></returns>
        public static string GetSpellCode(string cnStr)
        {
            var strTemp = new StringBuilder();
            int iLen = cnStr.Length;
            int i;

            for (i = 0; i <= iLen - 1; i++)
            {
                strTemp.Append(GetShortSpell(cnStr.Substring(i, 1)));
            }

            return strTemp.ToString();
        }

        /// <summary> 
        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母 
        /// </summary> 
        /// <param name="cnChar">单个汉字</param> 
        /// <returns>单个大写字母</returns> 
        private static string GetShortSpell(string cnChar)
        {
            byte[] arrCn = Encoding.Default.GetBytes(cnChar);
            if (arrCn.Length > 1)
            {
                int area = arrCn[0];
                int pos = arrCn[1];
                int code = (area << 8) + pos;
                int[] areacode =
                    {
                        45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614,
                        48119, 48119, 49062, 49324, 49896, 50371, 50614, 50622,
                        50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481
                    };
                for (int i = 0; i < 26; i++)
                {
                    int max = 55290;
                    if (i != 25)
                        max = areacode[i + 1];
                    if (areacode[i] <= code && code < max)
                        return Encoding.Default.GetString(new[] { (byte)(65 + i) });
                }
                return "*";
            }
            return cnChar;
        }

        public static readonly Func<int, IEnumerable<int>> EachMax = delegate(int max)
        {
            max = Math.Abs(max);
            return Enumerable.Range(0, max);
        };

        public static readonly Func<int, int, IEnumerable<int>> Each = delegate(int min, int max)
        {
            min = Math.Min(min, max);
            return Enumerable.Range(min, Math.Abs(max - min));
        };

        /// <summary> 配置文件读取 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parseFunc">类型转换方法</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="key">配置名</param>
        /// <param name="supressKey">配置别名</param>
        /// <returns></returns>
        public static T GetAppSetting<T>(Func<string, T> parseFunc = null, T defaultValue = default(T),
            [CallerMemberName] string key = null, string supressKey = null)
        {
            return ConfigHelper.GetAppSetting(parseFunc, defaultValue, key, supressKey);
        }
    }
}
