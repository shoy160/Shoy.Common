using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using System;

namespace Shoy.Utility.Extend
{
    ///<summary>
    /// 字符串通用扩展类
    ///</summary>
    public static class CommonExtension
    {
        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 判断是否不为空
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 字符串格式化
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="arg0">参数0</param>
        /// <returns>格式化后的字符串</returns>
        public static string FormatWith(this string str, object arg0)
        {
            return string.Format(str, arg0);
        }

        /// <summary>
        /// 字符串格式化
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="arg0">参数0</param>
        /// <param name="arg1">参数1</param>
        /// <returns>格式化后的字符串</returns>
        public static string FormatWith(this string str, object arg0,object arg1)
        {
            return string.Format(str, arg0, arg1);
        }

        /// <summary>
        /// 字符串格式化
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="arg0">参数0</param>
        /// <param name="arg1">参数1</param>
        /// <param name="arg2">参数2</param>
        /// <returns>格式化后的字符串</returns>
        public static string FormatWith(this string str, object arg0, object arg1, object arg2)
        {
            return string.Format(str, arg0, arg1, arg2);
        }

        /// <summary>
        /// 字符串格式化
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args">参数集</param>
        /// <returns></returns>
        public static string FormatWith(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        /// <summary>
        /// 倒置字符串，输入"abcd123"，返回"321dcba"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Reverse(this string str)
        {
            char[] input = str.ToCharArray();
            var output = new char[str.Length];
            for (int i = 0; i < input.Length; i++)
                output[input.Length - 1 - i] = input[i];
            return new string(output);
        }

        /// <summary>
        /// 截断字符扩展
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="start">起始位置</param>
        /// <param name="len">长度</param>
        /// <param name="v">省略符</param>
        /// <returns></returns>
        public static string Sub(this string str, int start, int len, string v)
        {
            //(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
            var reg = "[\u4e00-\u9fa5]".As<IRegex>().ToRegex(RegexOptions.Compiled);
            var chars = str.ToCharArray();
            var result = string.Empty;
            int index = 0;
            foreach (char t in chars)
            {
                if (index >= start && index < (start + len))
                    result += t;
                else if (index >= (start + len))
                {
                    result += v;
                    break;
                }
                index += (reg.IsMatch(t.ToString(CultureInfo.InvariantCulture)) ? 2 : 1);
            }
            return result;
        }

        /// <summary>
        /// 截断字符扩展
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static string Sub(this string str, int len, string v)
        {
            return str.Sub(0, len, v);
        }

        /// <summary>
        /// 截断字符扩展
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Sub(this string str, int len)
        {
            return str.Sub(0, len, "...");
        }

        /// <summary>
        /// 对传递的参数字符串进行处理，防止注入式攻击
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertSql(this string str)
        {
            str = str.Trim();
            str = str.Replace("'", "''");
            str = str.Replace(";--", "");
            str = str.Replace("=", "");
            str = str.Replace(" or ", "");
            str = str.Replace(" and ", "");
            return str;
        }

        /// <summary>
        /// json字符串转换为obj
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(this string json)
        {
            var serializer = new JavaScriptSerializer();
            var section =
                ConfigurationManager.GetSection("system.web.extensions/scripting/webServices/jsonSerialization")
                as ScriptingJsonSerializationSection;
            if (section != null)
            {
                serializer.MaxJsonLength = section.MaxJsonLength;
                serializer.RecursionLimit = section.RecursionLimit;
            }
            return serializer.Deserialize<T>(json);
        }

        /// <summary>
        /// 将obj转换为json字符
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            var serializer = new JavaScriptSerializer();
            var section =
                ConfigurationManager.GetSection("system.web.extensions/scripting/webServices/jsonSerialization")
                as ScriptingJsonSerializationSection;
            if (section != null)
            {
                serializer.MaxJsonLength = section.MaxJsonLength;
                serializer.RecursionLimit = section.RecursionLimit;
            }
            return serializer.Serialize(obj);
        }

        /// <summary>
        /// Html编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string str)
        {
            return HttpUtility.HtmlEncode(str);
        }

        /// <summary>
        /// Html解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string str)
        {
            return HttpUtility.HtmlDecode(str);
        }

        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string UrlEncode(this string str,Encoding encoding)
        {
            return HttpUtility.UrlEncode(str, encoding);
        }

        /// <summary>
        /// Url编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(this string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string UrlDecode(this string str,Encoding encoding)
        {
            return HttpUtility.UrlDecode(str, encoding);
        }

