using System.Collections.Generic;
using System.Web.Mvc;
using Shoy.Web;
using Shoy.Wiki.Models.Dtos;

namespace Shoy.Wiki.Controllers
{
    [RoutePrefix("manage")]
    public class ManageController : DController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WikiList()
        {
            var list = new List<WikiDto>();
            return View(list);
        }
    }
}