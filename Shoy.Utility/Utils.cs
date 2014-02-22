using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using Shoy.Utility.Extend;
using System.Reflection;

namespace Shoy.Utility
{
    /// <summary>
    /// 常用类 create by shy
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// string转换为float
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns></returns>
        public static float StrToFloat(string strValue, float defValue)
        {
            if ((strValue == null) || (strValue.Length > 10))
                return defValue;

            float intValue = defValue;
            bool isFloat = Regex.IsMatch(strValue, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
            if (isFloat)
                float.TryParse(strValue, out intValue);
            return intValue;
        }

        /// <summary>
        /// object转化为float
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static float StrToFloat(object obj, float defValue)
        {
            if (obj == null)
                return defValue;
            return StrToFloat(obj.ToString(), defValue);
        }

        /// <summary>
        /// string转化为int
        /// </summary>
        /// <param name="str">字符</param>
        /// <param name="defValue">默认值</param>
        /// <returns></returns>
        public static int StrToInt(string str, int defValue)
        {
            if (string.IsNullOrEmpty(str) || str.Trim().Length >= 11 ||
                !Regex.IsMatch(str.Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
                return defValue;

            int rv;
            if (Int32.TryParse(str, out rv))
                return rv;

            return Convert.ToInt32(StrToFloat(str, defValue));
        }

        /// <summary>
        /// object转化为int
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static int StrToInt(object obj, int defValue)
        {
            if (obj == null)
                return defValue;
            return StrToInt(obj.ToString(), defValue);
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(string str, DateTime defValue)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime dateTime;
                if (DateTime.TryParse(str, out dateTime))
                    return dateTime;
            }
            return defValue;
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public static DateTime StrToDateTime(object obj, DateTime defValue)
        {
            if (obj == null) return defValue;
            return StrToDateTime(obj.ToString(), defValue);
        }

        /// <summary>
        /// 将对象转换为日期时间类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <returns>转换后的int类型结果</returns>
        public static DateTime StrToDateTime(string str)
        {
            return StrToDateTime(str, DateTime.Now);
        }

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
            return GetTimeNow("yyyy-MM-dd HH:mm:ss");
        }

        public static string GetTimeNow(string format)
        {
            try
            {
                return DateTime.Now.ToString(format);
            }
            catch
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 获取新的文件名
        /// </summary>
        /// <param name="ext">扩展名</param>
        /// <returns></returns>
        public static string GetNewName(string ext)
        {
            string newName = (Guid.NewGuid().ToString()).Replace("-", "").ToLower();
            return newName + ext;
        }

        private static Mutex _mut;

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="str">字符</param>
        /// <param name="append">是否追加</param>
        /// <param name="code">编码</param>
        public static void WriteFile(string path, IEnumerable<string> str, bool append, Encoding code)
        {
            if (_mut == null)
                _mut = new Mutex();

            _mut.WaitOne();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(path, append, code);
                foreach (string s in str)
                {
                    sw.WriteLine(s);
                    sw.Flush();
                }
            }
            finally
            {
                if (sw != null)
                    sw.Close();
                _mut.ReleaseMutex();
            }
        }

        public static void WriteFile(string path, IEnumerable<string> str, Encoding code)
        {
            WriteFile(path, str, true, code);
        }

        public static void WriteFile(string path, IEnumerable<string> str, bool append)
        {
            WriteFile(path, str, append, Encoding.Default);
        }

        public static void WriteFile(string path, IEnumerable<string> str)
        {
            WriteFile(path, str, true, Encoding.Default);
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="str">写入值</param>
        /// <param name="append">是否追加</param>
        /// <param name="code">编码</param>
        public static void WriteFile(string path, string str, bool append, Encoding code)
        {
            var list = new[] { str };
            WriteFile(path, list, append, code);
        }

        /// <summary>
        /// 写文件(简化)
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="str">写入值</param>
        /// <param name="append">是否追加</param>
        public static void WriteFile(string path, string str, bool append)
        {
            WriteFile(path, str, append, Encoding.Default);
        }

        /// <summary>
        /// 写文件(简化),默认追加
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="str">写入值</param>
        /// <param name="code">编码</param>
        public static void WriteFile(string path, string str, Encoding code)
        {
            WriteFile(path, str, true, code);
        }

        /// <summary>
        /// 写文件(简化),默认追加
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="str">写入值</param>
        public static void WriteFile(string path, string str)
        {
            WriteFile(path, str, true, Encoding.Default);
        }

        /// <summary>
        /// 写异常日志
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ex"></param>
        public static void WriteException(string path, Exception ex)
        {
            string msg = GetTimeNow() + Environment.NewLine
                         + ex.Message + Environment.NewLine
                         + ex.Source + Environment.NewLine
                         + ex.StackTrace + Environment.NewLine
                         + ex.TargetSite.Name;
            WriteFile(path, msg, true);
        }

        public static void WriteException(Exception ex)
        {
            var path = GetCurrentDir() + "/exception.log";
            WriteException(path, ex);
        }

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
            var domain = GetDomain(t.Host);
            return !domains.Contains(domain);
        }

        /// <summary>
        /// 获取单个正则匹配的字符
        /// </summary>
        /// <param name="regex">正则</param>
        /// <param name="str">字符串</param>
        /// <param name="group">组</param>
        /// <param name="ops">表达式选项</param>
        /// <returns></returns>
        public static string GetRegStr(string str, string regex, int group, RegexOptions ops)
        {
            var reg = new Regex(regex, ops);
            var m = reg.Match(str);
            return m.Groups[group].Value;
        }

        /// <summary>
        /// (简化)获取正则匹配的字符
        /// </summary>
        /// <param name="regex">正则</param>
        /// <param name="str">字符串</param>
        /// <param name="group">组</param>
        /// <returns></returns>
        public static string GetRegStr(string str, string regex, int group)
        {
            return GetRegStr(str, regex, group, RegexOptions.None);
        }

        /// <summary>
        /// (简化)获取正则匹配的字符
        /// </summary>
        /// <param name="regex">正则</param>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string GetRegStr(string str, string regex)
        {
            return GetRegStr(str, regex, 1, RegexOptions.None);
        }

        public static List<string> GetRegHtmls(string docHtml, string regStr, int index)
        {
            var mts = (new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.Singleline)).Matches(docHtml);
            return (from Match mt in mts select mt.Groups[index].Value).ToList();
        }

        public static List<string> GetRegHtmls(string docHtml, string regStr, string name)
        {
            var mts = (new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.Singleline)).Matches(docHtml);
            return (from Match mt in mts select mt.Groups[name].Value).ToList();
        }

