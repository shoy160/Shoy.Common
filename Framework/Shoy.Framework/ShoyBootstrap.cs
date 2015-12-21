using System.Linq;
using System.Reflection;
using Autofac;
using Shoy.Core;
using Shoy.Core.Dependency;
using Shoy.Core.Domain.Repositories;
using Shoy.Core.Reflection;
using Shoy.Core.Wcf;
using Shoy.Data.EntityFramework;
using Shoy.Framework.Logging;
using Shoy.Utility;
using Shoy.Utility.Logging;

namespace Shoy.Framework
{
    public class ShoyBootstrap : DBootstrap
    {
        private ShoyBootstrap() { }
        public static ShoyBootstrap Instance
        {
            get
            {
                return (Singleton<ShoyBootstrap>.Instance ??
                        (Singleton<ShoyBootstrap>.Instance = new ShoyBootstrap()));
            }
        }

        public ContainerBuilder Builder { get; private set; }

        private IContainer _container;

        public IContainer Container
        {
            get { return _container ?? (_container = Builder.Build()); }
        }

        public delegate void BuilderAction(ContainerBuilder builderAction);

        public event BuilderAction BuilderHandler;

        public override void Initialize(Assembly executingAssembly = null)
        {
            LoggerInit();

            if (executingAssembly == null)
                executingAssembly = Assembly.GetExecutingAssembly();
            IocRegisters(executingAssembly);
            if (BuilderHandler != null)
                BuilderHandler(Builder);
            _container = Builder.Build();
            IocManager = _container.Resolve<IIocManager>();
            ModulesInstaller();
            CacheInit();
            DatabaseInit();

            #region WCF

            var wcfHelper = new WcfHelper
            {
                IocManager = IocManager,
                TypeFinder = IocManager.Resolve<ITypeFinder>()
            };
            wcfHelper.StartService();

            #endregion
        }

        /// <summary> 注册依赖 </summary>
        /// <param name="executingAssembly"></param>
        public override void IocRegisters(Assembly executingAssembly)
        {
            Builder = new ContainerBuilder();

            Builder.RegisterGeneric(typeof(UnitOfWorkDbContextProvider<>))
                .As(typeof(IDbContextProvider<>))
                .InstancePerLifetimeScope();

            Builder.RegisterGeneric(typeof(EfRepository<,,>)).As(typeof(IRepository<,>));

            var assemblies = ShoyAssemblyFinder.Instance.FindAll().Union(new[] { executingAssembly }).ToArray();
            Builder.RegisterAssemblyTypes(assemblies)
                .Where(type => typeof(ILifetimeDependency).IsAssignableFrom(type) && !type.IsAbstract)
                .AsSelf() //自身服务，用于没有接口的类
                .AsImplementedInterfaces() //接口服务
                .PropertiesAutowired() //属性注入
                .InstancePerLifetimeScope(); //保证生命周期基于请求

            Builder.RegisterAssemblyTypes(assemblies)
                .Where(type => typeof(IDependency).IsAssignableFrom(type) && !type.IsAbstract)
                .AsSelf() //自身服务，用于没有接口的类
                .AsImplementedInterfaces() //接口服务
                .PropertiesAutowired(); //属性注入
        }

        /// <summary> 初始化缓存 </summary>
        public override void CacheInit()
        {
        }

        /// <summary> 初始化日志模块 </summary>
        public override void LoggerInit()
        {
            LogManager.AddAdapter(new Log4NetAdapter());
        }

        /// <summary> 初始化数据库 </summary>
        public override void DatabaseInit()
        {
            //            Assembly assembly = Assembly.LoadFrom(file);
            //            DatabaseInitializer.AddMapperAssembly(assembly);
            //            DatabaseInitializer.Initialize();
        }
    }
}
