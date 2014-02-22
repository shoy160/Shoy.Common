using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Shoy.HttpCompress
{
    public class FilterBuilder
    {
        private readonly HttpContext _context;
        private readonly string _appPath;

        private FilterBuilder(){}

        private FilterBuilder(HttpContext context)
        {
            _context = context;
            _appPath = context.Request.ApplicationPath;
            if (_appPath != null) _appPath = _appPath.ToLower();
        }

        public static FilterBuilder GetInstance(HttpContext context)
        {
            return new FilterBuilder(context);
        }

        public string GetHtml(string html, bool autoCss, bool autoJs)
        {
            if (autoCss)
                html = ReplaceCss(html);
            if (autoJs)
                html = ReplaceJs(html);
            return html;
        }


        private string ReplaceCss(string html)
        {
            var header =
                Regex.Match(html, "<head[^>]*>([\\w\\W]+)</head>", RegexOptions.IgnoreCase | RegexOptions.Multiline)
                    .Groups[1].Value;
            var bkHeader = header;
            var css = new Dictionary<string, List<string>>();

            var baseUri = new Uri(_context.Request.Url.AbsoluteUri);

            foreach (Match match in Util.GetLinks(header))
            {
                string linkHtml = match.Value;
                if (!linkHtml.Contains("stylesheet") && !linkHtml.Contains("text/css"))
                    continue;
                string href = match.Groups["href"].Value;
                string url = "";

                var uri = new Uri(baseUri, href);

                if (uri.Host == baseUri.Host)
                {
                    if (_appPath != null && uri.AbsolutePath.ToLower().StartsWith(_appPath))
                    {
                        int index = uri.AbsolutePath.LastIndexOf("/");
                        string path = uri.AbsolutePath.Substring(0, index + 1);
                        string file = uri.AbsolutePath.Substring(index + 1);
                        if (!css.ContainsKey(path))
                        {
                            css.Add(path, new List<string>());
                            header = header.Replace(linkHtml, "<css_" + (css.Count - 1) + ">");
                        }
                        else
                        {
                            header = header.Replace(linkHtml, "");
                        }
                        css[path].Add(file + (href.Contains("?") ? href.Substring(0, href.IndexOf("?")) : ""));
                    }
                    else
                    {
                        url = uri.AbsolutePath + uri.Query;
                    }
                }
                else
                {
                    url = uri.AbsoluteUri;
                }
                if (!string.IsNullOrEmpty(url))
                {
                    header = header.Replace(linkHtml,
                                            string.Format(
                                                "<link href='css.axd?files={0}' type='text/css' rel='stylesheet' media='all' />",
                                                url));
                }
            }

            var link = new StringBuilder();
            link.AppendLine(Environment.NewLine);
            int i = 0;
            foreach (string key in css.Keys)
            {
                var item = string.Format("<link href='{0}css.axd?files={1}' type='text/css' rel='stylesheet' media='all' />",
                                         key, string.Join(",", css[key].ToArray())) + Environment.NewLine;
                header = header.Replace("<css_" + i + ">", item);
                i++;
            }
            html = html.Replace(bkHeader, header);
            return html;
        }

        private string ReplaceJs(string html)
        {
            int start, end;
            if (html.Contains("<head") && html.Contains("</head>"))
            {
                start = html.IndexOf("<head");
                end = html.IndexOf("</head>");
                string head = html.Substring(start, end - start);

                head = ReplaceJsInHead(head);

                html = html.Substring(0, start) + head + html.Substring(end);
            }

            if (html.Contains("<body") && html.Contains("</body>"))
            {
                start = html.IndexOf("<body");
                end = html.IndexOf("</body>");
                string head = html.Substring(start, end - start);

                head = ReplaceJsInBody(head);

                html = html.Substring(0, start) + head + html.Substring(end);
            }

            return html;
        }

        private string ReplaceJsInHead(string html)
        {
            //var javascript = new List<string>();
            var js = new Dictionary<string, List<string>>();

            var baseUri = new Uri(_context.Request.Url.AbsoluteUri);
            foreach (Match match in Util.GetScripts(html))
            {
                string linkHtml = match.Value;

                string src = match.Groups["src"].Value;

                var uri = new Uri(baseUri, src);
                var ext = Path.GetExtension(uri.AbsolutePath);
                string url = "";
                if (ext != ".js" && !uri.AbsolutePath.Contains("WebResource.axd"))
                    continue;
                if (uri.Host == baseUri.Host)
                {
                    if (_appPath != null && uri.AbsolutePath.ToLower().StartsWith(_appPath))
                    {
                        int index = uri.AbsolutePath.LastIndexOf("/");
                        string path = uri.AbsolutePath.Substring(0, index + 1);
                        string file = uri.AbsolutePath.Substring(index + 1);
                        if (!js.ContainsKey(path))
                        {
                            js.Add(path, new List<string>());
                            html = html.Replace(linkHtml, "<js_" + (js.Keys.Count - 1) + ">");
                        }
                        else
                        {
                            html = html.Replace(linkHtml, "");
                        }
                        js[path].Add(file + (src.Contains("?") ? src.Substring(src.IndexOf("?")) : ""));
                    }
                    else
                    {
                        url = uri.AbsolutePath + uri.Query;
                    }

                }
                else
                {
                    url = uri.AbsoluteUri;
                }
                if (!string.IsNullOrEmpty(url))
                {
                    html = html.Replace(linkHtml,
                                        string.Format(
                                            "<script src='js.axd?files={0}' type='text/javascript' /></script>", url));
                }
            }

            int i = 0;
            foreach (string key in js.Keys)
            {
                var item = string.Format(
                    "<script src='{0}js.axd?files={1}' type='text/javascript' ></script>", key,
                    string.Join(",", js[key].ToArray()));
                html = html.Replace("<js_" + i + ">", item);
                i++;
            }
            return html;
        }

        private string ReplaceJsInBody(string html)
        {
            var js = new Dictionary<string, List<string>>();
            var baseUri = new Uri(_context.Request.Url.AbsoluteUri);
            foreach (Match match in Util.GetScripts(html))
            {
                string linkHtml = match.Value;
                string src = match.Groups["src"].Value;

                var uri = new Uri(baseUri, src);
                var ext = Path.GetExtension(uri.AbsolutePath);
                if (ext != ".js" && !uri.AbsolutePath.Contains("WebResource.axd"))
                    continue;
                string url = "";
                if (uri.Host == baseUri.Host)
                {
                    if (_appPath != null && uri.AbsolutePath.ToLower().StartsWith(_appPath))
                    {
                        int index = uri.AbsolutePath.LastIndexOf("/");
                        string path = uri.AbsolutePath.Substring(0, index + 1),
                               file = uri.AbsolutePath.Substring(index + 1);
                        if (!js.ContainsKey(path))
                        {
                            js.Add(path, new List<string>());
                            html = html.Replace(linkHtml, "<js_" + (js.Count - 1) + ">");
                        }
                        else
                        {
                            html = html.Replace(linkHtml, "");
                        }
                        js[path].Add(file + (src.Contains("?") ? src.Substring(src.IndexOf("?")) : ""));
                    }
                    else
                        url = uri.AbsolutePath + uri.Query;
                }
                else
                    url = uri.AbsoluteUri;
                if (!string.IsNullOrEmpty(url))
                {
                    string newLinkHtml = linkHtml.Replace(src, "js.axd?files=" + url);
                    html = html.Replace(linkHtml, newLinkHtml);
                }
            }
            //var sb = new StringBuilder();
            int i = 0;
            foreach (string key in js.Keys)
            {
                var item = string.Format(
                    "<script src='{0}js.axd?files={1}' type='text/javascript' ></script>", key,
                    string.Join(",", js[key].ToArray()));
                html = html.Replace("<js_" + i + ">", item);
                i++;
            }
            return html;
        }
    }
}
