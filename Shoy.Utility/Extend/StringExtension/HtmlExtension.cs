using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;

namespace Shoy.Utility.Extend
{
    public interface IHtml:IExtension<string>{}

    public static class HtmlExtension
    {
        public static bool CheckTags(this IHtml c)
        {
            var list = Utils.GetRegHtmls(c.GetValue(), "(<[^>]*[^/]>)");
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

        public static string ClearHtml(this IHtml c)
        {
            var str = c.GetValue();
            if (str.IsNullOrEmpty())
                return "";
            str = HttpUtility.HtmlDecode(str);
            if (str.IsNullOrEmpty())
                return "";
            return str.As<IRegex>().Replace("<[^>]*>", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        }

        public static string GetHtml(this IHtml c, string method, string param, Encoding encoding)
        {
            if (method.IsNullOrEmpty())
                method = "Get";
            using (var http = new HttpHelper(c.GetValue(), method, encoding, param))
            {
                return http.GetHtml();
            }
        }

        public static string GetHtml(this IHtml c, Encoding encoding)
        {
            using (var http = new HttpHelper(c.GetValue(), encoding))
            {
                return http.GetHtml();
            }
        }

        public static string GetHtml(this IHtml c)
        {
            return c.GetHtml(Encoding.UTF8);
        }

        public static string GetHtmlById(this IHtml c, string id)
        {
            var html = c.GetValue();
            if (html.IsNullOrEmpty())
                return "";
            html = Utils.ClearTrn(html);
            const string pt =
                @"<([0-9a-zA-Z]+)[^>]*\bid=([""']){0}\2[^>]*>(?><\1[^>]*>(?<tag>)|</\1>(?<-tag>)|.)*?(?(tag)(?!))</\1>";
            const string pt1 = @"<([0-9a-zA-Z]+)[^>]*\bid=([""']){0}\2[^>]*/>";
            string reg = pt.FormatWith(id);
            if (html.As<IRegex>().IsMatch(pt))
                reg = pt1.FormatWith(id);
            return html.As<IRegex>().Match(reg, 0, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        public static IEnumerable<string> GetHtmlByCss(this IHtml c, string css)
        {
            var html = c.GetValue();
            if (html.IsNullOrEmpty())
                return new List<string>();
            html = Utils.ClearTrn(html);
            const string pt =
                @"<([0-9a-zA-Z]+)[^>]*\bclass=(['""]?)(?<t>[^""'\s]*\s)*{0}(?<b>\s[^""'\s]*)*\2[^>]*>(?><\1[^>]*>(?<tag>)|</\1>(?<-tag>)|.)*?(?(tag)(?!))</\1>";
            const string pt1 =
                @"<([0-9a-zA-Z]+)[^>]*\bclass=(['""]?)(?<t>[^""'\s]*\s)*{0}(?<b>\s[^""'\s]*)*\2[^>]*/>";
            string reg = pt.FormatWith(css);
            if (!html.As<IRegex>().IsMatch(reg))
                reg = pt1.FormatWith(css);
            return html.As<IRegex>().Matches(reg, 0, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        public static IEnumerable<string> GetHtmlByAttr(this IHtml c, string attr)
        {
            var html = c.GetValue();
            if (html.IsNullOrEmpty())
                return new List<string>();
            html = Utils.ClearTrn(html);
            const string pt =
                @"<([0-9a-zA-Z]+)[^>]*\b{0}[^>]*>(?><\1[^>]*>(?<tag>)|</\1>(?<-tag>)|.)*?(?(tag)(?!))</\1>";
            const string pt1 = @"<([0-9a-zA-Z]+)[^>]*\b{0}[^>]*/>";
            string reg = pt.FormatWith(attr);
            if (!html.As<IRegex>().IsMatch(reg))
                reg = pt1.FormatWith(attr);
            return html.As<IRegex>().Matches(reg, 0, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }
    }
}
