
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Shoy.Utility.Extend;

namespace Shoy.Utility.Helper
{
    /// <summary>
    /// 正则辅助类
    /// </summary>
    public class RegexHelper
    {
        private const string BrRegex = @"(\r|\n)";
        private const string TrnRegex = @"(\r|\n|\t)";
        private const string DomainRegex = @".(\w+).(com.cn|net.cn|org.cn|edu.cn|com|net|org|cn|biz|info|cc|tv)";
        private const string IpRegex = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";

        private const string UrlRegex =
            @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$";

        private const string HtmlTagRegex = @"</?[0-9a-zA-Z]+[^>]*/?>";
        private const string FloatRegex = @"^([-]|[0-9])[0-9]*(\.\w*)?$";

        private const string HtmlFindByIdRegex =
            @"<([0-9a-zA-Z]+)[^>]*\bid=([""']){0}\2[^>]*>(?><\1[^>]*>(?<tag>)|</\1>(?<-tag>)|.)*?(?(tag)(?!))</\1>";

        private const string HtmlIdRegex = @"<([0-9a-zA-Z]+)[^>]*\bid=([""']){0}\2[^>]*/>";

        private const string HtmlFindByCssRegex =
            @"<([0-9a-zA-Z]+)[^>]*\bclass=(['""]?)(?<t>[^""'\s]*\s)*{0}(?<b>\s[^""'\s]*)*\2[^>]*>(?><\1[^>]*>(?<tag>)|</\1>(?<-tag>)|.)*?(?(tag)(?!))</\1>";
        private const string HtmlCssRegex = @"<([0-9a-zA-Z]+)[^>]*\bclass=(['""]?)(?<t>[^""'\s]*\s)*{0}(?<b>\s[^""'\s]*)*\2[^>]*/>";
        private const string HtmlFindByAttrRegex = @"<([0-9a-zA-Z]+)[^>]*\b{0}[^>]*>(?><\1[^>]*>(?<tag>)|</\1>(?<-tag>)|.)*?(?(tag)(?!))</\1>";
        private const string HtmlAttrRegex = @"<([0-9a-zA-Z]+)[^>]*\b{0}[^>]*/>";

        /// <summary>
        /// 获取单个正则匹配的字符
        /// </summary>
        /// <param name="regex">正则</param>
        /// <param name="str">字符串</param>
        /// <param name="group">组</param>
        /// <param name="ops">表达式选项</param>
        /// <returns></returns>
        public static string Match(string str, string regex, int group, RegexOptions ops)
        {
            var reg = new Regex(regex, ops);
            var m = reg.Match(str);
            return m.Groups[group].Value;
        }

        /// <summary>
        /// (简化)获取正则匹配的字符
        /// </summary>
        /// <param name="regex">正则</param>
        /// <param name="str">字符串</param>
        /// <param name="group">组</param>
        /// <returns></returns>
        public static string Match(string str, string regex, int group)
        {
            return Match(str, regex, group, RegexOptions.None);
        }

        /// <summary>
        /// (简化)获取正则匹配的字符
        /// </summary>
        /// <param name="regex">正则</param>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static string Match(string str, string regex)
        {
            return Match(str, regex, 1, RegexOptions.None);
        }

        public static List<string> Matches(string docHtml, string regStr, int index)
        {
            var mts = (new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.Singleline)).Matches(docHtml);
            return (from Match mt in mts select mt.Groups[index].Value).ToList();
        }

        public static List<string> Matches(string docHtml, string regStr, string name)
        {
            var mts = (new Regex(regStr, RegexOptions.IgnoreCase | RegexOptions.Singleline)).Matches(docHtml);
            return (from Match mt in mts select mt.Groups[name].Value).ToList();
        }

        /// <summary>
        /// 获取多个正则匹配值
        /// </summary>
        /// <param name="docHtml">字符源</param>
        /// <param name="regStr">正则</param>
        /// <returns></returns>
        public static List<string> Matches(string docHtml, string regStr)
        {
            return Matches(docHtml, regStr, 1);
        }

        /// <summary>
        /// 清除给定字符串中的回车及换行符
        /// </summary>
        /// <param name="str">要清除的字符串</param>
        /// <returns>清除后返回的字符串</returns>
        public static string ClearBr(string str)
        {
            return string.IsNullOrEmpty(str) ? str : Regex.Replace(str, BrRegex, string.Empty);
        }

        /// <summary>
        /// 清除\r \n \t
        /// </summary>
        /// <param name="str">str</param>
        /// <returns></returns>
        public static string ClearTrn(string str)
        {
            return string.IsNullOrEmpty(str) ? str : Regex.Replace(str, TrnRegex, string.Empty);
        }

        /// <summary>
        /// 获取域名
        /// </summary>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public static string GetDomain(string hostName)
        {
            var reg = new Regex(DomainRegex);
            if (hostName == null)
                hostName = string.Empty;
            return reg.Match(hostName).Value;
        }

        /// <summary>
        /// 是否是IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIp(string ip)
        {
            return Regex.IsMatch(ip, IpRegex);
        }

        ///<summary>
        /// 判断是否是url
        ///</summary>
        ///<param name="strUrl"></param>
        ///<returns></returns>
        public static bool IsUrl(string strUrl)
        {
            return Regex.IsMatch(strUrl, UrlRegex);
        }

        /// <summary>
        /// 清楚Html标签
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ClearHtml(string str)
        {
            return Regex.Replace(str, HtmlTagRegex, string.Empty);
        }

        /// <summary>
        /// 是否是浮点字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsFloat(string str)
        {
            return Regex.IsMatch(str, FloatRegex);
        }

        public static string FindById(string html, string id)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;
            html = ClearTrn(html);
            var reg = HtmlFindByIdRegex.FormatWith(id);
            if (Regex.IsMatch(html, reg))
                reg = HtmlIdRegex.FormatWith(id);
            return Match(html, reg, 0, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        public static IEnumerable<string> FindByCss(string html, string css)
        {
            if (string.IsNullOrEmpty(html))
                return new List<string>();
            html = ClearTrn(html);
            var reg = HtmlFindByCssRegex.FormatWith(css);
            if (Regex.IsMatch(html, reg))
                reg = HtmlCssRegex.FormatWith(css);
            return Matches(html, reg, 0);
        }

        public static IEnumerable<string> FindByAttr(string html, string attr)
        {
            if (string.IsNullOrEmpty(html))
                return new List<string>();
            html = ClearTrn(html);
            var reg = HtmlFindByAttrRegex.FormatWith(attr);
            if (Regex.IsMatch(html, reg))
                reg = HtmlAttrRegex.FormatWith(attr);
            return Matches(html, reg, 0);
        }
    }
}
