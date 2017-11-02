using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Shoy.Open
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务
            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}_{action}");

            //设置json序列化方式
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.Formatters.JsonFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("format", "json", "application/json"));

            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            config.Formatters.XmlFormatter.MediaTypeMappings.Add(new QueryStringMapping("format", "xml",
                "application/xml"));

            config.Formatters.JsonFormatter.MediaTypeMappings.Add(new QueryStringMapping("format", "text",
                "text/plain"));
        }
    }
}
