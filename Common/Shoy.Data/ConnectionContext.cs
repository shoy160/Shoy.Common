using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Shoy.Data.Core;

namespace Shoy.Data
{
    public class ConnectionContext : IConnectionContext
    {
        private string _connectionString;
        private IDriver _driver;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private IsolationLevel _level = IsolationLevel.Unspecified;

        public ConnectionContext(string connectionString, IDriver driver)
        {
            _connectionString = connectionString;
            _driver = driver;
            _connection = _driver.Connection;
            _connection.ConnectionString = connectionString;
            _connection.Open();
        }

        public ConnectionContext GetInstance()
        {
            return new ConnectionContext(_connectionString, _driver);
        }

        private IDbCommand InitCommand(Command cmd)
        {
            var command = cmd.CreateCommand(_driver);
            command.Connection = _connection;
            if (_transaction != null)
                command.Transaction = _transaction;
            return command;
        }

        #region IConnectionContext 成员

        public void BeginTransaction(IsolationLevel level)
        {
            if (_transaction == null)
            {
                _level = level;
                _transaction = _connection.BeginTransaction(level);
            }
        }

        public void BeginTransaction()
        {
            if (_transaction == null)
                _transaction = _connection.BeginTransaction();
        }

        public int ExecuteNonQuery(Command cmd)
        {
            return InitCommand(cmd).ExecuteNonQuery();
        }

        public IDataReader ExecuteReader(Command cmd)
        {
            return InitCommand(cmd).ExecuteReader();
        }

        public object ExecuteScalar(Command cmd)
        {
            return InitCommand(cmd).ExecuteScalar();
        }

        public DataSet ExecuteDataSet(Command cmd)
        {
            var command = InitCommand(cmd);
            IDataAdapter apt = _driver.DataAdapter(command);
            var ds = new DataSet();
            apt.Fill(ds);
            return ds;
        }

        public DataTable ExecuteDataTable(Command cmd)
        {
            return ExecuteDataSet(cmd).Tables[0];
        }

        public IList<T> List<T>(Command cmd) where T : new()
        {
            return List<T>(cmd, null);
        }

        public IList<T> List<T>(Command cmd, Region region) where T : new()
        {
            return (IList<T>) List(typeof (T), cmd, region);
        }

        public object ExecProc(Command cmd)
        {
            var command = InitCommand(cmd);
            command.CommandType = CommandType.StoredProcedure;
            return command.ExecuteScalar();
        }

        public IList<T> ListProc<T>(Command cmd) where T : new()
        {
            return (IList<T>) ListProc(typeof (T), cmd);
        }

        public IList ListProc(Type entity, Command cmd)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            return List(entity, cmd, null);
        }

        public T ListFirst<T>(Command cmd) where T : new()
        {
            var item = ListFirst(typeof (T), cmd);
            if (item == null)
                return default(T);
            return (T) item;
        }

        public T Load<T>(Command cmd) where T : new()
        {
            var item = Load(typeof (T), cmd);
            return (T) item;
        }

        public IDictionary<T, TV> Dict<T, TV>(Command cmd)
        {
            return Dict<T, TV>(cmd, null);
        }

        public IDictionary<T, TV> Dict<T, TV>(Command cmd, Region region)
        {
            if (region == null)
                region = new Region(0, 9999999);
            var dict = new Dictionary<T, TV>();
            using (IDataReader reader = ExecuteReader(cmd))
            {
                int index = 0;
                while (reader.Read())
                {
                    if (reader.FieldCount < 2)
                        continue;
                    if (index >= region.Start && dict.Count < region.Size)
                    {
                        var key = reader[0];
                        var value = reader[1];
                        if (key.Equals(DBNull.Value))
                            continue;
                        key = key.ConvertTo(typeof (T));
                        if (dict.ContainsKey((T) key))
                            continue;
                        value = value.ConvertTo(typeof (TV));
                        dict.Add((T) key, (TV) value);
                        if (dict.Count == region.Size)
                        {
                            cmd.DbCommand.Cancel();
                            reader.Close();
                            break;
                        }
                    }
                    index++;
                }
            }
            return dict;
        }

        public IList List(Type type, Command cmd, Region region)
        {
            if (region == null)
                region = new Region(0, 9999999);
            Type itemstype = Type.GetType("System.Collections.Generic.List`1");
            if (itemstype == null) return null;
            itemstype = itemstype.MakeGenericType(type);
            var items = (IList) Activator.CreateInstance(itemstype, region.Size);
            object item;
            using (IDataReader reader = ExecuteReader(cmd))
            {
                int index = 0;
                while (reader.Read())
                {
                    if (index >= region.Start && items.Count < region.Size)
                    {
                        item = Activator.CreateInstance(type);
                        foreach (
                            var info in
                                type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
                            )
                        {
                            if (!reader.Contains(info.Name))
                                continue;
                            var value = reader[info.Name];
                            var t = info.PropertyType;
                            value = value.ConvertTo(t);
                            info.SetValue(item, value, null);
                        }
                        items.Add(item);
                        if (items.Count == region.Size)
                        {
                            cmd.DbCommand.Cancel();
                            reader.Close();
                            break;
                        }
                    }
                    index++;
                }
            }
            return items;
        }

        public object ListFirst(Type type, Command cmd)
        {
            IList items = List(type, cmd, new Region(0, 2));
            if (items.Count > 0)
                return items[0];
            return null;
        }

        public object Load(Type type, Command cmd)
        {
            IList items = List(type, cmd, new Region(0, 2));
            if (items.Count > 0)
                return items[0];
            return null;
        }

        public T GetValue<T>(Command cmd)
        {
            IList<T> items = GetValues<T>(cmd, new Region(0, 2));
            if (items.Count == 0)
                return default(T);
            return items[0];
        }

        public IList<T> GetValues<T>(Command cmd)
        {
            return GetValues<T>(cmd, null);
        }

        public IList<T> GetValues<T>(Command cmd, Region region)
        {
            List<T> result = null;
            object value;
            if (region == null)
            {
                region = new Region(0, 9999999);
            }
            result = new List<T>(region.Size);
            using (IDataReader reader = ExecuteReader(cmd))
            {
                int index = 0;
                while (reader.Read())
                {
                    if (index >= region.Start)
                    {
                        value = reader[0];
                        if (!value.Equals(DBNull.Value))
                        {
                            result.Add((T) Convert.ChangeType(value, typeof (T)));
                        }
                        else
                        {
                            result.Add(default(T));
                        }
                        if (result.Count == region.Size)
                        {
                            cmd.DbCommand.Cancel();
                            reader.Close();
                            break;
                        }
                    }
                    index++;
                }
            }
            return result;
        }

        #endregion

        #region IDbTransaction 成员

        public void Commit()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction = null;
            }
        }

        public IDbConnection Connection
        {
            get { return _connection; }
        }

        public IsolationLevel IsolationLevel
        {
            get { return _level; }
        }

        public void Rollback()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction = null;
            }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            Rollback();
            if (_connection != null)
                _connection.Dispose();
        }

        #endregion

    }
}
