using System.Data;

namespace Shoy.Data
{
    public class DriverTemplate<TConn, TCmd, TAdapter, TParameter, TBuilder> : IDriver
        where TConn : IDbConnection, new()
        where TCmd : IDbCommand, new()
        where TAdapter : IDbDataAdapter, new()
        where TParameter : IDbDataParameter, new()
        where TBuilder : ISqlBuilder, new()
    {
        private readonly ISqlBuilder _builder = new TBuilder();
        public IDbConnection Connection
        {
            get { return new TConn(); }
        }

        public IDbDataAdapter DataAdapter(IDbCommand cmd)
        {
            IDbDataAdapter adpt = new TAdapter { SelectCommand = cmd };
            return adpt;
        }

        public IDbCommand Command
        {
            get { return new TCmd(); }
        }

        public string ReplaceSql(string sql)
        {
            return _builder.ReplaceSql(sql);
        }

        public IDataParameter CreateProcParameter(string name, object value, ParameterDirection direction)
        {
            IDataParameter dp = new TParameter();
            _builder.SetProcParameter(dp, name, value, direction);
            return dp;
        }

        public IDataParameter CreateParameter(string name, object value, ParameterDirection direction)
        {
            IDataParameter dp = new TParameter();
            _builder.SetParameter(dp, name, value, direction);
            return dp;
        }
    }
}
