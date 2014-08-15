using System;
using System.Collections;
using System.Globalization;
using System.Linq;

namespace Shoy.Utility.Extend
{
    /// <summary>
    /// 对象扩展辅助
    /// </summary>
    public static class ObjectExtension
    {
        public static bool In(this object o, IEnumerable c)
        {
            return c.Cast<object>().Contains(o);
        }

        public static bool In<T>(this T t, params T[] c)
        {
            return c.Any(i => i.Equals(t));
        }

        public static T ObjectToT<T>(this object obj)
        {
            return obj.ObjectToT(default(T));
        }

        public static T ObjectToT<T>(this object obj, T def)
        {
            var value = obj.ConvertTo(typeof(T));
            if (value == null)
                return def;
            return (T)value;
        }

        public static object ConvertTo(this object obj, Type type)
        {
            try
            {
                if (type.Name == "Nullable`1")
                    type = type.GetGenericArguments()[0];
                if (type.IsValueType)
                    return Activator.CreateInstance(type);
                return Convert.ChangeType(obj, type, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public static void WriteTo(this Exception ex, string path)
        {
            Utils.WriteException(path, ex);
        }

        /// <summary>
        /// xml序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ObjectToXml(this object obj,string path)
        {
            return XmlHelper.XmlSerialize(path, obj);
        }

        /// <summary>
        /// Xml反序列化
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="path">xml路径</param>
        /// <returns></returns>
        public static T XmlToObject<T>(this string path)
        {
            return XmlHelper.XmlDeserialize<T>(path);
        }
    }
}