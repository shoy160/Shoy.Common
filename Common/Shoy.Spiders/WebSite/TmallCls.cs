using Shoy.Utility;
using Shoy.Utility.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shoy.Spiders.WebSite
{
    /// <summary>
    /// 天猫常用类
    /// </summary>
    public class TmallCls:WebSiteFactory
    {
        private const string BaseUrl = "http://www.tmall.com";
        #region dd
        
        private static bool _userProxy = false;

        /// <summary>
        /// 设置是否使用代理
        /// </summary>
        /// <param name="useproxy"></param>
        public static void SetUseProxy(bool useproxy)
        {
            _userProxy = useproxy;
        }

        public class TamllBase
        {
            public string Url { get; set; }
            public string Title { get; set; }
            public decimal Price { get; set; }
        }

        /// <summary>
        /// 获取列表页产品信息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="deepth"></param>
        /// <returns></returns>
        public static IEnumerable<TamllBase> GetTBaseFromList(string url, int deepth)
        {
            string next;
            var urls = GetTUrlsFromHtml(url, out next).ToList();
            int count = urls.Count();
            while (count < deepth && urls.Count() > 0)
            {
                url = next;
                var item = GetTUrlsFromHtml(url, out next);
                if (item.Count() == 0)
                    break;
                urls.AddRange(item);
                count = urls.Count();
            }
            urls = urls.Take(deepth).ToList();
            return urls;
        }

        /// <summary>
        /// 获取列表页产品链接
        /// </summary>
        /// <param name="url"></param>
        /// <param name="deepth"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetLinksFromList(string url, int deepth)
        {
            string next;
            var urls = GetUrlsFromHtml(url,out next).ToList();
            int count = urls.Count();
            while (count < deepth && urls.Count() > 0)
            {
                url = next;
                var item = GetUrlsFromHtml(url, out next);
                if (item.Count() == 0)
                    break;
                urls.AddRange(item);
                count = urls.Count();
            }
            urls = urls.Take(deepth).ToList();
            return urls;
        }

        public static string GetProDesc(string link)
        {
            //id=J_DivItemDesc
            var desc = "";
            var docHtml = HtmlCls.GetHtmlByUrl(link);
            var desurl = RegexHelper.Match(docHtml, "\"apiItemDesc\":\"([^\"]+?)\"");
            if (!string.IsNullOrEmpty(desurl))
            {
                desurl = desurl.Replace("\\", "");
                desc = HtmlCls.GetHtmlByUrl(desurl);
                desc = desc.Replace("var desc='", "").TrimEnd('\'');
            }
            return desc;
        }

        #region 私有

        private static IEnumerable<string> GetUrlsFromHtml(string url, out string next)
        {
            next = "";
            var urls = new List<string>();

            //tmall根据cookie不一样，前端显示也不一样。。
            const string cookie =
                "x=e%3D1%26p%3D*%26s%3D0%26c%3D0%26f%3D0%26g%3D0%26t%3D0; cna=qMo3B45XYmoCAct2enaIrZoT; t=9bfd6b376a1f1e450056f0e1b1c54240; tracknick=luoyong87610; mpp=t%3D0%26m%3D%26h%3D0%26l%3D0; uc1=x; cookie2=8eb29ff22cbe3bddcad34d264d01806f; passtime=1341280687588; isFirstOpen=false; x=e%3D1%26p%3D*%26s%3D0%26c%3D0%26f%3D0%26g%3D0%26t%3D0";
            string docHtml = HtmlCls.GetHtmlByUrl(url, Encoding.Default,cookie);
            if (!string.IsNullOrEmpty(docHtml))
            {
                docHtml = RegexHelper.ClearBr(docHtml);
                next =
                    Utils.UrlDecode(RegexHelper.Match(docHtml,
                                                    "<a[^>]*href=['\"]([^'\"\\s]+)['\"][^>]*class=['\"]ui-page-s-next['\"][^>]*>"));
                var listHtml = HtmlCls.GetHtmlById(docHtml, "J_itemList");
                const string regStr = "<a[^>]*href=['\"]([^'\"\\s]+?)['\"][^>]*class=['\"]product-Img['\"][^>]*>";
                //"<a[^>]*class=['\"]product-title['\"][^>]*href=['\"]([^'\"]+)['\"][^>]*>";
                urls = RegexHelper.Matches(listHtml, regStr);
                urls = urls.Select(t => (t.StartsWith("/") ? BaseUrl : "") + Utils.UrlDecode(t)).ToList();
            }
            return urls;
        }

        private static IEnumerable<TamllBase> GetTUrlsFromHtml(string url, out string next)
        {
            next = "";
            var urls = new List<TamllBase>();

            //tmall根据cookie不一样，前端显示也不一样。。
            const string cookie =
                "x=e%3D1%26p%3D*%26s%3D0%26c%3D0%26f%3D0%26g%3D0%26t%3D0; cna=qMo3B45XYmoCAct2enaIrZoT; t=9bfd6b376a1f1e450056f0e1b1c54240; tracknick=luoyong87610; mpp=t%3D0%26m%3D%26h%3D0%26l%3D0; uc1=x; cookie2=22291aea11e397a82512118642ac0abe; passtime=1341285069752; isFirstOpen=true";
            string docHtml = HtmlCls.GetHtmlByUrl(url, Encoding.Default, cookie);
            if (!string.IsNullOrEmpty(docHtml))
            {
                docHtml = RegexHelper.ClearBr(docHtml);
                next =
                    Utils.UrlDecode(RegexHelper.Match(docHtml,
                                                    "<a[^>]*href=['\"]([^'\"\\s]+)['\"][^>]*class=['\"]ui-page-s-next['\"][^>]*>"));
                var listHtml = HtmlCls.GetHtmlById(docHtml, "J_itemList");
                var list = HtmlCls.GetHtmlByCss(listHtml, "product");
                //1:url,2:name
                const string regStr =
                    "<a[^>]*href=['\"]([^'\"\\s]+?)['\"][^>]*class=['\"]product-title['\"][^>]*title=['\"]([^'\"]+?)['\"][^>]*>";
                //price
                const string priceReg =
                    "<span[^>]*class=['\"]product-normal['\"][^>]*title=['\"]([^'\"\\s]+)['\"][^>]*>";

                urls.AddRange(list.Select(item => new TamllBase
                                                      {
                                                          Url = Utils.UrlDecode(RegexHelper.Match(item, regStr, 1)),
                                                          Title = RegexHelper.Match(item, regStr, 2),
                                                          Price = Convert.ToDecimal(RegexHelper.Match(item, priceReg))
                                                      }));
            }
            return urls;
        }

        #endregion

        #endregion

        public override IEnumerable<string> GetUrlList(string listUrl)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> SearchWord(string word)
        {
            throw new NotImplementedException();
        }

        public override decimal GetPrice()
        {
            throw new NotImplementedException();
        }

        public override decimal GetMarketPrice()
        {
            throw new NotImplementedException();
        }

        public override int GetStockCode()
        {
            throw new NotImplementedException();
        }

        public override string GetProName()
        {
            throw new NotImplementedException();
        }

        public override string GetProPic()
        {
            throw new NotImplementedException();
        }

        public override WebSiteInfo GetWebSiteInfo()
        {
            return new WebSiteInfo
                       {
                           BaseUrl = "http://www.tmall.com",
                           LogPic = "http://img03.taobaocdn.com/tps/i3/T18s1zXnpWXXa.xMzo-80-20.png_80x40.jpg",
                           WebName = "天猫商城"
                       };
        }
    }
}
