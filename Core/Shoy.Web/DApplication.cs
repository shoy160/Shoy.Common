using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using Shoy.Framework;
using Shoy.Utility.Logging;
using Shoy.Web.Filters;

namespace Shoy.Web
{
    public abstract class DApplication : HttpApplication
    {
        private readonly ILogger _logger = LogManager.Logger<DApplication>();
        protected ShoyBootstrap Bootstrap { get; private set; }

        private readonly Assembly _executingAssembly;

        protected DApplication(Assembly executing)
        {
            Bootstrap = ShoyBootstrap.Instance;
            _executingAssembly = executing;
        }

        protected virtual void Application_Start(object sender, EventArgs e)
        {
            _logger.Info("Application_Start...");
            //MVC依赖注入
            Bootstrap.BuilderHandler += b =>
            {
                //mvc注入
                b.RegisterControllers(_executingAssembly).PropertiesAutowired();
                b.RegisterFilterProvider();
            };
            Bootstrap.Initialize(_executingAssembly);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Bootstrap.Container));
            //异常处理
            GlobalFilters.Filters.Add(DExceptionAttribute.Instance);
        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
            _logger.Info("Application_End...");
            Bootstrap.ModulesInstaller();
            //            Bootstrap.Dispose();
        }

        protected virtual void Session_Start(object sender, EventArgs e)
        {

        }

        protected virtual void Session_End(object sender, EventArgs e)
        {

        }
        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected virtual void Application_EndRequest(object sender, EventArgs e)
        {
        }

        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {
            _logger.Info("Application_Error...");
        }
    }
}
