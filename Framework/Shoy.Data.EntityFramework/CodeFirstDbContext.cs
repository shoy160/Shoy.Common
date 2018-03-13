using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using Shoy.Core.Config;
using Shoy.Core.Domain;
using Shoy.Core.Events;
using Shoy.Utility.Config;
using Shoy.Utility.Helper;
using Shoy.Utility.Logging;
using TransactionalBehavior = Shoy.Core.Domain.TransactionalBehavior;

namespace Shoy.Data.EntityFramework
{
    public class CodeFirstDbContext : DbContext, IUnitOfWork
    {
        private static readonly ILogger Logger = LogManager.Logger<CodeFirstDbContext>();
        public IEntityChangedEventHelper ChangedEventHelper { get; set; }

        private Dictionary<object, EntityState> _entityStates;

        #region 构造函数

        private void InitConfiguration()
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            //            Database.Log += LogManager.Logger<CodeFirstDbContext>().Info;
            _entityStates = new Dictionary<object, EntityState>();
        }

        /// <summary>
        /// 初始化一个<see cref="CodeFirstDbContext"/>类型的新实例
        /// </summary>
        public CodeFirstDbContext()
            : this(GetConnectionStringName())
        {
        }

        /// <summary>
        /// 使用连接名称或连接字符串 初始化一个<see cref="CodeFirstDbContext"/>类型的新实例
        /// </summary>
        public CodeFirstDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            InitConfiguration();
        }

        public CodeFirstDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
            InitConfiguration();
        } 
        #endregion

        /// <summary> 获取 数据库连接串名称 (从 ConnectionStrings中获取) </summary>
        protected static string GetConnectionStringName(string key = "connectionName",
            string def = "default")
        {
            return ConfigHelper.GetAppSetting(defaultValue: def, supressKey: key);
        }

        /// <summary> 获取 数据库连接串 (从 database.config中获取)</summary>
        protected static string GetConnectionString(string name = "default")
        {
            var config = ConfigUtils<DataBaseConfig>.Instance.Get().Get(name);
            if (config == null)
                return "name=default";
            return config.ConnectionString;
        }

        /// <summary>
        /// 根据数据库配置名创建Connection,解决非sql server
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected static DbConnection GetConnection(string name)
        {
            var config = ConfigUtils<DataBaseConfig>.Instance.Get()?.Get(name);
            if (config == null)
                return null;
            var conn = DbProviderFactories.GetFactory(config.ProviderName).CreateConnection();
            if (conn == null) return null;
            conn.ConnectionString = config.ConnectionString;
            return conn;
        }

        private static bool DataLoggingEnabled
        {
            get { return ConfigHelper.GetAppSetting(defaultValue: false, supressKey: "dataloggingEnabled"); }
        }

        public bool IsTransaction { get; set; }

        public int SqlExecute(TransactionalBehavior behavior, string sql, params object[] parameters)
        {
            var transactionalBehavior =
                (behavior == TransactionalBehavior.DoNotEnsureTransaction
                    ? System.Data.Entity.TransactionalBehavior.DoNotEnsureTransaction
                    : System.Data.Entity.TransactionalBehavior.EnsureTransaction);
            return Database.ExecuteSqlCommand(transactionalBehavior, sql, parameters);
        }

        public IEnumerable<TEntity> SqlQuery<TEntity>(string sql, params object[] parameters)
        {
            return Database.SqlQuery<TEntity>(sql, parameters);
        }

        public IEnumerable SqlQuery(Type entityType, string sql, params object[] parameters)
        {
            return Database.SqlQuery(entityType, sql, parameters);
        }

        #region SaveChanges
        /// <summary>
        /// 提交当前单元操作的更改。
        /// </summary>
        /// <returns>操作影响的行数</returns>
        public override int SaveChanges()
        {
            FillConcepts();
            return SaveChanges(true);
        }

        /// <summary> 填充数据更改概念 </summary>
        private void FillConcepts()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        _entityStates.Add(entry.Entity, entry.State);
                        break;
                }
            }
        }

        /// <summary> 应用数据概念更改 </summary>
        private void ApplyConcepts()
        {
            foreach (var entry in _entityStates.Keys)
            {
                switch (_entityStates[entry])
                {
                    case EntityState.Added:
                        ChangedEventHelper.TriggerCreatedEvent(entry);
                        break;
                    case EntityState.Modified:
                        ChangedEventHelper.TriggerUpdatedEvent(entry);
                        break;
                    case EntityState.Deleted:
                        ChangedEventHelper.TriggerDeletedEvent(entry);
                        break;
                }
            }
        }

        /// <summary>
        /// 提交当前单元操作的更改。
        /// </summary>
        /// <param name="validateOnSaveEnabled">提交保存时是否验证实体约束有效性。</param>
        /// <returns>操作影响的行数</returns>
        private int SaveChanges(bool validateOnSaveEnabled)
        {
            bool isReturn = Configuration.ValidateOnSaveEnabled != validateOnSaveEnabled;
            try
            {
                Configuration.ValidateOnSaveEnabled = validateOnSaveEnabled;
                int count = base.SaveChanges();
                IsTransaction = false;
                if (count > 0)
                {
                    ApplyConcepts();
                }
                return count;
            }
            catch (DbUpdateException e)
            {
                Logger.Error(e.Message, e);
                if (e.InnerException != null && e.InnerException.InnerException is SqlException)
                {
                    var sqlEx = (SqlException)e.InnerException.InnerException;
                    var msg = GetSqlExceptionMessage(sqlEx.Number);
                    throw new Exception("提交数据更新时发生异常：" + msg, sqlEx);
                }
                throw;
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    msg += string.Format("Table:{0},", validationErrors.Entry.Entity);
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0},Error: {1}{2}", validationError.PropertyName,
                            validationError.ErrorMessage, Environment.NewLine);
                }
                Logger.Error(msg, dbEx);
                throw new Exception(msg, dbEx);
            }
            finally
            {
                if (isReturn)
                {
                    Configuration.ValidateOnSaveEnabled = !validateOnSaveEnabled;
                }
                _entityStates.Clear();
            }
        }
        #endregion

        #region Transaction
        public int Transaction(Action action)
        {
            IsTransaction = true;
            action();
            return SaveChanges();
        }
        #endregion

        #region ModelCreating
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //PostgreSQL
            //modelBuilder.HasDefaultSchema("public");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //移除一对多的级联删除
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            //            //注册实体配置信息
            //            ICollection<IEntityMapper> entityMappers = DatabaseInitializer.EntityMappers;
            //            foreach (IEntityMapper mapper in entityMappers)
            //            {
            //                mapper.RegistTo(modelBuilder.Configurations);
            //            }
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 由错误码返回指定的自定义SqlException异常信息
        /// </summary>
        /// <param name="number"> </param>
        /// <returns> </returns>
        private static string GetSqlExceptionMessage(int number)
        {
            string msg = string.Empty;
            switch (number)
            {
                case 2:
                    msg = "连接数据库超时，请检查网络连接或者数据库服务器是否正常。";
                    break;
                case 17:
                    msg = "SqlServer服务不存在或拒绝访问。";
                    break;
                case 17142:
                    msg = "SqlServer服务已暂停，不能提供数据服务。";
                    break;
                case 2812:
                    msg = "指定存储过程不存在。";
                    break;
                case 208:
                    msg = "指定名称的表不存在。";
                    break;
                case 4060: //数据库无效。
                    msg = "所连接的数据库无效。";
                    break;
                case 18456: //登录失败
                    msg = "使用设定的用户名与密码登录数据库失败。";
                    break;
                case 547:
                    msg = "外键约束，无法保存数据的变更。";
                    break;
                case 2627:
                    msg = "主键重复，无法插入数据。";
                    break;
                case 2601:
                    msg = "未知错误。";
                    break;
            }
            return msg;
        }
        #endregion
    }
}
