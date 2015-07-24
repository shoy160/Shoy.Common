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
    /// 京东常用类
    /// 主要用于匹配html，获取相关信息。
    /// 1、列表页链接
    /// 2、成都地区库存
    /// 3、产品价格
    /// 4、产品编号
    /// 5、产品名称
    /// 6、产品首图
    /// 7、产品大图
    /// 8、产品描述
    /// </summary>
    public class Buy360Cls : WebSiteFactory
    {
        private static readonly Encoding SiteEncoding = Encoding.GetEncoding("gbk");
        private const string BaseUrl = "http://www.360buy.com";
        private static bool _useProxy = true;

        #region 公用方法

        /// <summary>
        /// 设置是否使用代理
        /// </summary>
        /// <param name="useProxy">是否使用</param>
        public static void SetUseProxy(bool useProxy)
        {
            _useProxy = useProxy;
        }

        /// <summary>
        /// 获取右则区域html
        /// </summary>
        /// <param name="docHtml"></param>
        /// <returns></returns>
        public static string GetRigthArea(string docHtml)
        {
            return HtmlCls.GetHtmlByCss(docHtml, "right-extra").FirstOrDefault();
        }

        /// <summary>
        /// 获取列表链接
        /// </summary>
        /// <param name="url">列表url</param>
        /// <param name="deepth">深度</param>
        /// <returns></returns>
        public static IEnumerable<string> GetUrlsListFromList(string url, int deepth)
        {
            var urls = new List<string>();
            try
            {
                urls = GetUrlsFromHtml(url).ToList();
                int count = urls.Count();
                while (count < deepth && urls.Count() > 0)
                {
                    url = GetNextPageUrl(url);
                    var item = GetUrlsFromHtml(url);
                    if (item.Count() == 0)
                        break;
                    urls.AddRange(item);
                    count = urls.Count();
                }
                urls = urls.Take(deepth).ToList();
            }
            catch (Exception ex)
            {
                FileHelper.WriteException(ex);
            }
            return urls;
        }

        /// <summary>
        /// 获取成都地区库存Code
        /// </summary>
        /// <param name="docHtml">页面html</param>
        /// <returns>-1,未知;0,缺货;1,有货</returns>
        public static int GetCdStockCode(string docHtml)
        {
            int code = -1;
            try
            {
                docHtml = RegexHelper.ClearBr(docHtml);
                string stockUrl = RegexHelper.Match(docHtml,
                                                  "(http://price.360buy.com/ows/stock/pshow-[a-zA-Z0-9]*.html)");
                if (string.IsNullOrEmpty(stockUrl))
                {
                    string skUid = RegexHelper.Match(docHtml, "wareinfo.*sid[^\"]*\"([0-9a-zA-Z]*)\"");

                    //省级库存
                    string purl =
                        "http://price.360buy.com/stocksoa/StockHandler.ashx?callback=getProvinceStockCallback&type=provincestock&skuid=" +
                        skUid + "&provinceid=22";

                    //市级库存
                    //string url =
                    //    "http://price.360buy.com/stocksoa/StockHandler.ashx?callback=getProvinceStockCallback&type=pcstock&skuid=" +
                    //    skUid + "&provinceid=22&cityid=1930";
                    string stockHtml = HtmlCls.GetHtmlByUrl(purl, SiteEncoding);
                    if (!string.IsNullOrEmpty(stockHtml))
                    {
                        string stockCode = RegexHelper.Match(stockHtml, "\"StockState\":(\\w+),");
                        code = (stockCode == "33" ? 1 : 0);
                    }
                }
                else
                {
                    string stockHtml = HtmlCls.GetHtmlByUrl(stockUrl, SiteEncoding);
                    //源代码中有库存连接<script type="text/javascript" src="http://price.360buy.com/ows/stock/pshow-0F5D9F92C79383CED35C7903D3927987.html"></script>
                    //内容如下:var stockdata = [{"Wid":183192,"Rid":6,"Stock":34,"Days":0,"Purchase":0,"IsPop":false},{"Wid":183192,"Rid":3,"Stock":33,"Days":0,"Purchase":0,"IsPop":false},{"Wid":183192,"Rid":10,"Stock":34,"Days":0,"Purchase":0,"IsPop":false},{"Wid":183192,"Rid":4,"Stock":33,"Days":0,"Purchase":0,"IsPop":false}];
                    //js主要代码如下:var orgname = { 6: "北京仓", 3: "上海仓", 10: "广州仓", 4: "成都仓", 5: "武汉仓", 7: "南京仓", 8: "济南仓", 9: "沈阳仓" };
                    //var stockstatus = { 33: "现货", 34: "无货", 36: "预定", 39: "在途", 0: "统计中" };
                    if (!string.IsNullOrEmpty(stockHtml))
                    {
                        string stockCode = RegexHelper.Match(stockHtml, "\"Rid\":4,\"Stock\":([^,]+),"); //仅仅对成都仓库
                        code = (stockCode == "33" ? 1 : 0);
                    }
                }
            }
            catch (Exception ex)
            {
                FileHelper.WriteException(ex);
            }
            return code;
        }

        /// <summary>
        /// 获取产品价格
        /// </summary>
        /// <param name="jdNum">jd产品编号</param>
        /// <returns></returns>
        public static decimal GetPriceFromCart(string jdNum)
        {
            const string addcart = "http://cart.360buy.com/cart/addSkuToCartAsync.action?pid={0}&ptype=1&pcount={1}&rd={2}";
            const string cart = "http://cart.360buy.com/cart/getCurrentCartNew.action?rd={0}";
            decimal price = 0;
            int count = 0;
            while ((price == 0 || price == 99M) && count < 2)
            {
                count++;
                try
                {
                    using (
                        var http = new HttpHelper(String.Format(addcart, jdNum, 1, Utils.GetJsonTime()), Encoding.UTF8))
                    {
                        http.GetCookie();
                        http.SetUrl(String.Format(cart, Utils.GetJsonTime()));
                        var html = http.GetHtml();
                        string sprice = RegexHelper.Match(html, "\"finalPrice\":\"([^\"]+)\"");
                        price = Convert.ToDecimal(sprice.Replace(",", ""));
                    }
                }
                catch
                {
                    price = 0;
                }
            }

            #region 旧方式

            //string url = String.Format(cart, (new Random().NextDouble()));
            //int count = 0;
            //while ((price == 0 || price == 99M) && count < 2)
            //{
            //    count++;
            //    try
            //    {
            //        string doc = HtmlCls.GetHtmlByUrl(url,
            //                                          "cart-main=\"{&spg&:{&ps&:[{&i&:" + jdNum +
            //                                          "$&n&:1$&at&:0$&ct&:1}]}$&y&:{}$&by&:false$&rs&:[" + jdNum +
            //                                          "]$&tm&:&" + Utils.GetTimeNow() +"&$&st&:&g&}\"", Encoding.UTF8, _useProxy);
            //        string sprice = RegexHelper.Match(doc, "\"finalPrice\":\"([^\"]+)\"");
            //        price = Convert.ToDecimal(sprice.Replace(",", ""));
            //    }
            //    catch (Exception)
            //    {
            //        price = 0;
            //    }
            //}

            #endregion

            return price;
        }

        public static decimal GetMarketerPrice(string docHtml)
        {
            decimal mprice;
            try
            {
                string str = HtmlCls.GetHtmlById(docHtml, "summary");
                str = RegexHelper.Match(str, "<del>￥([^<]+)</del>");
                if (string.IsNullOrEmpty(str))
                {
                    str = HtmlCls.GetHtmlById(docHtml, "book-price");
                    str = RegexHelper.Match(str, "<del>￥([^<]+)</del>");
                }
                mprice = decimal.Parse(str.Replace(",", ""));
            }
            catch (Exception)
            {
                mprice = 0;
            }
            return mprice > 100 ? Math.Round(mprice, 0) : Math.Round(mprice, 1);
        }

        /// <summary>
        /// 从链接中获取产品编号
        /// </summary>
        /// <param name="url">产品链接</param>
        /// <returns></returns>
        public static string GetProNumFromUrl(string url)
        {
            return RegexHelper.Match(url, "^(http://)?(book|www).360buy.com/(product/)?(\\d+).html$", 4);
        }

        public static int GetProWeight(string docHtml)
        {
            var dws = RegexHelper.Match(docHtml, @"<li>商品毛重：([^<]+)</li>");
            float weigth = ConvertHelper.StrToFloat(Regex.Replace(dws, @"k?g", "", RegexOptions.IgnoreCase), 0);
            if (dws.IndexOf("kg", StringComparison.Ordinal) >= 0)
            {
                weigth = weigth * 1000;
            }
            return (int)Math.Round(weigth, 0);
        }

        /// <summary>
        /// 获取产品名
        /// </summary>
        /// <param name="docHtml">html</param>
        /// <returns></returns>
        public static string GetProName(string docHtml)
        {
            string area = HtmlCls.GetHtmlById(docHtml, "name");
            return RegexHelper.Match(area, "<h1>([^<]*)<");
        }

        public static string GetBrandName(string docHtml)
        {
            string area = HtmlCls.GetHtmlById(docHtml, "i-detail");
            if (!string.IsNullOrEmpty(area))
                return RegexHelper.Match(area, "<li[^>]*>生产厂家：<a[^>]*brand[^>]*>([^<]+)</a>").Trim();
            return "";
        }

        public static string GetPackingList(string docHtml)
        {
            string area = HtmlCls.GetHtmlById(docHtml, "bzqd");
            if (!string.IsNullOrEmpty(area))
                return RegexHelper.Match(area, "<[^>]*>([^<]+)<[^>]*>").Trim();
            return "";
        }

        public static string GetAftersaleService(string docHtml)
        {
            string area = HtmlCls.GetHtmlById(docHtml, "detail");
            if (!string.IsNullOrEmpty(area))
            {
                var list = HtmlCls.GetHtmlByCss(area, "mc tabcon hide").ToList();
                if (list.Count() >= 3)
                    return RegexHelper.Match(list[2], "<[^>]*>([^<]+)<[^>]*>").Trim();
                return "";
            }
            return "";
        }

        /// <summary>
        /// 获取产品展示页版本
        /// </summary>
        /// <param name="proUrl">产品链接</param>
        /// <returns></returns>
        public static int GetProVersion(string proUrl)
        {
            int version = 0;
            if (Regex.IsMatch(proUrl, "^(http://)book.360buy.com/\\d+.html$"))
                version = 1;
            return version;
        }

        /// <summary>
        /// 获取首图
        /// </summary>
        /// <param name="docHtml"></param>
        /// <returns></returns>
        public static string GetBigPic(string docHtml)
        {
            string area = HtmlCls.GetHtmlById(docHtml, "spec-n1");
            string src = RegexHelper.Match(area, "<img[^>]*src=['\"]([^'\"]*)['\"][^>]*>");
            return src;
        }

        /// <summary>
        /// 获取大图html
        /// </summary>
        /// <param name="jdNum"></param>
        /// <returns></returns>
        public static string GetProBigPics(string jdNum)
        {
            string picArea = "";
            try
            {
                const string bigUrl = BaseUrl + "/bigimage.aspx?id={0}";
                string url = String.Format(bigUrl, jdNum);
                string picHtml = HtmlCls.GetHtmlByUrl(url);
                if (!string.IsNullOrEmpty(picHtml))
                {
                    string biger = HtmlCls.GetHtmlByCss(picHtml, "right").FirstOrDefault();
                    var bigList = RegexHelper.Matches(biger, "http://img10.360buyimg.com/n5([^'\"]*)");
                    if (bigList.Count() > 0)
                    {
                        picArea =
                            "<table width=\"750\" align=\"center\" border=\"0\" cellSpacing=\"0\" cellPadding=\"0\">";
                        picArea =
                            bigList.Aggregate(picArea,
                                              (current, s) =>
                                              current + "<tr><td><img src=\"http://img10.360buyimg.com/n0" + s +
                                              "\" /></td></tr>");
                        picArea += "</table>";
                    }
                }
            }
            catch (Exception ex)
            {
                FileHelper.WriteException(ex);
            }
            return picArea;
        }

        /// <summary>
        /// 获取产品描述
        /// </summary>
        /// <param name="docHtml">html文件</param>
        /// <param name="version">区分图书1和其他0</param>
        /// <returns></returns>
        public static string GetProDesc(string docHtml, int version)
        {
            string area = "";
            try
            {
                docHtml = RegexHelper.ClearTrn(docHtml);

                if (version == 0)
                {
                    //增加 规格描述 -2012-02-29 shy
                    string pt = HtmlCls.GetHtmlByCss(docHtml, "Ptable").FirstOrDefault();
                    if (!string.IsNullOrEmpty(pt))
                    {
                        area += pt;
                    }

                    area += HtmlCls.GetHtmlByCss(docHtml, "content").FirstOrDefault();

                }
                else
                {
                    //图书类
                    var list = HtmlCls.GetHtmlByCss(docHtml, "m m1");
                    area = list.Aggregate(area, (current, s) => current + s);
                    string listH = HtmlCls.GetHtmlByCss(area, "list-h").FirstOrDefault();
                    if (!string.IsNullOrEmpty(listH))
                        area = area.Replace(listH, "");
                    //去除【该作者其它作品】区域
                    listH = HtmlCls.GetHtmlById(area, "related-works");
                    if (!string.IsNullOrEmpty(listH))
                        area = area.Replace(listH, "");
                    string sum = HtmlCls.GetHtmlById(docHtml, "summary"); //加入图书信息
                    var sumList = RegexHelper.Matches(sum, "<li[^>]*>(.*?)</li>").Take(9);
                    sum = sumList.Aggregate("", (current, s) => current + "<div>" + s + "</div>");
                    sum = Regex.Replace(sum, "<a[^>]*href=[\"']([^'\"]+?)[\"'][^>]*>(.*?)</a>", "$2"); //排除a标签

                    area = sum + area;
                }
                //排除授权html
                string red = HtmlCls.GetHtmlByAttr(area, "color=\"red\"").FirstOrDefault();
                if (!string.IsNullOrEmpty(red))
                    area = area.Replace(red, "");
                area = area.Replace("class=\"content\"", "");//排除样式冲突
                //area = Regex.Replace(area, "class=['\"][^'\"]*['\"]", "");//排除样式冲突.终极
                area = Regex.Replace(area, "<a[^>]*href=[\"']([^'\"]+?)[\"'][^>]*>(.*?)</a>", "$2"); //排除a标签
                area = Regex.Replace(area, "\\sstyle=(['\"])[^'\"]+?\\1", "");//排除样式
                area = Regex.Replace(area, "<script[^>]*>(.*?)</script>", ""); //排除script标签
                area = Regex.Replace(area, "src\\d=", "src="); //显示src
                area = Regex.Replace(area, "京东商城|京东", "本商城");//排除京东字样
            }
            catch (Exception ex)
            {
                FileHelper.WriteException(ex);
            }
            return area;
        }

        #endregion

        #region 私有方法

        private static string GetNextPageUrl(string url)
        {
            int ver = GetListUrlVersion(url);
            string urlFormat =
                "^(" + BaseUrl +
                "/products/\\d+-\\d+-\\d+-\\d+-\\d+-\\d+-\\d+-\\d+-\\d+-\\d+-\\d+-\\d+-)(\\d+)(.*\\.html)$";
            if (ver == 2)
                urlFormat = "^(" + BaseUrl + "/booktop-\\d+-\\d+-)(\\d+)(.html)$";
            int currPage = ConvertHelper.StrToInt(RegexHelper.Match(url, urlFormat, 2), 1);
            currPage++;
            return Regex.Replace(url, urlFormat, "$1 " + currPage + "$3").Replace(" ", "");
        }

        private static IEnumerable<string> GetUrlsFromHtml(string url)
        {
            var urlList = new List<string>();
            //int ver = GetListUrlVersion(url);
            int ver = 0;//有些图书html样式居然不一样~
            string docHtml = HtmlCls.GetHtmlByUrl(url, SiteEncoding); //HtmlCls.GetHtmlByUrl(url, _useProxy);)
            if (!string.IsNullOrEmpty(docHtml))
            {
                docHtml = RegexHelper.ClearTrn(docHtml);
                var cssName = "p-img";
                var listHtml = HtmlCls.GetHtmlById(docHtml, "plist");
                if (listHtml.IsNullOrEmpty())
                {
                    cssName = "i-img";
                    listHtml = HtmlCls.GetHtmlByCss(docHtml, "list-h").FirstOrDefault();
                }
                var list =
                    HtmlCls.GetHtmlByCss(listHtml, cssName).Select(
                        t => RegexHelper.Match(t, "<a[^>]*href=[\"']?([^\"'>#]+)(#[^\"'>]*)?[\"']?[^>]*>")).Distinct().
                        ToList();
                return list;
            }
            return urlList;
        }

        /// <summary>
        /// 获取列表链接版本
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static int GetListUrlVersion(string url)
        {
            int version = 0; //html版本号
            string tag = RegexHelper.Match(url, "^" + BaseUrl + "/([^.]*).html$");
            if (tag.StartsWith("products/1713"))
            {
                //图书列表
                version = 1;
            }
            else if (tag.StartsWith("booktop"))
            {
                //图书排行榜
                version = 2;
            }
            return version;
        }

        #endregion

        public override IEnumerable<string> GetUrlList(string listUrl)
        {
            return GetUrlsListFromList(listUrl, 130).ToList();
        }

        public override IEnumerable<string> SearchWord(string word)
        {
            const string searchUrl = "http://search.360buy.com/Search?keyword={0}&enc=utf-8&area=22";
            var url = String.Format(searchUrl, Utils.UrlEncode(word, SiteEncoding));
            return GetUrlsFromHtml(url).ToList();
        }

        public override decimal GetPrice()
        {
            var jdNum = GetProNumFromUrl(ProLink);
            return GetPriceFromCart(jdNum);
        }

        public override decimal GetMarketPrice()
        {
            decimal mprice;
            try
            {
                GetHtml(SiteEncoding);
                string str = HtmlCls.GetHtmlById(DocHtml, "summary");
                str = RegexHelper.Match(str, "<del>￥([^<]+)</del>");
                if (string.IsNullOrEmpty(str))
                {
                    str = HtmlCls.GetHtmlById(DocHtml, "book-price");
                    str = RegexHelper.Match(str, "<del>￥([^<]+)</del>");
                }
                mprice = decimal.Parse(str.Replace(",", ""));
            }
            catch (Exception)
            {
                mprice = 0;
            }
            return mprice > 100 ? Math.Round(mprice, 0) : Math.Round(mprice, 1);
        }

        public override int GetStockCode()
        {
            int code = -1;
            try
            {
                GetHtml(SiteEncoding);
                string stockUrl = RegexHelper.Match(DocHtml,
                                                  "(http://price.360buy.com/ows/stock/pshow-[a-zA-Z0-9]*.html)");
                if (string.IsNullOrEmpty(stockUrl))
                {
                    string skUid = RegexHelper.Match(DocHtml, "[\"']?((skuidkey)|(sid))[\"']?:\\s*[\"']([0-9a-zA-Z]+)[\"']", 4);
                    var type = RegexHelper.Match(DocHtml, "type:\\s*(\\d+)");
                    var sUrl = "";
                    if (type == "1")
                    {
                        //市级库存
                        sUrl =
                            "http://price.360buy.com/stocksoa/StockHandler.ashx?callback=getProvinceStockCallback&type=ststock&skuid=" +
                            skUid + "&provinceid=22&cityid=1930&areaid=1945";
                    }
                    else
                    {
                        sUrl =
                            "http://st.3.cn/gsi.html?callback=gSC&type=provincestock&skuid=" + skUid + "&provinceid=22";
                        //省级库存
                        //sUrl =
                        //    "http://price.360buy.com/stocksoa/StockHandler.ashx?callback=getProvinceStockCallback&type=provincestock&skuid=" +
                        //    skUid + "&provinceid=22";
                    }

                    string stockHtml = HtmlCls.GetHtmlByUrl(sUrl, SiteEncoding);
                    if (!string.IsNullOrEmpty(stockHtml))
                    {
                        var stockStr = RegexHelper.Match(stockHtml, "\"StockStateName\":\"([^\"]+)\"", 1);
                        if (stockStr == "有货")
                            return 1;
                        var scode =
                            (RegexHelper.Match(stockHtml,
                                             type == "1" ? "\"S\":\"1-(\\d+)-1-0-0\"" : "\"StockState\":(\\w+),"));
                        code = (scode == "33" ? 1 : 0);
                    }
                }
                else
                {
                    string stockHtml = HtmlCls.GetHtmlByUrl(stockUrl, SiteEncoding);
                    //源代码中有库存连接<script type="text/javascript" src="http://price.360buy.com/ows/stock/pshow-0F5D9F92C79383CED35C7903D3927987.html"></script>
                    //内容如下:var stockdata = [{"Wid":183192,"Rid":6,"Stock":34,"Days":0,"Purchase":0,"IsPop":false},{"Wid":183192,"Rid":3,"Stock":33,"Days":0,"Purchase":0,"IsPop":false},{"Wid":183192,"Rid":10,"Stock":34,"Days":0,"Purchase":0,"IsPop":false},{"Wid":183192,"Rid":4,"Stock":33,"Days":0,"Purchase":0,"IsPop":false}];
                    //js主要代码如下:var orgname = { 6: "北京仓", 3: "上海仓", 10: "广州仓", 4: "成都仓", 5: "武汉仓", 7: "南京仓", 8: "济南仓", 9: "沈阳仓" };
                    //var stockstatus = { 33: "现货", 34: "无货", 36: "预定", 39: "在途", 0: "统计中" };
                    if (!string.IsNullOrEmpty(stockHtml))
                    {
                        string stockCode = RegexHelper.Match(stockHtml, "\"Rid\":4,\"Stock\":([^,]+),"); //仅仅对成都仓库
                        code = (stockCode == "33" ? 1 : 0);
                    }
                }
            }
            catch (Exception ex)
            {
                FileHelper.WriteException(ex);
            }
            return code;
        }

        public override string GetProName()
        {
            GetHtml(SiteEncoding);
            string area = HtmlCls.GetHtmlById(DocHtml, "name");
            return RegexHelper.Match(area, "<h1>([^<]*)<");
        }

        public override string GetProPic()
        {
            GetHtml(SiteEncoding);
            string area = HtmlCls.GetHtmlById(DocHtml, "spec-n1");
            string src = RegexHelper.Match(area, "<img[^>]*src=['\"]([^'\"]*)['\"][^>]*>");
            return src;
        }

        public override WebSiteInfo GetWebSiteInfo()
        {
            return new WebSiteInfo
                       {
                           BaseUrl = "",
                           LogPic = "http://img01.taobaocdn.com/imgextra/etao/i1/T15ZhLXgxzXXb1upjX.jpg_80x40.jpg",
                           WebName = "京东商城"
                       };
        }
    }
}
