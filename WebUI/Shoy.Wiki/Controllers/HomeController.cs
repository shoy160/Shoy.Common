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
            var groups = _wikiContract.GroupList();
            return View(groups);
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