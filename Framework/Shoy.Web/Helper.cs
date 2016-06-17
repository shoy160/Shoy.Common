
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Shoy.Core;
using Shoy.Utility.Extend;

namespace Shoy.Web
{
    public static class Helper
    {
        #region 私有方法
        private const string JsStr = "<script src=\"{0}{1}\" type=\"text/javascript\"></script>";
        private const string CssStr = "<link href=\"{0}{1}\" rel=\"stylesheet\" media=\"all\" />";

        private static string FormatPath(string path)
        {
            string ext;
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(ext = Path.GetExtension(path)))
                return path;
            if (path.IndexOf("min" + ext, StringComparison.Ordinal) > 0)
                return path;
            if (Consts.Config.IsOnline)
                return path.Replace(ext, ".min" + ext);
            if (path.StartsWith("v3/"))
                return path.Replace("v3/", "v3/source/");
            return string.Format("/source/{0}", path.TrimStart('/'));
        }

        private static string BaseLink(string ext)
        {
            switch (ext)
            {
                case ".js":
                    return JsStr;
                case ".css":
                    return CssStr;
                default:
                    return string.Empty;
            }
        }
        #endregion

        public static HtmlString CombineLink(this HtmlHelper htmlHelper, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return new HtmlString(string.Empty);
            return htmlHelper.CombineLink(path.Split(','));
        }

        public static HtmlString CombineLink(this HtmlHelper htmlHelper, params string[] pathList)
        {
            if (pathList.IsNullOrEmpty())
                return new HtmlString(string.Empty);
            var ext = Path.GetExtension(pathList.First());
            string link = BaseLink(ext), paths = Consts.Config.StaticSite + "/";
            if (Consts.Config.IsOnline)
            {
                paths =
                    (pathList.Aggregate(paths + "??",
                        (current, item) => current + (FormatPath(item).TrimStart('/') + ",")));
                return new HtmlString(string.Format(link, paths.TrimEnd(','), "&t=" + Consts.Config.StaticTick));
            }
            var sb = new StringBuilder();
            foreach (var p in pathList)
            {
                var item = p;
                sb.AppendLine(string.Format(link, paths + FormatPath(item).TrimStart('/'),
                    "?t=" + Consts.Config.StaticTick));
            }
            return new HtmlString(sb.ToString());
        }
    }
}
