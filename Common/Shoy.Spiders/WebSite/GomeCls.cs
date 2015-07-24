using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shoy.Utility;
using Shoy.Utility.Extend;
using System.Text.RegularExpressions;
using Shoy.Utility.Helper;

namespace Shoy.Spiders.WebSite
{
    /// <summary>
    /// 国美常用类
    /// </summary>
    public class GomeCls:WebSiteFactory
    {
        private static readonly Encoding SiteEncoding = Encoding.UTF8;

        public override IEnumerable<string> GetUrlList(string listUrl)
        {
            try
            {
                using (var http = new HttpHelper(listUrl, SiteEncoding))
                {
                    var html = http.GetHtml();
                    html = (!html.IsNullOrEmpty() ? RegexHelper.ClearTrn(html) : http.GetHtml());
                    if (html.IsNullOrEmpty())
                        return new List<string>();
                    var showList = HtmlCls.GetHtmlByCss(html, "pic");
                    var list =
                        showList.Select(t => RegexHelper.Match(t, "<a[^>]*href=[\"']?([^\"'>;]+)(;[^\"'>]*)?[\"']?[^>]*>"))
                            .Distinct().ToList();
                    return list.Where(t => !t.IsNullOrEmpty()).Select(t => Utils.GetAbsoluteUrl(GetWebSiteInfo().BaseUrl, t)).ToList();
                }
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public override IEnumerable<string> SearchWord(string word)
        {
            //http://www.gome.com.cn/ec/homeus/atgsearch/gomeSearchResults.jsp?question=%E8%AF%BA%E5%9F%BA%E4%BA%9A
            string searchUrl = GetWebSiteInfo().BaseUrl + "/ec/homeus/atgsearch/gomeSearchResults.jsp?question={0}";
            var url = String.Format(searchUrl, Utils.UrlEncode(word, SiteEncoding));
            return GetUrlList(url);
        }

        public override decimal GetPrice()
        {
            try
            {
                GetHtml(SiteEncoding);//prdprice
                var str = RegexHelper.Match(DocHtml, "<b[^>]*class=[\"'][\"'][^>]*>\\s*([^<\\s]+)\\s*</b>");
                return Convert.ToDecimal(str.Replace(",", ""));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public override decimal GetMarketPrice()
        {
            return 0;
        }

        public override int GetStockCode()
        {
            try
            {
                GetHtml(SiteEncoding);
                var proNum = HtmlCls.GetHtmlByCss(DocHtml, "prodNum").ToList()[1];
                proNum = Regex.Replace(proNum, "</?[0-9a-zA-Z]+[^>]*>", "").Replace("商品编号：", "").Trim();
                var stockUrl = GetWebSiteInfo().BaseUrl + "/ec/homeus/browse/exactMethod.jsp?goodsNo={0}&city=71010000";
                stockUrl = String.Format(stockUrl, proNum);
                using (var http = new HttpHelper(stockUrl, SiteEncoding))
                {
                    var html = http.GetHtml();
                    var str = RegexHelper.Match(html, "\"result\":\"([a-zA-Z])\"");
                    return (str == "Y" ? 1 : 0);
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public override string GetProName()
        {
            try
            {
                GetHtml(SiteEncoding);
                var name = RegexHelper.Match(DocHtml, "var tdisplayName = encodeURI(Component)?\\('([^']+)'\\);", 2);
                return name;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public override string GetProPic()
        {
            try
            {
                GetHtml(SiteEncoding);
                var pic = HtmlCls.GetHtmlById(DocHtml, "bgPics");
                return RegexHelper.Match(pic, "\\s+src=[\"']([^\"'>]+)[\"']");
            }
            catch (Exception)
            {
                return "";
            }
        }

        public override WebSiteInfo GetWebSiteInfo()
        {
            return new WebSiteInfo
                       {
                           BaseUrl = "http://www.gome.com.cn",
                           LogPic = "http://img01.taobaocdn.com/imgextra/etao/i1/T1IkqNXhlfXXb1upjX.jpg_80x40.jpg",
                           WebName = "国美网上商城"
                       };
        }
    }
}
