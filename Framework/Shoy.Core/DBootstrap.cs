
using System.Reflection;
using Shoy.Core.Dependency;
using Shoy.Core.Modules;

namespace Shoy.Core
{
    /// <summary> 应用程序启动基类 </summary>
    public abstract class DBootstrap : IBootstrap
    {
        public abstract void Initialize(Assembly executingAssembly = null);

        protected bool IsDisposed;

        /// <summary> 注入管理 </summary>
        public IIocManager IocManager { get; protected set; }

        /// <summary> 注册依赖 </summary>
        public abstract void IocRegisters(Assembly executingAssembly);

        /// <summary> 初始化各个模块 </summary>
        public void ModulesInstaller()
        {
            IocManager.Resolve<IModuleManager>().InitializeModules();
        }

        /// <summary> 缓存初始化 </summary>
        public abstract void CacheInit();

        /// <summary> 日志初始化 </summary>
        public abstract void LoggerInit();

        /// <summary> 数据库初始化 </summary>
        public abstract void DatabaseInit();

        public virtual void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            IocManager.Resolve<IModuleManager>().ShutdownModules();
        }
    }
}
