using System;
using System.Data;
using System.Globalization;

namespace Shoy.Data.Core
{
    public static class Ext
    {
        /// <summary>
        /// 是否包含指定列
        /// </summary>
        /// <param name="reader">DataReader</param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        public static bool Contains(this IDataReader reader, string columnName)
        {
            int count = reader.FieldCount;
            for (int i = 0; i < count; i++)
            {
                if (reader.GetName(i).Equals(columnName,StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static T ObjectToT<T>(this object obj)
        {
            return obj.ObjectToT(default(T));
        }

        public static T ObjectToT<T>(this object obj, T def)
        {
            var value = obj.ConvertTo(typeof (T));
            if (value == null)
                return def;
            return (T) value;
        }

        public static object ConvertTo(this object obj, Type type)
        {
            try
            {
                if (type.Name == "Nullable`1")
                    type = type.GetGenericArguments()[0];
                if (obj.Equals(DBNull.Value))
                {
                    if (type.IsValueType)
                        return Activator.CreateInstance(type);
                    return null;
                }
                return Convert.ChangeType(obj, type, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }
    }
}
