using System.Collections.Generic;
using System.Configuration;
using Shoy.Utility;
using Shoy.Utility.Extend;
using System.Text;
using Shoy.Utility.Helper;

namespace Shoy.Solr
{
    public class SolrContext
    {
        private static SolrConfig Config { get; set; }
        private static string BaseUrl { get; set; }
        private SolrContext(){}

        static SolrContext ()
        {
            Config = ConfigurationManager.GetSection("ShoySolr") as SolrConfig;
            if (Config != null)
            {
                BaseUrl = Config.Solr.BaseUrl + (Config.Solr.BaseUrl.EndsWith("/") ? "" : "/") + Config.Solr.CoreName;
            }
        }

        public static void SetConfig(string url, string core, string dataType)
        {
            Config = new SolrConfig
                {
                    Solr = new SolrBase
                        {
                            BaseUrl = url,
                            CoreName = core,
                            DataType = dataType
                        }
                };
            BaseUrl = Config.Solr.BaseUrl + (Config.Solr.BaseUrl.EndsWith("/") ? "" : "/") + Config.Solr.CoreName;
        }

        private static string GetResult(string method, Dictionary<string, string> dicts, string postData = "")
        {
            var type = "GET";
            if (method != "select") type = "Post";

            dicts.Add("wt", Config.Solr.DataType);
            var para = new StringBuilder();
            foreach (var key in dicts.Keys)
            {
                para.Append(key + "=" + dicts[key] + "&");
            }
            para.Length--;

            var url = string.Format("{0}/{1}?{2}", BaseUrl, method, para);
            using (var http = new HttpHelper(url, type, Encoding.UTF8, postData))
            {
                return http.GetHtml();
            }
        }

        private static string BuildJson<T>(T obj)
        {
            var t = new
                {
                    add = new
                        {
                            doc = obj,
                            overwrite = true,
                            commitWithin = 5000
                        }
                };
            return t.ToJson();
        }

        public static string Update<T>(T data)
        {
            var json = BuildJson(data);
            var result = GetResult("update", new Dictionary<string, string>(), json);
            return result;
        }

        public static string Query(string word)
        {
            var dict = new Dictionary<string, string> {{"q", Utils.UrlEncode(word)}};
            return GetResult("select", dict);
        }
    }
}
