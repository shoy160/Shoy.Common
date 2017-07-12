using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Shoy.Wpf.Helper
{
    /// <summary> 配置文件辅助类 </summary>
    public static class ConfigHelper
    {
        private static XDocument _xmlDoc;
        private static CacheHelper _cache;

        static ConfigHelper()
        {
            _cache = new CacheHelper("ConfigHelper");
        }

        public static void InitConfig(string path)
        {
            _xmlDoc = XDocument.Load(path);
        }

        private static T Read<T>(XElement ele)
        {
            if (ele == null)
                return default(T);
            var type = typeof(T);
            var model = Activator.CreateInstance<T>();
            foreach (var attr in ele.Attributes())
            {
                var prop = type.GetProperty(attr.Name.LocalName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase);
                if (prop == null)
                    continue;
                prop.SetValue(model, Convert.ChangeType(attr.Value, prop.PropertyType), null);
            }
            return model;
        }

        public static T Read<T>(string eleName)
        {
            var model = _cache.Get<T>(eleName);
            if (model != null)
                return model;
            if (_xmlDoc == null)
                return default(T);
            var ele = _xmlDoc.Descendants().FirstOrDefault(t => t.Name == eleName);
            model = Read<T>(ele);
            _cache.Set(eleName, model);
            return model;
        }

        public static List<T> ReadList<T>(string eleName)
        {
            var list = _cache.Get<List<T>>(eleName);
            if (list != null)
                return list;
            if (_xmlDoc == null) return new List<T>();
            var ele = _xmlDoc.Descendants().FirstOrDefault(t => t.Name == eleName);
            if (ele == null)
                return new List<T>();
            list = new List<T>();
            var nodes = ele.Elements();
            foreach (var node in nodes)
            {
                var item = Read<T>(node);
                list.Add(item);
            }
            _cache.Set(eleName, list);
            return list;
        }

        public static void Dispose()
        {
            _cache.Clear();
            _cache = null;
            _xmlDoc = null;
        }

        /// <summary> 得到AppSettings中的配置字符串信息 </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigString(string key)
        {
            return GetAppSetting<string>(null, supressKey: key);
        }

        /// <summary> 配置文件读取 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parseFunc">类型转换方法</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="key">配置名</param>
        /// <param name="supressKey">配置别名</param>
        /// <returns></returns>
        public static T GetAppSetting<T>(Func<string, T> parseFunc = null, T defaultValue = default(T), string key = null, string supressKey = null)
        {
            if (!string.IsNullOrWhiteSpace(supressKey))
                key = supressKey;
            if (parseFunc == null)
                parseFunc = s => (T)Convert.ChangeType(s, typeof(T));
            try
            {
                var node = ConfigurationManager.AppSettings[key];
                return string.IsNullOrWhiteSpace(node) ? defaultValue : parseFunc(node);
            }
            catch (Exception ex)
            {
                LogManager.Logger("config").Error(ex.Message, ex);
                return defaultValue;
            }
        }
    }
}
