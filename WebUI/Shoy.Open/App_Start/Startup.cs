
using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(Shoy.Open.Startup))]
namespace Shoy.Open
{
    /// <summary> 程序启动 </summary>
    public partial class Startup
    {
        /// <summary> 配置 </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
            SwaggerConfig.Register();
        }
    }
}