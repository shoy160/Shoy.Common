using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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
        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.LowercaseUrls = true;

            routes.MapMvcAttributeRoutes();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("share_views", "share/{resourceName}",
                new { controller = "Resource", action = "Index", resourcePath = "Views" });

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional });
        }

        protected virtual void Application_Start(object sender, EventArgs e)
        {
            //路由注册
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);

            //MVC依赖注入
            Bootstrap.BuilderHandler += b =>
            {
                //mvc注入
                b.RegisterControllers(_executingAssembly).PropertiesAutowired();
                b.RegisterFilterProvider();
            };
            Bootstrap.Initialize(_executingAssembly);
            _logger.Info("Application_Start...");
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Bootstrap.Container));
            //异常处理
            GlobalFilters.Filters.Add(DExceptionAttribute.Instance);
        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
            _logger.Info("Application_End...");
            Bootstrap.Dispose();
        }

        protected virtual void Session_Start(object sender, EventArgs e)
        {
            _logger.Debug("Session_Start...");
        }

        protected virtual void Session_End(object sender, EventArgs e)
        {
            _logger.Debug("Session_End...");
        }
        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
            _logger.Debug("Application_BeginRequest...");
        }

        protected virtual void Application_EndRequest(object sender, EventArgs e)
        {
            _logger.Debug("Application_EndRequest...");
        }

        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            _logger.Debug("Application_AuthenticateRequest...");
        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {
            _logger.Info("Application_Error...");
            var ex = Server.GetLastError().GetBaseException();
            _logger.Error(ex.Message, ex);
        }
    }
}
