using Shoy.Utility.Helper;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Shoy.Utility.Extend
{
    public interface IHtml : IExtension<string> { }

    public static class HtmlExtension
    {
        /// <summary>
        /// 检查Html标签闭合
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool CheckTags(this IHtml c)
        {
            var list = RegexHelper.Matches(c.GetValue(), "(<[^>]*[^/]>)");
            if (list.Count % 2 != 0)
                return false;
            var un = new List<string>();
            foreach (var i in list)
            {
                var tag = i.As<IRegex>().Match("<([a-z0-9A-Z]+)[^>]*>", 1);
                if (tag.IsNotNullOrEmpty())
                    un.Add(tag);
                else
                {
                    tag = i.As<IRegex>().Match("</([a-z0-9A-Z]+)>", 1);
                    if (tag.IsNotNullOrEmpty())
                    {
                        if (un[un.Count - 1] != tag)
                            return false;
                        un.RemoveAt(un.Count - 1);
                    }
                }
            }
            return un.Count == 0;
        }

        /// <summary>
        /// 清空Html标签
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string ClearHtml(this IHtml c)
        {
            var html = c.GetValue();
            if (html.IsNullOrEmpty())
                return string.Empty;
            html = HttpUtility.HtmlDecode(html);
            return html.IsNullOrEmpty() ? string.Empty : RegexHelper.ClearHtml(html);
        }

        /// <summary>
        /// 获取网页源码
        /// </summary>
        /// <param name="c"></param>
        /// <param name="method"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetHtml(this IHtml c, string method = null, string param = null)
        {
            return c.GetHtml(Encoding.UTF8, method, param);
        }

        public static string GetHtml(this IHtml c, Encoding encoding, string method = null, string param = null)
        {
            if (method.IsNullOrEmpty())
                method = "Get";
            using (var http = new HttpHelper(c.GetValue(), method, encoding, param))
            {
                return http.GetHtml();
            }
        }

        /// <summary>
        /// 获取ID
        /// </summary>
        /// <param name="c"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetHtmlById(this IHtml c, string id)
        {
            var html = c.GetValue();
            return RegexHelper.FindById(html, id);
        }

        /// <summary>
        /// 获取CSS
        /// </summary>
        /// <param name="c"></param>
        /// <param name="css"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetHtmlByCss(this IHtml c, string css)
        {
            var html = c.GetValue();
            return RegexHelper.FindByCss(html, css);
        }

        /// <summary>
        /// 获取Attr
        /// </summary>
        /// <param name="c"></param>
        /// <param name="attr"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetHtmlByAttr(this IHtml c, string attr)
        {
            var html = c.GetValue();
            return RegexHelper.FindByAttr(html, attr);
        }
    }
}
