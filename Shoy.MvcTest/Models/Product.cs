using System;
using SolrNet.Attributes;

namespace Shoy.MvcTest.Models
{
    public class Product
    {
        [SolrUniqueKey("id")]
        public int Id { get; set; }
        [SolrField("name")]
        public string Name { get; set; }
        [SolrField("title")]
        public string[] Title { get; set; }
        [SolrField("num")]
        public string Num { get; set; }
        [SolrField("desc")]
        public string Desc { get; set; }
        [SolrField("createOn")]
        public DateTime CreateOn { get; set; }
        [SolrField("priceMarket")]
        public decimal PriceMarket { get; set; }
        [SolrField("priceShop")]
        public decimal PriceShop { get; set; }
    }
}