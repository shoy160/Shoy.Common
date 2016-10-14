using System.Web.Mvc;
using com.shoy.dubbo.api;
using com.shoy.dubbo.model;
using Shoy.DubboWeb.Models;
using Shoy.Utility.Helper;

namespace Shoy.DubboWeb.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public string Hello(string name)
        {
            return DubboHelper.Instance.DubboAction<ShoyService, string>(t => t.sayHello(name));
        }

        public int Add(int a, int b)
        {
            return DubboHelper.Instance.DubboAction<ShoyService, int>(t => t.add(a, b));

            //            var factory = new CHessianProxyFactory();
            //            var service = (ShoyService)factory.Create(typeof(ShoyService), "http://192.168.10.254:8080/http_dubbo/hello");
            //            return service.add(a, b);
        }

        [HttpGet]
        public new ActionResult User()
        {
            var user = DubboHelper.Instance.DubboAction<ShoyService, User>(t => t.getUser());
            return Content(JsonHelper.ToJson(user, NamingType.CamelCase, true));
        }
    }
}