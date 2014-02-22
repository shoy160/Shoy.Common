using System;
using Shoy.Utility.Extend;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrNet;
using SolrNet.Attributes;
using Shoy.Solr;

namespace Shoy.Test
{
    /// <summary>
    /// SolrTest 的摘要说明
    /// </summary>
    [TestClass]
    public class SolrTest
    {
        private readonly ISolrOperations<Product> _solr;

        static SolrTest()
        {
            Startup.Init<Product>("http://192.168.157.130:8080/solr/test");
        }

        public SolrTest()
        {
            _solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            SolrContext.SetConfig("http://192.168.157.130:8080/solr", "test", "json");
        }

        [TestMethod]
        public void QueryTest()
        {
            var result = _solr.Query(new SolrQuery("s*"));
            Console.Write(result.ToJson());
        }

        [TestMethod]
        public void AddTest()
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
            var result = SolrContext.Update(p); //_solr.Add(p);
            Console.Write(result.ToJson());
        }

        public class Product
        {
            [SolrUniqueKey("id")]
            public int Id { get; set; }
            [SolrNet.Attributes.SolrField("name")]
            public string Name { get; set; }
            [SolrNet.Attributes.SolrField("title")]
            public string[] Title { get; set; }
            [SolrNet.Attributes.SolrField("num")]
            public string Num { get; set; }
            [SolrNet.Attributes.SolrField("desc")]
            public string Desc { get; set; }
            [SolrNet.Attributes.SolrField("createOn")]
            public DateTime CreateOn { get; set; }
            [SolrNet.Attributes.SolrField("priceMarket")]
            public decimal PriceMarket { get; set; }
            [SolrNet.Attributes.SolrField("priceShop")]
            public decimal PriceShop { get; set; }
        }
    }
}
