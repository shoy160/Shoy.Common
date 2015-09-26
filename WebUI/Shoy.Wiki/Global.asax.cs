using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Shoy.Utility.Logging;
using Shoy.Web;

namespace Shoy.Wiki
{
    public class MvcApplication : DApplication
    {
        private readonly ILogger _logger = LogManager.Logger<MvcApplication>();
        public MvcApplication()
            : base(Assembly.GetExecutingAssembly())
        {
        }

        protected override void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            base.Application_Start(sender, e);
        }
    }
}