        /// <summary>
        /// 获取多个正则匹配值
        /// </summary>
        /// <param name="docHtml">字符源</param>
        /// <param name="regStr">正则</param>
        /// <returns></returns>
        public static List<string> GetRegHtmls(string docHtml, string regStr)
        {
            return GetRegHtmls(docHtml, regStr, 1);
        }

        /// <summary>
        /// 清除\r \n \t
        /// </summary>
        /// <param name="str">str</param>
        /// <returns></returns>
        public static string ClearTrn(string str)
        {
            return string.IsNullOrEmpty(str)
                       ? str
                       : str.Replace("\r", "").Replace("\n", "").Replace("\t", "");
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
        /// 清除给定字符串中的回车及换行符
        /// </summary>
        /// <param name="str">要清除的字符串</param>
        /// <returns>清除后返回的字符串</returns>
        public static string ClearBr(string str)
        {
            Match m;
            var regexBr = new Regex(@"(\r\n)", RegexOptions.IgnoreCase);
            for (m = regexBr.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), "");
            }
            return str;
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
            var pageStr = GetRegStr(url, "[?&]" + pageQ + "=(\\d+)");
            int page = 1;
            if (!string.IsNullOrEmpty(pageStr))
            {
                page = StrToInt(pageStr, 0);
                url = url.Replace(GetRegStr(url, "([?&]" + pageQ + "=\\d+)"), "");
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
            var pageStr = GetRegStr(url, "[?&]" + pageQ + "=(\\d+)");
            int page = StrToInt(pageStr, (startOne ? 1 : 0));
            if (!string.IsNullOrEmpty(pageStr))
            {
                url = url.Replace(GetRegStr(url, "([?&]" + pageQ + "=\\d+)"), "");
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
            return GetRegStr(DateTime.Now.ToJson(), "(\\d+)");
        }

        public static Random GetRandom()
        {
            var bytes = new byte[4];
            var rng =
                new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            int seed = BitConverter.ToInt32(bytes, 0);
            long tick = DateTime.Now.Ticks + (seed);
            return new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
        }

        /// <summary>
        /// 获取流的字符信息
        /// </summary>
        /// <returns></returns>
        public static string GetTxtFromStream(Stream stream, Encoding encoding)
        {
            if (stream == null)
                return "";
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
                return "";
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
                if(!string.IsNullOrEmpty(str))
                {
                    list = str.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries).ToList();
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
                return "";
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
                html = "";
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
                list = txt.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries).ToList();
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

        /// <summary>
        /// 相同属性不同类转换
        /// </summary>
        /// <typeparam name="T">转换目标类</typeparam>
        /// <param name="o">待转换类</param>
        /// <returns></returns>
        public static T ClassConvert<T>(object o)
            where T : new()
        {
            var t = new T();
            Type type = o.GetType();
            PropertyInfo[] ps = type.GetProperties();
            foreach (PropertyInfo p in ps)
            {
                object pv = p.GetValue(o, null);
                Type type2 = t.GetType();

                PropertyInfo pr = type2.GetProperty(p.Name);
                if (pr != null)
                {
                    pr.SetValue(t, pv, null);
                }
            }
            return t;
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

        /// <summary>
        /// 合并文件，合并到文件1
        /// </summary>
        /// <param name="path1">文件1</param>
        /// <param name="path2">文件2</param>
        public static void AddFile(string path1, string path2)
        {
            StreamReader sr = null;
            StreamWriter sw = null;
            try
            {
                sr = new StreamReader(path2, Encoding.Default);
                sw = new StreamWriter(path1, true, Encoding.Default);
                string str = sr.ReadLine();
                while (!string.IsNullOrEmpty(str))
                {
                    sw.WriteLine(str);
                    str = sr.ReadLine();
                }
            }
            finally
            {
                if (sr != null)
                    sr.Close();
                if (sw != null)
                    sw.Close();
            }
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        /// <param name="delOld"></param>
        /// <returns></returns>
        public static bool MoveFile(string oldPath, string newPath, bool delOld)
        {
            if (!File.Exists(oldPath))
                return false;
            StreamReader sr = null;
            StreamWriter sw = null;
            try
            {
                sr = new StreamReader(oldPath);
                sw = new StreamWriter(newPath, false);
                sw.Write(sr.ReadToEnd());
            }
            catch
            {
                return false;
            }
            finally
            {
                if (sr != null)
                    sr.Close();
                if (sw != null)
                    sw.Close();
            }
            if (delOld)
                File.Delete(oldPath);
            return true;
        }

        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="oldPath">旧文件夹路径</param>
        /// <param name="newPath">新文件夹</param>
        public static bool CopyDirectory(string oldPath, string newPath)
        {
            try
            {
                if (!Directory.Exists(oldPath))
                    return false;

                if (newPath[newPath.Length - 1] != Path.DirectorySeparatorChar)
                    newPath += Path.DirectorySeparatorChar;

                if (!Directory.Exists(newPath))
                    Directory.CreateDirectory(newPath);

                var files = Directory.GetFileSystemEntries(oldPath);

                foreach (string file in files)
                {
                    var thePath = newPath + Path.GetFileName(file);
                    if (Directory.Exists(file))
                    {
                        //递归Copy该目录下面的文件
                        CopyDirectory(file, thePath);
                    }
                    else // 否则直接copy文件
                    {
                        File.Copy(file, thePath, true);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查路径文件是否存在，若不存在则创建
        /// </summary>
        /// <param name="path"></param>
        /// <param name="create"></param>
        public static bool CheckDirectory(string path,bool create)
        {
            var dir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(dir))
                return false;
            if (!Directory.Exists(dir))
            {
                if (create)
                    Directory.CreateDirectory(dir);
                return false;
            }
            return true;
        }

        public static string GetCurrentDir()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            // 或者 AppDomain.CurrentDomain.SetupInformation.ApplicationBase
            return dir;
        }

        public static string GetDomain(string hostName)
        {
            var reg = new Regex(@".(\w+).(com.cn|net.cn|org.cn|edu.cn|com|net|org|cn|biz|info|cc|tv)");
            if (hostName == null)
                hostName = String.Empty;
            return reg.Match(hostName).Value;
        }

        /// <summary>
        /// 获取指定长度的随机字符串
        /// </summary>
        /// <param name="len">长度</param>
        /// <param name="hasHardChar">是否排除难辨别字符</param>
        /// <returns></returns>
        public static string GetRanStr(int len, bool hasHardChar)
        {
            string str = "", restr = "";
            const string allstr = "qwNOPerWXYktyu421ioKfdsS867plVjMZ9hgDEnbTUxcABGHIJaCFmL0vzQR53";
            const string hard = "0oOz29q1ilI6b";

            str = (hasHardChar ? allstr : new string(allstr.ToArray().Except(hard.ToArray()).ToArray()));
            var ran = GetRandom();
            for (int i = 0; i < len; i++)
            {
                restr += str[ran.Next(0, str.Length - 1)];
            }
            return restr;
        }

        /// <summary>
        /// (简化)获取指定长度的随机字符串
        /// </summary>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string GetRanStr(int len)
        {
            return GetRanStr(len, true);
        }

        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
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
                value = "";
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
            search = "";
            for (int i = 0; i < list.Count; i++)
            {
                search += list.AllKeys[i] + "=" + list[i];
                if (i < list.Count - 1)
                    search += "&";
            }
            return qs[0] + "?" + search;
        }

        /// <summary>
        /// 获取数字中文
        /// 不完善
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string GetBigNumber(int num)
        {
            var word = new[] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            var dw = new[] { "十", "百", "千", "万", "亿" };
            var desc = "";
            var str = num.ToString().Reverse();
            //1000,000)
            for (var i = 0; i < str.Length; i++)
            {
                var n = (Utils.StrToInt(str[i].ToString(), 0));
                if (n > 0 || ((i - 4) % 4 == 0))
                {
                    if ((i - 1) % 4 == 0)
                        desc = dw[0] + desc;
                    else if ((i - 2) % 4 == 0)
                        desc = dw[1] + desc;
                    else if ((i - 3) % 4 == 0)
                        desc = dw[2] + desc;
                    else if (i > 3 && (i - 4) % 8 == 0)
                        desc = dw[3] + desc;
                    else if (i > 7 && i % 8 == 0)
                        desc = dw[4] + desc;
                }
                if (i != 0 || n != 0)
                {
                    if (!desc.StartsWith(word[0]) && (n != 0 || ((i - 4) % 4 != 0)))
                        desc = word[n] + desc;
                }
            }
            return desc;
        }

        ///<summary>
        /// 判断是否是url
        ///</summary>
        ///<param name="strUrl"></param>
        ///<returns></returns>
        public static bool IsUrl(string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }

        public static string ClearHtml(string str)
        {
            return Regex.Replace(str, "</?[0-9a-zA-Z]+[^>]*/?>", "");
        }
    }
}

