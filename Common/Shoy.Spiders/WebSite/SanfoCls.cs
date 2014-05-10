using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Text.RegularExpressions;
using Shoy.Utility;

namespace Shoy.Spiders.WebSite
{
    /// <summary>
    /// 三夫户外常用类
    /// </summary>
    public static class SanfoCls
    {
        private const string SanfoUrl = "http://www.sanfo.com";

        /// <summary>
        /// 获取列表链接
        /// </summary>
        /// <param name="url">列表链接</param>
        /// <param name="deepth">扫描深度</param>
        /// <returns></returns>
        public static IEnumerable<string> GetLinksFormList(string url, int deepth)
        {
            var urls = GetUrlsFromHtml(url).ToList();
            int count = urls.Count();
            while (count < deepth && urls.Count() > 0)
            {
                url = Utils.GetNextPageUrl(url);
                var item = GetUrlsFromHtml(url);
                if (item.Count() == 0)
                    break;
                urls.AddRange(item);
                count = urls.Count();
            }
            urls = urls.Take(deepth).ToList();
            return urls;
        }

        /// <summary>
        /// 获取Id
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetSanfoId(string url)
        {
            const string regStr = "http://www.sanfo.com/hwyp/product/(\\d+).html";
            return Utils.GetRegStr(url, regStr);
        }

        /// <summary>
        /// 获取产品名
        /// </summary>
        /// <param name="docHtml"></param>
        /// <returns></returns>
        public static string GetProName(string docHtml)
        {
            const string regStr = "<dt[^>]*class=['\"]headline['\"][^>]*>([^<]+)</dt>";
            return Utils.GetRegStr(docHtml, regStr);
        }

        /// <summary>
        /// 获取首图
        /// </summary>
        /// <param name="docHtml"></param>
        /// <returns></returns>
        public static string GetBigPic(string docHtml)
        {
            const string regStr = "<dt[^>]*class=['\"]pic['\"][^>]*><img[^>]*src=['\"]([^'\"]+)['\"][^>]*></dt>";
            return Utils.GetRegStr(docHtml, regStr);
        }

        /// <summary>
        /// 获取市场价
        /// </summary>
        /// <param name="docHtml"></param>
        /// <returns></returns>
        public static decimal GetMarketerPrice(string docHtml)
        {
            const string regStr = "<li>零售价： <span class=\"font_grey_12_th\">￥([0-9,.]+)</span></li>";
            var p = Utils.GetRegStr(docHtml, regStr);
            if (string.IsNullOrEmpty(p))
                return 0;
            return Math.Round(Convert.ToDecimal(p), 1);
        }

        /// <summary>
        /// 获取网购价
        /// </summary>
        /// <param name="docHtml"></param>
        /// <returns></returns>
        public static decimal GetShopPrice(string docHtml)
        {
            const string regStr = "<li>网上价： <span class=\"font_red_16B\">￥([0-9,.]+)</span></li>";
            var p = Utils.GetRegStr(docHtml, regStr);
            if (string.IsNullOrEmpty(p))
                return 0;
            return Math.Round(Convert.ToDecimal(p), 1);
        }

        /// <summary>
        /// 获取品牌
        /// </summary>
        /// <param name="docHtml"></param>
        /// <returns></returns>
        public static string GetBrandName(string docHtml)
        {
            const string regStr = "<li>品牌：\\s*([^<]+)</li>";
            return Utils.GetRegStr(docHtml, regStr);
        }

        public static string GetProNum(string docHtml)
        {
            const string regStr = "<li>款号：\\s*([^<]+)</li>";
            return Utils.GetRegStr(docHtml, regStr);
        }

        /// <summary>
        /// 获取重量
        /// </summary>
        /// <param name="docHtml"></param>
        /// <returns></returns>
        public static int GetProWeigth(string docHtml)
        {
            const string regStr = "<li>重量： 约([^<]+)</li>";
            var w = Utils.GetRegStr(docHtml, regStr);
            if (!string.IsNullOrEmpty(w))
            {
                if (w.IndexOf("千克") >= 0)
                    return Utils.StrToInt(w.Replace("千克", "").Trim(), 0)*1000;
                return Utils.StrToInt(w.Replace("克", "").Trim(), 0);
            }
            return 0;
        }

        /// <summary>
        /// 获取描述
        /// </summary>
        /// <param name="docHtml"></param>
        /// <param name="sanId"></param>
        /// <returns></returns>
        public static string GetProDesc(string docHtml,string sanId)
        {
            var desc = "";
            var area = HtmlCls.GetHtmlByCss(docHtml, "detailBox");
            if (area.Count() > 0)
            {
                desc = area.Aggregate("", (current, t) => current + t);

                //图片居然单独一个请求

                const string imgUrl = "http://www.sanfo.com/shop/product.info.asp?command=findthumb&vid={0}";

                var imgs = HtmlCls.GetHtmlByUrl(String.Format(imgUrl, sanId), Encoding.UTF8);

                desc = Regex.Replace(desc, "<dt class=\"detailImg\" id=\"item_product_images\"></dt>",
                                     "<dt class=\"detailImg\" id=\"item_product_images\">" + imgs + "</dt>");

                //排除a标签
                desc = Regex.Replace(desc, "<a[^]*href=[\"|'][^'\"]*[\"'][^>]*>(.*?)</a>", "$1");
                //排除script标签
                desc = Regex.Replace(desc, "<script[^>]*>[^<]*</script>", "");
                //清除样式
                desc = Regex.Replace(desc, "(\\s*class=\"[^\"]+\")|(\\s*style=\"[^\"]+\")", "");

                //替换成绝对路径
                desc = Regex.Replace(desc, "src=\"(/[^\"]+)\"", "src=\"" + SanfoUrl + "$1\"");

                //替换三夫
                desc = Regex.Replace(desc, "(三夫(户外?)?)", "本商城");
            }
            return desc;
        }

        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <param name="docHtml"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAttrList(string docHtml)
        {
            const string regStr = "_colorSizeMap\\[\"([^\"]+)\"\\] = \"([^\"]+?)\";";
            var mts = (new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.Singleline)).Matches(docHtml);
            return (from Match mt in mts select mt.Groups[1].Value + ";" + mt.Groups[2].Value).ToList();
        }


        #region 私有方法

        private static IEnumerable<string> GetUrlsFromHtml(string url)
        {
            var urls = new List<string>();
            string docHtml = HtmlCls.GetHtmlByUrl(url);
            if (!string.IsNullOrEmpty(docHtml))
            {
                var listHtml = HtmlCls.GetHtmlById(docHtml, "Id_prodItemList");
                const string regStr = "<div[^>]*class=['\"]proPic['\"][^>]*><a[^>]*href=['\"]([^'\"]+)['\"][^>]*>";
                urls = Utils.GetRegHtmls(listHtml, regStr);
                urls = urls.Select(t => (t.StartsWith("/") ? SanfoUrl + t : t)).ToList();
            }
            return urls;
        }

        #endregion
    }
}
