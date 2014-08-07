using System;
using System.Data;

namespace Shoy.Data
{
    public class PostgreBuilder:ISqlBuilder
    {
        public string ReplaceSql(string sql)
        {
            return sql.Replace("@", ":").Replace("?", ":").Replace("[", "\"").Replace("]", "\"");
        }

        public void SetProcParameter(IDataParameter dp, string name, object value, ParameterDirection direction)
        {
            dp.ParameterName = string.Concat(":", name); ;
            dp.Value = value ?? DBNull.Value;
            dp.Direction = direction;
        }

        public void SetParameter(IDataParameter dp, string name, object value, ParameterDirection direction)
        {
            dp.ParameterName = string.Concat(":", name);
            dp.Value = value ?? DBNull.Value;
            dp.Direction = direction;
        }
    }
}
