using System.Data;

namespace Shoy.Data
{
    public interface IDriver
    {
        IDbConnection Connection
        {
            get;
        }
        IDbDataAdapter DataAdapter(IDbCommand cmd);
        IDbCommand Command
        {
            get;
        }
        string ReplaceSql(string sql);
        IDataParameter CreateProcParameter(string name, object value, ParameterDirection direction);
        IDataParameter CreateParameter(string name, object value, ParameterDirection direction);
    }
}
