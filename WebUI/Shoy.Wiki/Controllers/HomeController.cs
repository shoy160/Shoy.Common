using System.Web.Mvc;
using Shoy.Web;
using Shoy.Wiki.Contracts;

namespace Shoy.Wiki.Controllers
{
    public class HomeController : DController
    {
        private readonly IWikiContract _wikiContract;
        public HomeController(IWikiContract wikiContract)
        {
            _wikiContract = wikiContract;
        }
        // GET: Home
        public ActionResult Index()
        {
            _wikiContract.AddWiki("Oracle", "5ab24eb96a5d2cca", "b783b4cfba517267",
                "Oracle Database，又名Oracle RDBMS，或简称Oracle。是甲骨文公司的一款关系数据库管理系统。它是在数据库领域一直处于领先地位的产品。可以说Oracle数据库系统是目前世界上流行的关系数据库管理系统，系统可移植性好、使用方便、功能强，适用于各类大、中、小、微机环境。它是一种高效率、可靠性好的 适应高吞吐量的数据库解决方案。");

            var groups = _wikiContract.GroupList();
            return View(groups);
        }

        [Route("~/group/{code}")]
        public ActionResult Group(string code)
        {
            var item = _wikiContract.GroupRepository.SingleOrDefault(g => g.Code == code);
            return View(item);
        }

        [Route("~/wiki/{name}")]
        public ActionResult Wiki(string name)
        {
            var wiki = _wikiContract.Load(name);
            if (wiki == null)
                return RedirectToAction("Index");
            return View(wiki);
        }
    }
}