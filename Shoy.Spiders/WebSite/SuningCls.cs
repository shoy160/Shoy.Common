using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Shoy.Utility;
using Shoy.Utility.Extend;
using System.Collections.Generic;

namespace Shoy.Spiders.WebSite
{
    /// <summary>
    /// 苏宁易购常用类
    /// </summary>
    public sealed class SuningCls:WebSiteFactory
    {
        //http://www.suning.com/emall/prd_10052_10051_-7_1939595_.html
        private static string _stockUrl;
        private static readonly Encoding SiteEncoding = Encoding.UTF8;

        private static decimal _mkPrice;

        public override decimal GetPrice()
        {
            try
            {
                string priceUrl = GetWebSiteInfo().BaseUrl + "/emall/SNProductStatusView?storeId={0}&catalogId={1}&productId={2}&cityId=9265&_={3}";
                var reg = Regex.Match(ProLink, GetWebSiteInfo().BaseUrl + "/emall/prd_(\\d+)_(\\d+)_-\\d_(\\d+)_.html");
                if (!reg.Success)
                    return 0;
                var url = String.Format(priceUrl, reg.Groups[1].Value, reg.Groups[2].Value, reg.Groups[3].Value,
                                        Utils.GetJsonTime());
                using (var http = new HttpHelper(url, "", SiteEncoding, "", ProLink, ""))
                {
                    var html = http.GetHtml();
                    var str = Utils.GetRegStr(html, "\"promotionPrice\":\\s*\"([^\"]+)\"");
                    var mStr = Utils.GetRegStr(html, "\"itemPrice\":\\s*\"([^\"]+)\"");
                    _mkPrice = (mStr.IsNullOrEmpty() ? 0M : Convert.ToDecimal(mStr.Replace(",", "")));
                    var salesOrg = Utils.GetRegStr(html, "\"salesOrg\":\\s*\"([^\"]+)\"");
                    var deptNo = Utils.GetRegStr(html, "\"deptNo\":\\s*\"([^\"]+)\"");
                    var vendor = Utils.GetRegStr(html, "\"vendor\":\\s*\"([^\"]+)\"");
                    _stockUrl = GetWebSiteInfo().BaseUrl +
                                "/emall/SNProductSaleView?storeId={0}&catalogId={1}&productId={2}&salesOrg={3}&&deptNo={4}&vendor={5}&cityId=9265&_={6}";
                    _stockUrl = String.Format(_stockUrl, reg.Groups[1].Value, reg.Groups[2].Value, reg.Groups[3].Value,
                                              salesOrg, deptNo, vendor, Utils.GetJsonTime());
                    return Convert.ToDecimal(str.Replace(",", ""));
                }
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
                return 0;
            }
        }

        public override int GetStockCode()
        {
            try
            {
                if (_stockUrl.IsNullOrEmpty())
                    GetPrice();
                using (var http = new HttpHelper(_stockUrl, "", SiteEncoding, "", ProLink, ""))
                {
                    var html = http.GetHtml();
                    var status = Utils.GetRegStr(html, "\"productStatus\":\\s*\"([^\"]+)\"");
                    var offset = Utils.GetRegStr(html, "\"shipOffset\":\\s*\"([^\"]+)\"");
                    return Utils.StrToInt(status, 0)*10 + Utils.StrToInt(offset, 0);
                }
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
                return -1;
            }
        }

        public override string GetProName()
        {
            try
            {
                GetHtml();
                var name = Utils.GetRegStr(DocHtml, "<h1[^>]*>[\\w\\W]*?<span>([^<]+)<");
                if (name.IsNullOrEmpty())
                    name = Utils.GetRegStr(DocHtml, "<h3[^>]*class=[\"']title[\"'][^>]*>[\\w\\W]*?<span>([^<]+)<");
                return name;
            }
            catch(Exception)
            {
                return "";
            }
        }

        public override string GetProPic()
        {
            try
            {
                GetHtml();
                var picArea = Utils.GetRegStr(DocHtml,
                                              "(http://image\\d?.suning.cn/content/catentries/\\d+/\\d+/fullimage/\\d+_1f?.jpg)");
                return picArea;
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
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
                    html = Utils.ClearTrn(html);
                    var showList = HtmlCls.GetHtmlById(html, "proShow");
                    var linkReg = "<a[^>]*href=[\"']?(" + GetWebSiteInfo().BaseUrl +
                                  "/emall/prd_\\d+_\\d+_-\\d+_\\d+_.html)[\"']?[^>]*>";
                    var list = Utils.GetRegHtmls(showList, linkReg).Distinct().ToList();
                    return list;
                }
            }
            catch(Exception ex)
            {
                Utils.WriteException(ex);
                return new List<string>();
            }
        }

        public override IEnumerable<string> SearchWord(string word)
        {
            try
            {
                const string searchUrl = "http://search.suning.com/emall/search.do?keyword={0}&cityId=9265";
                var url = String.Format(searchUrl, Utils.UrlEncode(word, SiteEncoding));
                return GetUrlList(url);
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
                return new List<string>();
            }
        }

        /// <summary>
        /// 先用GetPrice
        /// </summary>
        /// <returns></returns>
        public override decimal GetMarketPrice()
        {
            if (_stockUrl.IsNullOrEmpty())
                GetPrice();
            return _mkPrice;
        }

        public override WebSiteInfo GetWebSiteInfo()
        {
            return new WebSiteInfo
                       {
                           BaseUrl = "http://www.suning.com",
                           LogPic = "http://img01.taobaocdn.com/imgextra/etao/i1/T1YJ92XgFhXXb1upjX.jpg_80x40.jpg",
                           WebName = "苏宁易购"
                       };
        }
    }
}
