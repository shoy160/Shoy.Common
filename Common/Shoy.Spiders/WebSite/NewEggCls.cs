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
    /// 新蛋常用类
    /// </summary>
    public class NewEggCls:WebSiteFactory
    {
        private static readonly Encoding SiteEncoding = Encoding.GetEncoding("gb2312");

        public override IEnumerable<string> GetUrlList(string listUrl)
        {
            try
            {
                using (var http = new HttpHelper(listUrl, SiteEncoding))
                {
                    var html = http.GetHtml();
                    var url = http.GetRequestUrl();
                    if (Regex.IsMatch(url, "^http://www.newegg.com.cn/Product/[0-9a-zA-Z\\-]+.htm$"))
                        return new List<string> {url};
                    if (!html.IsNullOrEmpty())
                        html = RegexHelper.ClearTrn(html);
                    var showList = HtmlCls.GetHtmlById(html, "itemGrid1");
                    var list =
                        RegexHelper.Matches(showList, "(http://www.newegg.com.cn/Product/[0-9a-zA-Z\\-]+.htm)").Distinct()
                            .ToList();
                    return
                        list.Where(t => !t.IsNullOrEmpty()).Select(
                            t => Utils.GetAbsoluteUrl(GetWebSiteInfo().BaseUrl, t)).ToList();
                }
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public override IEnumerable<string> SearchWord(string word)
        {
            string searchUrl = GetWebSiteInfo().BaseUrl + "/Search.aspx?keyword={0}&TabStore=";
            var url = String.Format(searchUrl, Utils.UrlEncode(word, SiteEncoding));
            return GetUrlList(url);
        }

        public override decimal GetPrice()
        {
            try
            {
                //pvalues
                GetHtml(SiteEncoding);
                var str = RegexHelper.Match(DocHtml, "pvalues:([0-9\\.,]+)");
                return Math.Round(Convert.ToDecimal(str.Replace(",", "")), 1);
                //var cartUrl = GetWebSiteInfo().BaseUrl +
                //              "/Shopping/ShoppingCart.aspx?action=Add&productno={0}&quantity=1&warrantyID=&FPA=5&CMSCT=0";
                //var url = GetWebSiteInfo().BaseUrl + "/Shopping/ShoppingCart.aspx?productno={0}&FPA=5&CMSCT=0";
                //cartUrl = String.Format(cartUrl, GetProductId());
                //using (var http = new HttpHelper(cartUrl, SiteEncoding))
                //{
                //    string cookie = http.GetCookie();
                //    cookie = cookie.Replace("cartQty=0", "cartQty=1");
                //    Utils.WriteFile(Utils.GetCurrentDir() + "log.txt", cookie);
                //    url = String.Format(url, GetProductId());
                //    http.SetHttpInfo(url, cookie, cartUrl);
                //    Thread.Sleep(300);
                //    var str = http.GetHtml();
                //    Utils.WriteFile(Utils.GetCurrentDir() + "log.txt", str);
                //    str = HtmlCls.GetHtmlByCss(str, "prodImageCell").ToList()[1];
                //    str = Regex.Replace(str, "</?[0-9a-zA-Z]+[^>]*>", "");
                //    return Convert.ToDecimal(str.Replace("&yen;", "").Replace(",", ""));
                //}
            }
            catch
            {
                return 0;
            }
        }

        public override decimal GetMarketPrice()
        {
            try
            {
                GetHtml(SiteEncoding);
                var str = RegexHelper.Match(DocHtml, "<del[^>]*>￥\\s*([^<]+)</del>");
                return Convert.ToDecimal(str);
            }
            catch (Exception ex)
            {
                FileHelper.WriteException(ex);
                return 0;
            }
        }

        public override int GetStockCode()
        {
            try
            {
                var stockUrl = GetWebSiteInfo().BaseUrl +
                               "/Ajax/Product/AjaxProductInventory.aspx?stockId=54&productID={0}&isFirstLoad=false";
                var url = String.Format(stockUrl, GetProductId());
                using (var http = new HttpHelper(url, SiteEncoding))
                {
                    var str = http.GetHtml();
                    if (str.IndexOf("成都仓有货") >= 0)
                        return 1;
                    return 0;
                }
            }
            catch
            {
                return -1;
            }
        }

        private static string GetProductId()
        {
            try
            {
                GetHtml(SiteEncoding);
                return RegexHelper.Match(DocHtml, "<h1[^>]*id=[\"']pro(\\d+)[\"'][^>]*>[^<]+</h1>");
            }
            catch
            {
                return "";
            }
        }

        public override string GetProName()
        {
            try
            {
                GetHtml(SiteEncoding);
                var str = RegexHelper.Match(DocHtml, "<h1[^>]*>([^<]+)</h1>");
                return str;
            }
            catch (Exception ex)
            {
                FileHelper.WriteException(ex);
                return "";
            }
        }

        public override string GetProPic()
        {
            try
            {
                GetHtml(SiteEncoding);
                var str = HtmlCls.GetHtmlById(DocHtml, "midImg");
                str = HtmlCls.GetAttrValue(str, "src340");
                return Regex.Replace(str, "\\?.*$", "");
            }
            catch (Exception ex)
            {
                FileHelper.WriteException(ex);
                return "";
            }
        }

        public override WebSiteInfo GetWebSiteInfo()
        {
            return new WebSiteInfo
                       {
                           BaseUrl = "http://www.newegg.com.cn",
                           LogPic = "http://img01.taobaocdn.com/imgextra/etao/i1/T18OKyXiBEXXb1upjX.jpg_80x40.jpg",
                           WebName = "新蛋网"
                       };
        }
    }
}
