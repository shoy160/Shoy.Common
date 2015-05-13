using System.Linq;
using System.Text;
using Shoy.Utility;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;
using Shoy.Utility.Helper;

namespace Shoy.Spiders
{
    public class HtmlCls
    {
        public static string GetHtmlByUrl(string url, Encoding encoding = null, string cookie = "")
        {
            encoding = (encoding ?? Encoding.Default);
            using (var http = new HttpHelper(url, "", encoding, cookie, "", ""))
            {
                return http.GetHtml();
            }
        }

        /// <summary>
        /// 根据Id获取html内相关id标签下的html
        /// </summary>
        /// <param name="html"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetHtmlById(string html, string id)
        {
            const string pt =
                @"<([0-9a-zA-Z]+)[^>]*\bid=([""']){0}\2[^>]*>(?><\1[^>]*>(?<tag>)|</\1>(?<-tag>)|.)*?(?(tag)(?!))</\1>";
            const string pt1 = @"<([0-9a-zA-Z]+)[^>]*\bid=([""']){0}\2[^>]*/>";
            string reg = String.Format(pt, id);
            if (!Regex.IsMatch(html, reg))
                reg = String.Format(pt1, id);
            return Regex.Match(html, reg, RegexOptions.Singleline | RegexOptions.IgnoreCase).Value;
        }

        /// <summary>
        /// 根据Id获取html内相关css标签下的html
        /// </summary>
        /// <param name="html"></param>
        /// <param name="css"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetHtmlByCss(string html, string css)
        {
            const string pt =
                @"<([0-9a-zA-Z]+)[^>]*\bclass=(['""]?)(?<t>[^""'\s]*\s)*{0}(?<b>\s[^""'\s]*)*\2[^>]*>(?><\1[^>]*>(?<tag>)|</\1>(?<-tag>)|.)*?(?(tag)(?!))</\1>";
            const string pt1 = @"<([0-9a-zA-Z]+)[^>]*\bclass=(['""]?)(?<t>[^""'\s]*\s)*{0}(?<b>\s[^""'\s]*)*\2[^>]*/>";
            string reg = String.Format(pt, css);
            if (!Regex.IsMatch(html, reg))
                reg = String.Format(pt1, css);
            var ms = Regex.Matches(html, reg, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return (from Match match in ms select match.Value).ToList();
        }

        /// <summary>
        /// 根据Id获取html内相关css标签下的html
        /// </summary>
        /// <param name="html"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetHtmlByAttr(string html, string attr)
        {
            const string pt =
                @"<([0-9a-zA-Z]+)[^>]*\b{0}[^>]*>(?><\1[^>]*>(?<tag>)|</\1>(?<-tag>)|.)*?(?(tag)(?!))</\1>";
            const string pt1 = @"<([0-9a-zA-Z]+)[^>]*\b{0}[^>]*/>";
            string reg = String.Format(pt, attr);
            if (!Regex.IsMatch(html, reg))
                reg = String.Format(pt1, attr);
            var ms = Regex.Matches(html, reg, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return (from Match match in ms select match.Value).ToList();
        }

        /// <summary>
        /// 根据Id获取html内相关css标签下的html
        /// </summary>
        /// <param name="html"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public static string GetAttrValue(string html, string attr)
        {
            const string pt =
                @"<([0-9a-zA-Z]+)[^>]*\b{0}=([""'])(?<attr>[^""']*)\2[^>]*/?>";
            string reg = String.Format(pt, attr);
            var ms = Regex.Match(html, reg, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return ms.Groups["attr"].Value;
        }

        /// <summary>
        /// 根据url地址下载文件(主要是图片文件)
        /// </summary>
        /// <param name="url">文件地址</param>
        /// <param name="filename">保存的文件名(含路径)</param>
        /// <returns></returns>
        public static bool UrlDownLoadToFile(string url, string filename)
        {
            bool result;

            using (var http = new HttpHelper(url))
            {
                result = http.SaveFile(filename);
                Thread.Sleep(100);
            }
            return result;
        }
    }
}
