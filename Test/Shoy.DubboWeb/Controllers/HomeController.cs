using System;
using System.Web.Mvc;
using com.shoy.dubbo.api;
using Damai.Dubbo.Client;
using Shoy.Utility.Helper;

namespace Shoy.DubboWeb.Controllers
{
    public class HomeController : Controller
    {
        public T ServiceAction<T>(Func<ShoyService, T> action)
        {
            var client = new DubboClient("net-consumer", "zookeeper", "zookeeper://192.168.10.14:2181", 2181);
            using (var reference = new ServiceReference<ShoyService>
            {
                DubboClient = client,
                Registry = client.Registry,
                Timeout = 1000
            })
            {
                return action(reference.Get());
            }
        }
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public string Hello(string name)
        {
            return ServiceAction(t => t.sayHello(name));
        }

        public int Add(int a, int b)
        {
            return ServiceAction(t => t.add(a, b));

            //            var factory = new CHessianProxyFactory();
            //            var service = (ShoyService)factory.Create(typeof(ShoyService), "http://192.168.10.254:8080/http_dubbo/hello");
            //            return service.add(a, b);
        }

        [HttpGet]
        public new ActionResult User()
        {
            var user = ServiceAction(t => t.getUser());
            return Content(JsonHelper.ToJson(user, NamingType.CamelCase, true));
        }
    }
}