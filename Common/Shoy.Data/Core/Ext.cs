using System;
using System.Data;

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
    }
}
