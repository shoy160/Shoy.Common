using System.Web.Mvc;
using System.Web.Routing;
using StackExchange.Profiling;
using StackExchange.Profiling.EntityFramework6;

namespace Shoy.MvcDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            MiniProfilerEF6.Initialize();
        }

        protected void Application_BeginRequest()
        {
            MiniProfiler.Start();
        }
        protected void Application_EndRequest()
        {
            MiniProfiler.Stop();
        }
    }
}
