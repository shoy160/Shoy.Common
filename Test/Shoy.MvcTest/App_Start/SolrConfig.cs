using Shoy.MvcTest.Models;
using SolrNet.Impl;

namespace Shoy.MvcTest
{
    public class SolrConfig
    {
        public static void InitSolr()
        {
            var con = new SolrConnection("http://192.168.157.130:8080/solr/test");
            
            SolrNet.Startup.Init<Product>(con);
        }
    }
}