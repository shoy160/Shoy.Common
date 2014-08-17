using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Shoy.Data
{
    /// <summary>
    /// 数据连接上下文对象基于线程存储
    /// </summary>
    public interface IConnectionContext : IDbTransaction, IDisposable
    {
        void BeginTransaction(IsolationLevel level);
        void BeginTransaction();
        int ExecuteNonQuery(Command cmd);
        IDataReader ExecuteReader(Command cmd);
        object ExecuteScalar(Command cmd);
        DataSet ExecuteDataSet(Command cmd);
        DataTable ExecuteDataTable(Command cmd);
        IList<T> List<T>(Command cmd, Region region) where T : new();

        object ExecProc(Command cmd);
        IList<T> ListProc<T>(Command cmd) where T : new();
        IList ListProc(Type entity, Command cmd);

        T ListFirst<T>(Command cmd) where T : new();

        T Load<T>(Command cmd) where T : new();

        IDictionary<T, TV> Dict<T, TV>(Command cmd);

        IList List(Type type, Command cmd, Region region);
        object ListFirst(Type type, Command cmd);
        object Load(Type type, Command cmd);
        T GetValue<T>(Command cmd);
        IList<T> GetValues<T>(Command cmd);
        IList<T> GetValues<T>(Command cmd, Region region);
    }
}
