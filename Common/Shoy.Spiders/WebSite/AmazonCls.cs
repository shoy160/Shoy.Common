using Shoy.Utility;
using Shoy.Utility.Extend;
using Shoy.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Shoy.Spiders.WebSite
{
    /// <summary>
    /// 亚马逊卓越网常用类
    /// </summary>
    public sealed class AmazonCls:WebSiteFactory
    {
        private static readonly Encoding SiteEncoding = Encoding.UTF8;
        public override decimal GetPrice()
        {
            try
            {
                GetHtml(SiteEncoding);
                var str = RegexHelper.Match(DocHtml, "<b[^>]*class=[\"']priceLarge[\"'][^>]*>￥ ([^<]+)</b>");
                return Convert.ToDecimal(str.Replace(",", ""));
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
                GetHtml(SiteEncoding);
                var stock = HtmlCls.GetHtmlByCss(DocHtml, "availGreen").FirstOrDefault();
                if (stock.IsNullOrEmpty())
                    return 0;
                return 1;
            }
            catch(Exception ex)
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
                var str = HtmlCls.GetHtmlById(DocHtml, "btAsinTitle");
                str = Regex.Replace(str, "</?[0-9a-zA-Z]+[^>]*>", "");
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
                var str = HtmlCls.GetHtmlById(DocHtml, "prodImageCell");
                str = RegexHelper.Match(str, "\\s+src=[\"']([^\"'>]+)[\"']");
                return str;
            }
            catch (Exception ex)
            {
                FileHelper.WriteException(ex);
                return "";
            }
        }

        public override IEnumerable<string> GetUrlList(string listUrl)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> SearchWord(string word)
        {
            try
            {
                string searchUrl = GetWebSiteInfo().BaseUrl +
                                   "/s/ref=nb_sb_noss_1?__mk_zh_CN=%E4%BA%9A%E9%A9%AC%E9%80%8A%E7%BD%91%E7%AB%99&url=search-alias%3Daps&field-keywords={0}";
                var url = String.Format(searchUrl, Utils.UrlEncode(word, SiteEncoding));
                using (var http = new HttpHelper(url, SiteEncoding))
                {
                    var html = http.GetHtml();
                    if (!html.IsNullOrEmpty())
                        html = RegexHelper.ClearTrn(html);
                    var showList = HtmlCls.GetHtmlById(html, "atfResults") + HtmlCls.GetHtmlById(html, "btfResults");
                    var list =
                        HtmlCls.GetHtmlByCss(showList, "productImage").Select(
                            t => RegexHelper.Match(t, "<a[^>]*href=[\"']?([^\"'>]+)(#[^\"'>]*)?[\"']?[^>]*>")).Distinct().
                            ToList();
                    return list;
                }
            }
            catch(Exception ex)
            {
                FileHelper.WriteException(ex);
                return new List<string>();
            }
        }

        public override decimal GetMarketPrice()
        {
            try
            {
                GetHtml(SiteEncoding);
                var str = RegexHelper.Match(DocHtml, "<span[^>]*id=[\"']listPriceValue[\";][^>]*>￥ ([^<]+)</span>");
                return Convert.ToDecimal(str);
            }
            catch (Exception ex)
            {
                FileHelper.WriteException(ex);
                return 0;
            }
        }

        public override WebSiteInfo GetWebSiteInfo()
        {
            return new WebSiteInfo
                       {
                           BaseUrl = "http://www.amazon.cn",
                           LogPic = "http://img01.taobaocdn.com/imgextra/etao/i1/T12ZHGXX8jXXb1upjX.jpg_80x40.jpg",
                           WebName = "亚马逊卓越网"
                       };
        }
    }
}
