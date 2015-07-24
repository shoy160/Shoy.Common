using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Text;
using Shoy.Utility.Helper;

namespace Shoy.Utility.Extend
{
    /// <summary>
    /// 对象扩展辅助
    /// </summary>
    public static class ObjectExtension
    {

        /// <summary>
        /// 对象转换为泛型
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CastTo<T>(this object obj)
        {
            return obj.CastTo(default(T));
        }

        /// <summary>
        /// 对象转换为泛型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="def"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CastTo<T>(this object obj, T def)
        {
            var value = obj.CastTo(typeof(T));
            if (value == null)
                return def;
            return (T)value;
        }

        /// <summary>
        /// 把对象类型转换为指定类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="conversionType"></param>
        /// <returns></returns>
        public static object CastTo(this object value, Type conversionType)
        {
            if (value == null) return null;
            if (conversionType.IsNullableType())
                conversionType = conversionType.GetUnNullableType();
            if (conversionType.IsEnum)
                return Enum.Parse(conversionType, value.ToString());
            if (conversionType == typeof(Guid))
                return Guid.Parse(value.ToString());
            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 将对象[主要是匿名对象]转换为dynamic
        /// </summary>
        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            var type = value.GetType();
            var properties = TypeDescriptor.GetProperties(type);
            foreach (PropertyDescriptor property in properties)
            {
                var val = property.GetValue(value);
                if (property.PropertyType.FullName.StartsWith("<>f__AnonymousType"))
                {
                    dynamic dval = val.ToDynamic();
                    expando.Add(property.Name, dval);
                }
                else
                {
                    expando.Add(property.Name, val);
                }
            }
            return (ExpandoObject) expando;
        }

        /// <summary>
        /// 写异常信息
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="path"></param>
        public static void WriteTo(this Exception ex, string path)
        {
            FileHelper.WriteException(path, ex);
        }

        /// <summary>
        /// 异常信息格式化
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="isHideStackTrace"></param>
        /// <returns></returns>
        public static string Format(this Exception ex, bool isHideStackTrace = false)
        {
            var sb = new StringBuilder();
            var count = 0;
            var appString = string.Empty;
            while (ex != null)
            {
                if (count > 0)
                {
                    appString += "  ";
                }
                sb.AppendLine(string.Format("{0}异常消息：{1}", appString, ex.Message));
                sb.AppendLine(string.Format("{0}异常类型：{1}", appString, ex.GetType().FullName));
                sb.AppendLine(string.Format("{0}异常方法：{1}", appString,
                    (ex.TargetSite == null ? null : ex.TargetSite.Name)));
                sb.AppendLine(string.Format("{0}异常源：{1}", appString, ex.Source));
                if (!isHideStackTrace && ex.StackTrace != null)
                {
                    sb.AppendLine(string.Format("{0}异常堆栈：{1}", appString, ex.StackTrace));
                }
                if (ex.InnerException != null)
                {
                    sb.AppendLine(string.Format("{0}内部异常：", appString));
                    count++;
                }
                ex = ex.InnerException;
            }
            return sb.ToString();
        }
    }
}