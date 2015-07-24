using System;
using System.Collections.Specialized;
using System.Web;
using Shoy.Utility.Extend;

namespace Shoy.Utility.Helper
{
    /// <summary>
    /// cookie操作类 Edit by shy 2012-12-19
    /// </summary>
    public class CookieHelper
    {
        /// <summary> 
        /// 创建或修改COOKIE对象并赋Value值 
        /// </summary> 
        /// <param name="strCookieName">COOKIE对象名</param> 
        /// <param name="iExpires"> 
        /// COOKIE对象有效时间（秒数）
        /// 0表示会话cookie，负数表示删除 
        /// </param> 
        /// <param name="strDomain">作用域</param> 
        /// <param name="strValue">COOKIE对象Value值</param> 
        /// <remarks> 
        /// 对COOKIE修改必须重新设Expires 
        /// </remarks> 
        public static void Set(string strCookieName, string strValue, int iExpires, string strDomain)
        {
            var objCookie = new HttpCookie(strCookieName.Trim())
                                {
                                    Value = HttpContext.Current.Server.UrlEncode(strValue.Trim()),
                                };
            if (!string.IsNullOrEmpty(strDomain))
                objCookie.Domain = strDomain.Trim();
            if (iExpires > 0)
            {
                objCookie.Expires = GetExpries(iExpires);
            }
            if (iExpires < 0)
            {
                objCookie.Expires = DateTime.Now.AddDays(-235);
            }
            HttpContext.Current.Response.AppendCookie(objCookie);
        }

        /// <summary> 
        /// 创建或修改COOKIE对象并赋Value值 
        /// </summary> 
        /// <param name="strCookieName">COOKIE对象名</param> 
        /// <param name="iExpires">COOKIE对象有效时间（秒数）</param> 
        /// <param name="strValue">COOKIE对象Value值</param> 
        /// <remarks> 
        /// 对COOKIE修改必须重新设Expires 
        /// </remarks> 
        public static void Set(string strCookieName, string strValue, int iExpires)
        {
            Set(strCookieName, strValue, iExpires, null);
        }

        /// <summary> 
        /// 创建或修改COOKIE对象并赋Value值 
        /// </summary> 
        /// <param name="strCookieName">COOKIE对象名</param> 
        /// <param name="strValue">COOKIE对象Value值</param> 
        /// <remarks> 
        /// 对COOKIE修改必须重新设Expires 
        /// </remarks> 
        public static void Set(string strCookieName, string strValue)
        {
            Set(strCookieName, strValue, 0, null);
        }

        /// <summary> 
        /// 创建COOKIE对象并赋多个KEY键值 
        /// </summary> 
        /// <param name="strCookieName">COOKIE对象名</param> 
        /// <param name="iExpires"> COOKIE对象有效时间（秒数）</param> 
        /// <param name="strDomain">作用域</param> 
        /// <param name="keyValue">键/值对集合</param> 
        public static void Set(string strCookieName, NameValueCollection keyValue, int iExpires, string strDomain)
        {
            var objCookie = new HttpCookie(strCookieName.Trim());
            foreach (string key in keyValue.AllKeys)
            {
                objCookie[key] = HttpContext.Current.Server.UrlEncode(keyValue[key].Trim());
            }
            if (!string.IsNullOrEmpty(strDomain))
                objCookie.Domain = strDomain.Trim();
            if (iExpires > 0)
            {
                objCookie.Expires = GetExpries(iExpires);
            }
            HttpContext.Current.Response.AppendCookie(objCookie);
        }

        /// <summary> 
        /// 创建COOKIE对象并赋多个KEY键值 
        /// </summary> 
        /// <param name="strCookieName">COOKIE对象名</param> 
        /// <param name="iExpires"> COOKIE对象有效时间（秒数）</param> 
        /// <param name="keyValue">键/值对集合</param> 
        public static void Set(string strCookieName, NameValueCollection keyValue, int iExpires)
        {
            Set(strCookieName, keyValue, iExpires, null);
        }

        /// <summary> 
        /// 读取Cookie某个对象的某个Key键的键值 
        /// </summary> 
        /// <param name="strCookieName">Cookie对象名称</param> 
        /// <param name="strKeyName">Key键名</param> 
        /// <returns>Key键值，如果对象或键值就不存在，则返回 null</returns> 
        public static string GetValue(string strCookieName, string strKeyName)
        {
            var cookie = HttpContext.Current.Request.Cookies[strCookieName];
            if (cookie == null)
            {
                return string.Empty;
            }
            string strObjValue = cookie.Value;
            if (!string.IsNullOrEmpty(strKeyName))
            {
                string strKeyName2 = strKeyName + "=";
                if (strObjValue.IndexOf(strKeyName2, StringComparison.Ordinal) == -1)
                {
                    return string.Empty;
                }
                strObjValue = cookie[strKeyName];
            }
            return HttpContext.Current.Server.UrlDecode(strObjValue);
        }

