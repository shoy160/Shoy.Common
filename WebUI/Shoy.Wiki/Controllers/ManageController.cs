using System.Collections.Generic;
using System.Web.Mvc;
using Shoy.Web;
using Shoy.Wiki.Contracts;
using Shoy.Wiki.Models.Dtos;

namespace Shoy.Wiki.Controllers
{
    [RoutePrefix("manage")]
    public class ManageController : DController
    {
        private readonly IWikiContract _wikiContract;
        public ManageController(IWikiContract wikiContract)
        {
            _wikiContract = wikiContract;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WikiList()
        {
            var list = new List<WikiDto>();
            return View(list);
        }

        [Route("~/group/{code}")]
        public ActionResult Group(string code)
        {
            var item = _wikiContract.GroupRepository.SingleOrDefault(g => g.Code == code);
            return View(item);
        }

        [Route("~/group/add")]
        public ActionResult AddGroup(GroupDto group)
        {
            return View();
        }
    }
}