using System;
using System.Data;

namespace Shoy.Data
{
    public class MsAccessBuilder:ISqlBuilder
    {
        #region ISqlBuilder 成员

        public string ReplaceSql(string sql)
        {
            return sql;
        }

        public void SetProcParameter(IDataParameter dp, string name, object value, ParameterDirection direction)
        {
            dp.ParameterName = "@" + name;
            dp.Value = value ?? DBNull.Value;
            dp.Direction = direction;
        }

        public void SetParameter(IDataParameter dp, string name, object value, ParameterDirection direction)
        {
            dp.ParameterName = "@" + name;
            dp.Value = value ?? DBNull.Value;
            dp.Direction = direction;
        }

        #endregion
    }
}
