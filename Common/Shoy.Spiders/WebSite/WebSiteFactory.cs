using System.Collections.Generic;
using System.Reflection;
using Shoy.Utility;
using Shoy.Utility.Extend;
using System.Text;
using Shoy.Utility.Helper;

namespace Shoy.Spiders.WebSite
{
    public abstract class WebSiteFactory
    {
        protected static string ProLink;
        protected static string DocHtml;

        public static WebSiteFactory GetInstance(WebSites web)
        {
            return GetInstance(web.GetValue());
        }

        public static WebSiteFactory GetInstance(string web)
        {
            WebSiteFactory instance;
            if (!string.IsNullOrEmpty(web))
            {
                var ass = Assembly.Load("Shoy.Spiders");
                instance =
                    (WebSiteFactory)
                    ass.CreateInstance("Shoy.Spiders.WebSite." + web + "Cls");
            }
            else
                instance = null;
            return instance;
        }

        protected static void GetHtml(Encoding encoding)
        {
            if (DocHtml.IsNullOrEmpty())
            {
                using (var http = new HttpHelper(ProLink, encoding))
                {
                    DocHtml = http.GetHtml();
                    if (!DocHtml.IsNullOrEmpty())
                        DocHtml = RegexHelper.ClearTrn(DocHtml);
                }
            }
        }

        protected static void GetHtml()
        {
            GetHtml(Encoding.UTF8);
        }

        public void SetUrl(string link)
        {
            ProLink = link;
            DocHtml = "";
        }

        public abstract WebSiteInfo GetWebSiteInfo();

        public abstract IEnumerable<string> GetUrlList(string listUrl);

        public abstract IEnumerable<string> SearchWord(string word);

        public abstract decimal GetPrice();

        public abstract decimal GetMarketPrice();

        public abstract int GetStockCode();

        public abstract string GetProName();

        public abstract string GetProPic();
    }
}
