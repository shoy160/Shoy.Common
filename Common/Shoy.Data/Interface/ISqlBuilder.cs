using System.Data;

namespace Shoy.Data
{
    /// <summary>
    /// SQL转换器
    /// </summary>
    public interface ISqlBuilder
    {
        string ReplaceSql(string sql);

        void SetProcParameter(IDataParameter dp, string name, object value, ParameterDirection direction);


        void SetParameter(IDataParameter dp, string name, object value, ParameterDirection direction);
    }
}