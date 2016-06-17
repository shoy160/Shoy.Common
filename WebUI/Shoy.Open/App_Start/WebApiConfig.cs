using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace Shoy.Open
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApi",
                "api/{controller}/{id}",
                new
                {
                    id = RouteParameter.Optional
                });
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