        /// <summary>
        /// Url解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlDecode(this string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        /// 获取该字符串的QueryString值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="str">字符串</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static T Query<T>(this string str, T def)
        {
            try
            {
                var c = HttpContext.Current;
                var qs = c.Request.QueryString[str].Trim();
                return qs.CastTo(def);
            }
            catch
            {
                return def;
            }
        }

        /// <summary>
        /// 获取该字符串的Form值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="str">字符串</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static T Form<T>(this string str, T def)
        {
            try
            {
                var c = HttpContext.Current;
                var qs = c.Request.Form[str].Trim();
                return qs.CastTo(def);
            }
            catch
            {
                return def;
            }
        }

        /// <summary>
        /// 获取该字符串QueryString或Form值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="str"></param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static T QueryOrForm<T>(this string str, T def)
        {
            try
            {
                var c = HttpContext.Current;
                var qs = c.Request[str].Trim();
                return qs.CastTo(def);
            }
            catch
            {
                return def;
            }
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="url">url</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static string SetQuery(this string key, string url, object value)
        {
            if ( key.IsNullOrEmpty())
                return url;
            if (value == null)
                value = "";
            if (url.IsNullOrEmpty())
            {
                url = "http://" + HttpContext.Current.Request.ServerVariables["HTTP_HOST"] +
                      HttpContext.Current.Request.RawUrl;
            }
            var qs = url.Split('?');
            var list = new System.Collections.Specialized.NameValueCollection();
            if (qs.Length < 2)
            {
                list.Add(key, UrlEncode(value.ToString()));
            }
            else
            {
                foreach (var query in qs[1].Split('&'))
                {
                    var item = query.Split('=');
                    if (item.Length == 2)
                        list.Add(item[0], item[1]);
                }
                list[key] = UrlEncode(value.ToString());
            }
            var search = string.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                search += list.AllKeys[i] + "=" + list[i];
                if (i < list.Count - 1)
                    search += "&";
            }
            return qs[0] + "?" + search;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static string SetQuery(this string key, object value)
        {
            return key.SetQuery(string.Empty, value);
        }

        /// <summary>
        /// 将字符串写入到文件
        /// </summary>
        /// <param name="msg">字符串</param>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码</param>
        public static void WriteTo(this string msg, string path, Encoding encoding)
        {
            Utils.WriteFile(path, msg, encoding);
        }

        /// <summary>
        /// 获取该值的MD5
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Md5(this string str)
        {
            if (str.IsNullOrEmpty())
                return str;
            return SecurityCls.Md5(str);
        }
		
		/// <summary>
		/// 字符串转换为指定类型
		/// </summary>
		/// <param name="str"></param>
		/// <param name="def"></param>
		/// <param name="splitor"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T To<T>(this string str, T def = default(T), string splitor = ",")
        {
            var type = typeof (T);

            if (type.IsArray || type.Name == "List`1")
            {
                try
                {
                    Type st = typeof (string);
                    bool isList = false;
                    if (type.IsArray)
                        st = Type.GetType(type.FullName.TrimEnd('[', ']'));
                    else if (type.Name == "List`1")
                    {
                        isList = true;
                        var reg = Regex.Match(type.FullName, "System.Collections.Generic.List`1\\[\\[([^,]+),");
                        st = Type.GetType(reg.Groups[1].Value);
                    }
                    var arr = str.Split(new[] {splitor}, StringSplitOptions.RemoveEmptyEntries);
                    if (st != typeof (string) && st != null)
                    {
                        if (st == typeof (int))
                        {
                            var rt = Array.ConvertAll(arr, s => s.CastTo(0));
                            return (isList ? (T) (object) rt.ToList() : (T) (object) rt);
                        }
                        if (st == typeof (double))
                        {
                            var rt = Array.ConvertAll(arr, s => s.CastTo(0.0));
                            return (isList ? (T) (object) rt.ToList() : (T) (object) rt);
                        }
                        if (st == typeof (decimal))
                        {
                            var rt = Array.ConvertAll(arr, s => s.CastTo(0M));
                            return (isList ? (T) (object) rt.ToList() : (T) (object) rt);
                        }
                        if (st == typeof (float))
                        {
                            var rt = Array.ConvertAll(arr, s => s.CastTo(0F));
                            return (isList ? (T) (object) rt.ToList() : (T) (object) rt);
                        }
                        if (st == typeof (DateTime))
                        {
                            var rt = Array.ConvertAll(arr, s => s.CastTo(DateTime.MinValue));
                            return (isList ? (T) (object) rt.ToList() : (T) (object) rt);
                        }
                    }
                    return (isList ? (T) (object) arr.ToList() : (T) (object) arr);
                }
                catch
                {
                    return def;
                }
            }
            return str.CastTo(def);
        }
    }
}
