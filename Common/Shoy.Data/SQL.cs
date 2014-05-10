using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Shoy.Data
{
    public class SQL
    {
        private readonly Command _cmd = new Command("");

        public Command Cmd { get { return _cmd; } }

        public SQL(string sql)
        {
            _cmd.AddSqlText(sql);
        }

        public SQL AddSql(string sql)
        {
            _cmd.AddSqlText(sql);
            return this;
        }

        public static SQL operator +(string subsql, SQL sql)
        {
            sql.AddSql(subsql);
            return sql;
        }

        public static SQL operator +(SQL sql, string subsql)
        {
            sql.AddSql(subsql);
            return sql;
        }

        public SQL Parameter(string name, object value)
        {
            _cmd.AddParameter(name, value);
            return this;
        }

        public SQL this[string name, object value]
        {
            get { return Parameter(name, value); }
        }

        public static implicit operator SQL(string sql)
        {
            return new SQL(sql);
        }

        public int Execute()
        {
            using (var cc = DbContext.Get())
            {
                return Execute(cc);
            }
        }
        public int Execute(string type)
        {
            using (var cc = DbContext.Get(type))
            {
                return Execute(cc);
            }
        }
        public int Execute(IConnectionContext cc)
        {
            return cc.ExecuteNonQuery(_cmd);
        }

        public DataTable ExecuteDataTable()
        {
            using (var cc = DbContext.Get())
            {
                return ExecuteDataTable(cc);
            }
        }

        public DataTable ExecuteDataTable(string type)
        {
            using (var cc = DbContext.Get(type))
            {
                return ExecuteDataTable(cc);
            }
        }

        public DataTable ExecuteDataTable(IConnectionContext cc)
        {
            return cc.ExecuteDataTable(_cmd);
        }

        public DataSet ExecuteDataSet()
        {
            using (var cc = DbContext.Get())
            {
                return ExecuteDataSet(cc);
            }
        }

        public DataSet ExecuteDataSet(string type)
        {
            using (var cc = DbContext.Get(type))
            {
                return ExecuteDataSet(cc);
            }
        }

        public DataSet ExecuteDataSet(IConnectionContext cc)
        {
            return cc.ExecuteDataSet(_cmd);
        }

        public T GetValue<T>()
        {
            using (IConnectionContext cc = DbContext.Get())
            {
                return GetValue<T>(cc);
            }
        }

        public T GetValue<T>(string type)
        {
            using (var cc = DbContext.Get(type))
            {
                return GetValue<T>(cc);
            }
        }

        public T GetValue<T>(IConnectionContext cc)
        {
            return (T)cc.ExecuteScalar(GetCommand());
        }

        public T ListFirst<T>() where T : new()
        {
            using (var cc = DbContext.Get())
            {
                return ListFirst<T>(cc);
            }
        }

        public T ListFirst<T>(string type) where T : new()
        {
            using (var cc = DbContext.Get(type))
            {
                return ListFirst<T>(cc);
            }
        }

        public T ListFirst<T>(IConnectionContext cc) where T : new()
        {
            IList<T> result = List<T>(cc, new Region(0, 2));
            if (result.Count > 0)
                return result[0];
            return default(T);

        }

        internal object ListFirst(Type type, IConnectionContext cc)
        {
            IList result = List(type, cc, null);
            if (result.Count > 0)
                return result[0];
            return null;
        }

        public IList<T> List<T>() where T : new()
        {
            using (var cc = DbContext.Get())
            {
                return List<T>(cc);
            }
        }

        public IList<T> List<T>(Region region) where T : new()
        {
            using (var cc = DbContext.Get())
            {
                return List<T>(cc, region);
            }
        }
        public IList<T> List<T>(string type) where T : new()
        {
            using (var cc = DbContext.Get(type))
            {
                return List<T>(cc);
            }
        }
        public IList<T> List<T>(IConnectionContext cc) where T : new()
        {
            return List<T>(cc, null);
        }

        internal IList List(Type type, IConnectionContext cc, Region region)
        {
            var cmd = GetCommand();
            return cc.List(type, cmd, region);
        }


        public IList<T> List<T>(string type, Region region) where T : new()
        {
            using (var cc = DbContext.Get(type))
            {
                return List<T>(cc, region);
            }
        }

        public IList<T> List<T>(IConnectionContext cc, Region region) where T : new()
        {
            return (IList<T>)List(typeof(T), cc, region);
        }

        private Command GetCommand()
        {
            return _cmd;
        }
    }
}
