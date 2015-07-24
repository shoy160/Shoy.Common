using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Shoy.Utility;
using Shoy.Utility.Helper;

namespace Shoy.Spiders.WebSite
{
    /// <summary>
    /// 红孩子常用类
    /// </summary>
    public static class RedBabyCls
    {
        private const string RedBabyUrl = "http://www.redbaby.com.cn/";

        private static readonly string BasePath = Utils.GetMapPath("RedBay");

        /// <summary>
        /// 异常日志
        /// </summary>
        private static readonly string Err = BasePath + @"\Exception.log";

        /// <summary>
        /// 根据商品编号获取链接(合作方式变更)
        /// 不能提供分类编号，只能用强大的搜索功能了~
        /// </summary>
        /// <param name="num">商品编号</param>
        /// <returns></returns>
        public static string GetProUrlFromNum(string num)
        {
            string searchUrl = RedBabyUrl + "/search?keyword=" + num;
            string html = HtmlCls.GetHtmlByUrl(searchUrl, Encoding.UTF8);
            const string proReg =
                "<div[^>]*class=\"globalProductName\"[^>]*>\\s*<a[^>]*href=['|\"]([^'\"]*)['|\"][^>]*>";
            return Utils.GetAbsoluteUrl(RedBabyUrl, RegexHelper.Match(html, proReg));
        }

        /// <summary>
        /// 根据列表页获取产品链接
        /// </summary>
        /// <param name="listUrl">列表链接</param>
        /// <param name="deepth">扫描深度</param>
        /// <returns></returns>
        public static IEnumerable<string> GetUrlListFromList(string listUrl, int deepth)
        {
            var listArea = new List<string>();
            try
            {
                listUrl = listUrl.TrimEnd('/');
                var docHtml = HtmlCls.GetHtmlByUrl(listUrl, Encoding.UTF8);
                if (string.IsNullOrEmpty(listUrl))
                    return listArea;
                docHtml = RegexHelper.ClearTrn(docHtml);
                const string prolink =
                    "<div[^>]*class=\"globalProductName\">\\s*<a[^>]*href=['|\"]([^'\"]*)['|\"][^>]*>";
                var page = 1;
                listArea = RegexHelper.Matches(docHtml, prolink);

                //分页处理
                int count = listArea.Count();
                while (count < deepth)
                {
                    var pstr = RegexHelper.Match(listUrl, @"p(\d+)?$");
                    if (!string.IsNullOrEmpty(pstr))
                        page = Convert.ToInt32(pstr);
                    page++;
                    listUrl = Regex.Replace(listUrl, @"p(\d+)$", "") + "p" + page;
                    docHtml = HtmlCls.GetHtmlByUrl(listUrl, Encoding.UTF8);
                    if (string.IsNullOrEmpty(docHtml))
                        break;
                    listArea.AddRange(RegexHelper.Matches(docHtml, prolink));
                    count = listArea.Count();
                }

                listArea = listArea.Take(deepth).ToList();
            }
            catch (Exception ex)
            {
                FileHelper.WriteException(Err, ex);
            }
            return listArea;
        }

        /// <summary>
        /// 获取列表中的链接(保存在文件中)
        /// </summary>
        /// <param name="listUrl"></param>
        /// <param name="deepth"></param>
        /// <returns></returns>
        public static int GetUrlsFromList(string listUrl,int deepth)
        {
            var list = GetUrlListFromList(listUrl, deepth);
            return list.Count();
        }

