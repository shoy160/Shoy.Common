using Autofac;
using Autofac.Configuration;
using Autofac.Integration.Mvc;
using Shoy.Injection;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

[assembly: PreApplicationStartMethod(typeof(Register), "Init")]

namespace Shoy.Injection
{
    public class Register
    {
        public static void Init()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));

            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsImplementedInterfaces();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            DiHelper.Register(container);
        }
    }
}
