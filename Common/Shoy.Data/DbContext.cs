using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace Shoy.Data
{
    public class MsSqlTemplate : DriverTemplate<SqlConnection, SqlCommand, SqlDataAdapter, SqlParameter, MsSqlBuilder> { }
    public class MsAccessTemplate : DriverTemplate<OleDbConnection, OleDbCommand, OleDbDataAdapter, OleDbParameter, MsAccessBuilder> { }

    public class DbContext
    {
        public static readonly Dictionary<string,ConnectionContext> ConnectionCache;

        public static readonly Dictionary<string, IDriver> DriversCache;

        private static ConnectionSession Config { get; set; }

        public static string Default { get; set; }

        static DbContext()
        {
            ConnectionCache = new Dictionary<string, ConnectionContext>(30);
            DriversCache = new Dictionary<string, IDriver>(15)
                               {
                                   {"MsSql", new MsSqlTemplate()},
                                   {"MsAccess", new MsAccessTemplate()}
                               };
            Config = (ConnectionSession) ConfigurationManager.GetSection("ShoyData");
            if(Config != null)
            {
                InitConfig();
            }
        }
        private DbContext(){}

        public static void InitConfig()
        {
            if (Config != null && Config.Connects.Count > 0)
            {
                foreach (DbConnect connect in Config.Connects)
                {
                    try
                    {
                        var driver = GetDriver(connect.ServerType);
                        if (driver != null)
                        {
                            AddConnection(connect.Name, connect.ConnectString, GetDriver(connect.ServerType));
                            if (connect.IsDefault)
                                Default = connect.Name;
                        }
                    }
                    catch{}
                }
            }
        }

        private static IDriver GetDriver(string key)
        {
            if (DriversCache.ContainsKey(key))
                return DriversCache[key];
            return null;
        }

        public static void AddDriver(string key,IDriver driver)
        {
            if (!DriversCache.ContainsKey(key))
                DriversCache.Add(key, driver);
        }

        public static void AddConnection(string name, string connectionString, IDriver driver)
        {
            if (ConnectionCache.ContainsKey(name))
                return;
            ConnectionCache.Add(name, new ConnectionContext(connectionString, driver));
        }

        public static void RemoveConnection(string name)
        {
            if (ConnectionCache.ContainsKey(name))
                ConnectionCache.Remove(name);
        }

        public static ConnectionContext Get(string name)
        {
            if (ConnectionCache.ContainsKey(name))
                return ConnectionCache[name].GetInstance();
            return Get();
        }

        public static ConnectionContext Get()
        {
            if (ConnectionCache.ContainsKey(Default))
                return ConnectionCache[Default].GetInstance();
            return null;
        }

        public static void TransactionExecute(string name, Action<IConnectionContext> handler)
        {
            using (IConnectionContext cc = DbContext.Get(name))
            {
                cc.BeginTransaction();
                handler(cc);
                cc.Commit();
            }
        }

        public static void Transaction(Action<IConnectionContext> handler)
        {
            using (IConnectionContext cc = DbContext.Get())
            {
                cc.BeginTransaction();
                handler(cc);
                cc.Commit();
            }
        }
        public static void Transaction(string name, Action<IConnectionContext> handler)
        {
            using (IConnectionContext cc = DbContext.Get(name))
            {
                cc.BeginTransaction();
                handler(cc);
                cc.Commit();
            }
        }

        public static void Transaction(IConnectionContext cc, Action<IConnectionContext> handler)
        {
            using (cc)
            {
                cc.BeginTransaction();
                handler(cc);
                cc.Commit();
            }
        }
    }
}