        /// <summary>
        /// 获取价格信息[market_price]、[price]
        /// </summary>
        /// <param name="rNum">002站点产品Id</param>
        /// <returns></returns>
        //public static JsonCls.JsonObject GetPriceInfo(string rNum)
        //{
        //    const string baseUrl = "http://www.redbaby.com.cn/catalog/category/getPriceInfo?ids={0}";
        //    string url = String.Format(baseUrl, rNum);
        //    string info = HtmlCls.GetHtmlByUrl(url, Encoding.UTF8);
        //    if (!string.IsNullOrEmpty(info) && info != "[]")
        //    {
        //        JsonCls.JsonObject json = JsonCls.JsonConvert.DeserializeObject(info);
        //        //初始化
        //        JsonCls.JsonConvert.SetJson(new JsonCls.JsonObject());
        //        return (JsonCls.JsonObject)json[rNum];
        //    }
        //    return null;
        //}

        /// <summary>
        /// 判断四川是否有货
        /// </summary>
        /// <param name="proId">002站点产品ID</param>
        /// <returns></returns>
        public static bool CheckScStock(string proId)
        {
            const string baseUrl = "http://www.redbaby.com.cn/catalog/product/getStockInfo?id={0}";
            string url = String.Format(baseUrl, proId);
            string info = HtmlCls.GetHtmlByUrl(url, Encoding.UTF8);
            info = Encoding.UTF8.GetString(Encoding.Default.GetBytes(info));
            if (!string.IsNullOrEmpty(info))
            {
                if (info.IndexOf("有货") > -1 && info.IndexOf("四川") > -1)
                    return true;
                return false;
            }
            return false;
        }

        /// <summary>
        /// 获取产品名
        /// </summary>
        /// <param name="docHtml"></param>
        /// <returns></returns>
        public static string GetProNameFromHtml(string docHtml)
        {
            string name = RegexHelper.Match(docHtml, "<div[^>]*id=\"pName\"[^>]*>\\s*<h1>([^<]*)");
            name = Regex.Replace(name, @"红孩子母婴商城|红孩子", "本商场");
            return name;
        }

        /// <summary>
        /// 产品重量
        /// </summary>
        /// <param name="docHtml"></param>
        /// <returns></returns>
        public static int GetWeightFromHtml(string docHtml)
        {
            string weiStr = RegexHelper.Match(docHtml, "<span>规格：</span>(\\d*)G");
            return ConvertHelper.StrToInt(weiStr, 0);
        }

        /// <summary>
        /// 产品首图
        /// </summary>
        /// <param name="docHtml"></param>
        /// <returns></returns>
        public static string GetBigPicFromHtml(string docHtml)
        {
            return RegexHelper.Match(docHtml, "<div[^>]*id=\"jqzoomDiv\"[^>]*>\\s*<img[^>]*src=['\"]([^'\"]*)['\"][^>]*>");
        }

        /// <summary>
        /// 产品描述
        /// </summary>
        /// <param name="docHtml"></param>
        /// <returns></returns>
        public static string GetDescFromHtml(string docHtml)
        {
            string desc = HtmlCls.GetHtmlById(docHtml, "productDescription");
            if (!string.IsNullOrEmpty(desc))
            {
                desc = Regex.Replace(desc, @"红孩子母婴商城|红孩子", "本商场");
                desc = desc.Replace("id=\"productDescription\"", ""); //排除样式冲突
                //area = Regex.Replace(area, "class=['\"][^'\"]*['\"]", "");//排除样式冲突.终极
                desc = Regex.Replace(desc, "<a[^]*href=[\"|'][^'\"]*[\"'][^>]*>(.*?)</a>", ""); //排除a标签
                desc = Regex.Replace(desc, "<script[^>]*>[^<]*</script>", ""); //排除script标签
                desc = Regex.Replace(desc, "src\\d=", "src="); //显示src
            }
            return desc;
        }

        public static string GetStarsFromHtml(string docHtml)
        {
            string star = RegexHelper.Match(docHtml, "<span[^>]*class=['\"]scoreText font_yellow['\"][^>]*>([^<]*)</span>");
            return star;
        }

        public static string GetPlFromHtml(string docHtml)
        {
            string pl = RegexHelper.Match(docHtml, "已有(\\d+)人评价");
            return pl;
        }
    }
}