        /// <summary> 
        /// 读取Cookie某个对象的Value值，返回Value值 
        /// </summary> 
        /// <param name="strCookieName">Cookie对象名称</param> 
        /// <returns>Value值，如果对象本就不存在，则返回 null</returns> 
        public static string GetValue(string strCookieName)
        {
            return GetValue(strCookieName, null);
        }

        /// <summary>
        /// 获取Cookie值
        /// </summary>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public static NameValueCollection GetValues(string cookieName)
        {
            var cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie == null)
            {
                return null;
            }

            var values = new NameValueCollection();
            foreach (var key in cookie.Values.AllKeys)
            {
                var value = cookie[key];
                values.Add(key, value.UrlDecode());
            }
            return values;
        }

        /// <summary> 
        /// 修改某个COOKIE对象某个Key键的键值或给某个COOKIE对象添加Key键 
        /// </summary> 
        /// <param name="strCookieName">Cookie对象名称</param> 
        /// <param name="strKeyName">Key键名</param> 
        /// <param name="keyValue">Key键值</param> 
        /// <param name="iExpires"> 
        /// COOKIE对象有效时间（秒数） 
        /// 1表示永久有效，0和负数都表示不设有效时间 
        /// 大于等于2表示具体有效秒数 
        /// 86400秒 = 1天 = （60*60*24） 
        /// 604800秒 = 1周 = （60*60*24*7） 
        /// 2593000秒 = 1月 = （60*60*24*30） 
        /// 31536000秒 = 1年 = （60*60*24*365） 
        /// </param> 
        /// <returns>如果对象本就不存在，则返回 false</returns> 
        public static bool Edit(string strCookieName, string strKeyName, string keyValue, int iExpires)
        {
            var cookie = HttpContext.Current.Request.Cookies[strCookieName];
            if (cookie == null)
            {
                return false;
            }
            cookie[strKeyName] = HttpContext.Current.Server.UrlEncode(keyValue.Trim());
            if (iExpires > 0)
            {
                cookie.Expires = GetExpries(iExpires);
            }
            HttpContext.Current.Response.AppendCookie(cookie);
            return true;
        }

        /// <summary> 
        /// 删除COOKIE对象 
        /// </summary> 
        /// <param name="strCookieName">Cookie对象名称</param>
        /// <param name="domain"></param> 
        public static void Delete(string strCookieName, string domain)
        {
            Set(strCookieName, string.Empty, -1, domain);
        }

        /// <summary>
        /// 删除Cookie
        /// </summary>
        /// <param name="strCookieName"></param>
        public static void Delete(string strCookieName)
        {
            Delete(strCookieName, string.Empty);
        }

        /// <summary> 
        /// 删除某个COOKIE对象某个Key子键 
        /// </summary> 
        /// <param name="strCookieName">Cookie对象名称</param> 
        /// <param name="strKeyName">Key键名</param> 
        /// <param name="iExpires"> 
        /// COOKIE对象有效时间（秒数） 
        /// 1表示永久有效，0和负数都表示不设有效时间 
        /// 大于等于2表示具体有效秒数 
        /// 86400秒 = 1天 = （60*60*24） 
        /// 604800秒 = 1周 = （60*60*24*7） 
        /// 2593000秒 = 1月 = （60*60*24*30） 
        /// 31536000秒 = 1年 = （60*60*24*365） 
        /// </param>
        /// <param name="domain"></param>
        /// <returns>如果对象本就不存在，则返回 false</returns> 
        public static bool Delete(string strCookieName, string strKeyName, int iExpires,string domain)
        {
            var cookie = HttpContext.Current.Request.Cookies[strCookieName];
            if (cookie == null)
            {
                return false;
            }
            cookie.Values.Remove(strKeyName);
            if (iExpires > 0)
            {
                cookie.Expires = GetExpries(iExpires);
            }
            if (!string.IsNullOrEmpty(domain))
                cookie.Domain = domain.Trim();
            HttpContext.Current.Response.AppendCookie(cookie);
            return true;
        }

        /// <summary>
        /// 删除Cookie
        /// </summary>
        /// <param name="strCookieName"></param>
        /// <param name="strKeyName"></param>
        /// <param name="iExpires"></param>
        /// <returns></returns>
        public static bool Delete(string strCookieName, string strKeyName, int iExpires)
        {
            return Delete(strCookieName, strKeyName, iExpires, string.Empty);
        }

        private static DateTime GetExpries(int expires)
        {
            return (expires == 1 ? DateTime.MaxValue : DateTime.Now.AddSeconds(expires));
        }

        /// <summary>
        /// 获取多少小时
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static int GetHour(int hours)
        {
            return 60*60*hours;
        }

        /// <summary>
        /// 获取多少天
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static int GetDay(int day)
        {
            return 60*60*24*day;
        }

        /// <summary>
        /// 获取多少月
        /// </summary>
        /// <param name="months"></param>
        /// <returns></returns>
        public static int GetMonths(int months)
        {
            return 60*60*24*30*months;
        }
    }
}
