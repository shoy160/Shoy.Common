using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace Shoy.Utility
{
    public class CacheCls
    {
        public static bool Add(string key, object obj, int minutes)
        {
            if (string.IsNullOrEmpty(key) || minutes <= 0) return false;
            if (Exists(key))
                Delete(key);
            HttpContext.Current.Cache.Insert(
                key,
                obj,
                null,
                DateTime.Now.AddMinutes(minutes),
                Cache.NoSlidingExpiration);
            return true;
        }

        public static bool Add(string key, object obj, string path)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(path)) return false;
            if (HttpContext.Current == null) return false;
            if (Exists(key))
                Delete(key);
            HttpContext.Current.Cache.Insert(
                key,
                obj,
                new CacheDependency(path),
                Cache.NoAbsoluteExpiration,
                Cache.NoSlidingExpiration);
            return true;
        }

        public static T Get<T>(string key)
        {
            if (HttpContext.Current == null || string.IsNullOrEmpty(key)) return default(T);
            var obj = HttpContext.Current.Cache[key];
            try
            {
                return (T) obj;
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary> 
        /// 判断Cache是否存在 
        /// </summary> 
        /// <param name="key"></param> 
        /// <returns></returns> 
        private static bool Exists(string key)
        {
            var c = HttpContext.Current;
            if (c == null) return false;
            return c.Cache[key] != null;
        }

        private static void Delete(string key)
        {
            if (HttpContext.Current == null) return;
            HttpContext.Current.Cache.Remove(key);
        }

        ////显示所有缓存 
        public static string Show()
        {
            string str = "";
            IDictionaryEnumerator cacheEnum = HttpRuntime.Cache.GetEnumerator();

            while (cacheEnum.MoveNext())
            {
                str += "<br />缓存名<b>[" + cacheEnum.Key + "]</b><br />";
            }
            return "当前网站总缓存数:" + HttpRuntime.Cache.Count + "<br />" + str;
        }
    }
}
