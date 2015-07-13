using System;
using System.Configuration;
using System.Runtime.CompilerServices;
using Shoy.Utility.Logging;

namespace Shoy.Utility.Helper
{
    public class ConfigHelper
    {
        private static readonly ILogger Logger = LogManager.Logger<ConfigHelper>();
        /// <summary>
        /// 得到AppSettings中的配置字符串信息
        /// </summary>
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
        public static T GetAppSetting<T>(Func<string, T> parseFunc = null, T defaultValue = default(T), [CallerMemberName] string key = null,
              string supressKey = null)
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
                Logger.Error(ex.Message, ex);
                return defaultValue;
            }
        }
    }
}
