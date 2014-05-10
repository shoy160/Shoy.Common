using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Shoy.MvcTest.Models;
using Shoy.Solr;
using Microsoft.Practices.ServiceLocation;
using SolrNet;

namespace Shoy.MvcTest.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Index/

        public ActionResult Index()
        {
            var user = new User {Id = 0, Name = "shoy", PassWord = "dddddd", CreateOn = DateTime.Now};
            //using (var db = new UserContext())
            //{
            //    db.Users.Add(user);
            //    db.SaveChanges();
            //}
            return View(user);
        }

        public ActionResult Solr()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            var list = solr.Query(new SolrQueryByField("title", "s*"));
            var products = list.ToList();
            return View(products);
        }

        [HttpPost]
        public JsonResult SolrAdd()
        {
            var p = new Product
                {
                    Id = 2300,
                    Name = "贷款的法拉利萨顶顶",
                    Num = "sh0923",
                    Desc = "<p>dfdd</p>",
                    PriceMarket = 458,
                    PriceShop = 320,
                    CreateOn = DateTime.Now
                };
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            //var item = solr.Delete("1000");
            var item = solr.Add(p);
            //solr.Commit();
            return new JsonResult {Data = item.Status + "--" + item.QTime};
        }
    }
}
