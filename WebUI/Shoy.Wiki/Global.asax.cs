using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Shoy.Web;

namespace Shoy.Wiki
{
    public class MvcApplication : DApplication
    {
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
