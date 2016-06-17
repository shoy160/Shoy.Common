using System.Collections.Generic;
using System.Web.Mvc;
using Shoy.Web;
using Shoy.Wiki.Contracts;
using Shoy.Wiki.Models.Dtos;

namespace Shoy.Wiki.Areas.Manage.Controllers
{
    [RouteArea("manage", AreaPrefix = "manage")]
    public class ManageController : DController
    {
        private readonly IWikiContract _wikiContract;

        public ManageController(IWikiContract wikiContract)
        {
            _wikiContract = wikiContract;
        }

        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        #region Group

        [Route("groups")]
        public ActionResult GroupList()
        {
            var list = new List<GroupDto>();
            return View(list);
        }

        [HttpGet]
        [Route("group/{code}")]
        public ActionResult Group(string code)
        {
            var item = _wikiContract.GroupRepository.SingleOrDefault(g => g.Code == code);
            return View(item);
        }

        [HttpGet]
        [Route("add-group")]
        public ActionResult AddGroup()
        {
            return View();
        }

        [HttpPost]
        [Route("add-group")]
        public ActionResult AddGroup(GroupDto dto)
        {
            var result = _wikiContract.AddGroup(dto, string.Empty);
            return DeyiJson(result);
        }

        #endregion

        [Route("wikis")]
        public ActionResult WikiList()
        {
            var list = new List<WikiDto>();
            return View(list);
        }
    }
}