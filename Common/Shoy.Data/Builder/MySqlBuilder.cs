using System;
using System.Data;

namespace Shoy.Data.Builder
{
    public class MySqlBuilder:ISqlBuilder
    {
        public string ReplaceSql(string sql)
        {
            return sql.Replace("[", "`").Replace("]", "`");
        }

        public void SetProcParameter(IDataParameter dp, string name, object value, ParameterDirection direction)
        {
            dp.ParameterName = string.Concat("@", name);
            dp.Value = value ?? DBNull.Value;
            dp.Direction = direction;
        }

        public void SetParameter(IDataParameter dp, string name, object value, ParameterDirection direction)
        {
            dp.ParameterName = string.Concat("@", name);
            dp.Value = value ?? DBNull.Value;
            dp.Direction = direction;
        }
    }
}
