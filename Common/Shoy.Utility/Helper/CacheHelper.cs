using Shoy.Utility.Extend;
using Shoy.Utility.Logging;
using System;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Shoy.Utility.Helper
{
    /// <summary>
    /// 缓存辅助类
    /// </summary>
    public class CacheHelper
    {
        private static readonly ILogger Logger = LogManager.Logger<CacheHelper>();
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存 键</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="minutes">有效时间(分钟)</param>
        /// <returns></returns>
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

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存 键</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="path">缓存依赖文件路径</param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存 键</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            if (HttpContext.Current == null || string.IsNullOrEmpty(key)) return default(T);
            var obj = HttpContext.Current.Cache[key];
            try
            {
                return obj.CastTo<T>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
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

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        private static void Delete(string key)
        {
            if (HttpContext.Current == null) return;
            HttpContext.Current.Cache.Remove(key);
        }

        /// <summary>
        /// 显示所有缓存
        /// </summary>
        /// <returns></returns>
        public static string Show()
        {
            var sb = new StringBuilder();
            IDictionaryEnumerator cacheEnum = HttpRuntime.Cache.GetEnumerator();

            sb.AppendLine(string.Format("当前网站总缓存数:{0}", HttpRuntime.Cache.Count));
            while (cacheEnum.MoveNext())
            {
                sb.AppendLine(string.Format("缓存名:[{0}]", cacheEnum.Key));
            }
            return sb.ToString();
        }
    }
}
