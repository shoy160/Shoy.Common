using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;
using Shoy.Utility.Helper;

namespace Shoy.Utility.Extend
{
    public interface IHtml : IExtension<string> { }

    public static class HtmlExtension
    {
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
            return RegexHelper.FindById(html, id);
        }

        public static IEnumerable<string> GetHtmlByCss(this IHtml c, string css)
        {
            var html = c.GetValue();
            return RegexHelper.FindByCss(html, css);
        }

        public static IEnumerable<string> GetHtmlByAttr(this IHtml c, string attr)
        {
            var html = c.GetValue();
            return RegexHelper.FindByAttr(html, attr);
        }
    }
}
