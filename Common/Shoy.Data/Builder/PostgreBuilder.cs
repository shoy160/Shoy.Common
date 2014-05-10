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
            dp.ParameterName = ":" + name;
            if (value == null)
                dp.Value = DBNull.Value;
            else
                dp.Value = value;
            dp.Direction = direction;
        }

        public void SetParameter(IDataParameter dp, string name, object value, ParameterDirection direction)
        {
            dp.ParameterName = ":" + name;
            if (value == null)
                dp.Value = DBNull.Value;
            else
                dp.Value = value;
            dp.Direction = direction;
        }
    }
}
