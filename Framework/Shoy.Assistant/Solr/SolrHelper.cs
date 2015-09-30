using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Practices.ServiceLocation;
using Shoy.Assistant.Config;
using Shoy.Utility;
using Shoy.Utility.Config;
using Shoy.Utility.Extend;
using Shoy.Utility.Helper;
using SolrNet;

namespace Shoy.Assistant.Solr
{
    /// <summary> Solr辅助类 </summary>
    public class SolrHelper
    {
        private static readonly Dictionary<Type, string> SolrUrlDicts = new Dictionary<Type, string>();

        public static SolrHelper Instance
        {
            get
            {
                return Singleton<SolrHelper>.Instance
                       ?? (Singleton<SolrHelper>.Instance = new SolrHelper());
            }
        }

        public void InitSolr<T>()
        {
            string url = string.Empty;
            var type = typeof(T);
            if (SolrUrlDicts.ContainsKey(type))
                url = SolrUrlDicts[type];
            else
            {
                var config = ConfigUtils<SolrConfig>.Config;
                if (config == null || !config.SolrList.Any()) return;
                var attr = type.GetCustomAttributes(false).FirstOrDefault(t => t is SolrCore) as SolrCore;
                string coreName = (attr != null ? attr.CoreName : type.Name);
                var solr = config.SolrList.FirstOrDefault(t => t.Name == coreName);
                if (solr != null)
                {
                    url = solr.CoreUrl;
                    SolrUrlDicts.Add(type, url);
                }
            }
            if (!string.IsNullOrWhiteSpace(url))
                Startup.Init<T>(url);
        }

        public ISolrOperations<T> Solr<T>()
        {
            return ServiceLocator.Current.GetInstance<ISolrOperations<T>>();
        }

        private static string GetJsonTime()
        {
            var reg = new Regex("(\\d+)");
            return reg.Match(DateTime.Now.ToJson()).Groups[1].Value;
        }

        public ResponseHeader Commit<T>()
        {
            const string action = "{0}/update?commit=true&wt=json&_={1}";
            var url = SolrUrlDicts[typeof(T)];
            using (var http = new HttpHelper(string.Format(action, url, GetJsonTime())))
            {
                var html = http.GetHtml();
                return html.JsonToObject<ResponseHeader>();
            }
        }

        public ResponseHeader DataImport<T>(bool fullImport)
        {
            const string action = "{0}/dataimport";
            var url = string.Format(action, SolrUrlDicts[typeof(T)]);
            string command = (fullImport ? "full-import" : "delta-import"),
                clean = (fullImport ? "true" : "false");

            const string parasTemp =
                "command={0}&commit=true&wt=json&indent=true&verbose=false&clean={1}&optimize=false&debug=false";
            var paras = string.Format(parasTemp, command, clean);
            using (var http = new HttpHelper(url, "post", Encoding.UTF8, paras))
            {
                var html = http.GetHtml();
                return html.JsonToObject<ResponseHeader>();
            }
        }

        public SortOrder SortOrder<T>(T type)
            where T : struct
        {
            //随机排序
            if ("random".Equals(type.ToString(), StringComparison.CurrentCultureIgnoreCase))
                return new RandomSortOrder("random_" + RandomHelper.Random().Next());
            var enumType = type.GetType();
            var name = type.ToString();
            var order = enumType.GetField(name).GetCustomAttribute<DSolrOrder>();
            return order != null
                ? new SortOrder(order.FieldName, order.OrderType)
                : new SortOrder(name);
        }
    }
}
