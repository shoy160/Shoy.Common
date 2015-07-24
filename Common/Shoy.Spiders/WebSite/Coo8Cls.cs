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
    /// 库巴网常用类
    /// </summary>
    public class Coo8Cls:WebSiteFactory
    {
        private static readonly Encoding SiteEncoding = Encoding.GetEncoding("gbk");
        public override decimal GetPrice()
        {
            try
            {
                GetHtml(SiteEncoding);
                var str = RegexHelper.Match(DocHtml, "product_price:\"([^\"']+)\"");
                return Convert.ToDecimal(str.Replace(",", ""));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public override int GetStockCode()
        {
            try
            {
                string stockUrl = GetWebSiteInfo().BaseUrl +
                                  "/interfaces/stock/hasStock.action?itemid={0}&province=71000000&city=71030000&county=71031500&a={1}";
                //成都青羊区
                //var data= {"productId":0,"storeStatus":"0","useTrans":"宅急送","goodsPattern":"SMI","isArrive":"1","isArrivePay":"1","freightPrice":"0","comeTime":"9月29日","refuseOrderTime":"7小时51分钟"}
                //var states = ["现货","预订","无货","在途"];
                var proId = RegexHelper.Match(ProLink, "product/([\\d]+).html");
                var url = String.Format(stockUrl, proId, RandomHelper.Random().NextDouble());
                using (var http = new HttpHelper(url, SiteEncoding))
                {
                    var html = http.GetHtml();
                    if (!html.IsNullOrEmpty())
                        html = RegexHelper.ClearTrn(html);
                    var status = RegexHelper.Match(html, "\"storeStatus\":\"([\\d]+)\"");
                    return (status == "0" ? 1 : 0);
                }
            }
            catch (Exception ex)
            {
                FileHelper.WriteException(ex);
                return -1;
            }
        }

        public override string GetProName()
        {
            try
            {
                GetHtml(SiteEncoding);
                var name = HtmlCls.GetHtmlById(DocHtml, "title-descript");
                return Regex.Replace(name, "</?[0-9a-zA-Z]+[^>]*>", "").Trim();
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
                var pic = HtmlCls.GetHtmlById(DocHtml, "BigPic");
                return RegexHelper.Match(pic, "\\s+src=[\"']([^\"'>\\?]+)(\\?[^\"']*)?[\"']");
            }
            catch (Exception)
            {
                return "";
            }
        }

        public override IEnumerable<string> GetUrlList(string listUrl)
        {
            try
            {
                using (var http = new HttpHelper(listUrl, SiteEncoding))
                {
                    var html = http.GetHtml();
                    if (!html.IsNullOrEmpty())
                        html = RegexHelper.ClearTrn(html);
                    html = HtmlCls.GetHtmlByCss(html, "search-content").FirstOrDefault();
                    var showList = HtmlCls.GetHtmlByCss(html, "name");
                    var list =
                        showList.Select(t => RegexHelper.Match(t, "<a[^>]*href=[\"']?([^\"'>]+)(#[^\"'>]*)?[\"']?[^>]*>"))
                            .Distinct().ToList();
                    return list.Where(t => !t.IsNullOrEmpty()).Select(t => Utils.GetAbsoluteUrl(GetWebSiteInfo().BaseUrl, t)).ToList();
                }
            }
            catch(Exception)
            {
                return new List<string>();
            }
        }

        public override IEnumerable<string> SearchWord(string word)
        {
            string searchUrl = GetWebSiteInfo().BaseUrl + "/interfaces/search/showSearchResult.action?searchKeywords={0}";
            //var url1 = String.Format(searchUrl, Utils.UrlEncode(word).Replace("%", "%25"));
            var url = String.Format(searchUrl, Utils.UrlEncode(word, SiteEncoding));
            return GetUrlList(url);
        }

        public override decimal GetMarketPrice()
        {
            try
            {
                GetHtml(SiteEncoding);
                var str = RegexHelper.Match(DocHtml, "<del[^>]*>￥([^<]+)</del>");
                return Convert.ToDecimal(str.Replace(",", ""));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public override WebSiteInfo GetWebSiteInfo()
        {
            return new WebSiteInfo
                       {
                           BaseUrl = "http://www.coo8.com",
                           LogPic = "http://img01.taobaocdn.com/imgextra/etao/i1/T1UIVMXjRyXXb1upjX.jpg_80x40.jpg",
                           WebName = "库巴网"
                       };
        }
    }
}
