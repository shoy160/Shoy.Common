using System.Web.Mvc;
using com.dayeasy.service.paper.api;
using com.dayeasy.service.paper.model;
using Shoy.DubboWeb.Models;
using Shoy.Utility.Helper;

namespace Shoy.DubboWeb.Controllers
{
    public class PaperController : Controller
    {
        public string Paper()
        {
            //            var factory = new CHessianProxyFactory();
            //            var service = (IPaperService)factory.Create(typeof(IPaperService), "http://192.168.10.14:8088/paper");
            //            return service.getPaperId();
            return DubboHelper.Instance.DubboAction<IPaperService, string>(t => t.createPaper());
        }

        public ActionResult Index(string id)
        {
            var dto = DubboHelper.Instance.DubboAction<IPaperService, PaperDto>(t => t.getPaper(id));
            return Content(JsonHelper.ToJson(dto, NamingType.CamelCase, true), "application/json");
        }
    }
}