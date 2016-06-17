using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Shoy.Utility.Logging;
using Shoy.Web;
using Shoy.Wiki.Areas.Manage;
using Shoy.Wiki.Contracts.Services;

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
            Bootstrap.BuilderHandler += b =>
            {
                b.RegisterGeneric(typeof(WikiRepository<>)).As(typeof(IWikiRepository<>));
                b.RegisterGeneric(typeof(WikiRepository<,>)).As(typeof(IWikiRepository<,>));
            };
            base.Application_Start(sender, e);
        }
    }
}
