using System;
using System.IO.Compression;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web.Caching;

namespace Shoy.HttpCompress
{
    internal class Util
    {
        private const string LinkPattern = "<link[^>]*href=['\"](?<href>[^'\"]+)['\"][^>]*auto=\"true\"[^>]*>";
        private const string ScriptPattern = "<script[^>]*src=['\"](?<src>[^'\"]+)['\"][^>]*auto=\"true\"[^>]*></script>";
        private const string IfEnd = "<!--\\[[^\\]]+\\]>([\\w\\W]+?)<!\\[endif\\]-->";

        public static MatchCollection GetLinks(string html)
        {
            return Regex.Matches(html, LinkPattern, RegexOptions.IgnoreCase);
        }

        public static MatchCollection GetScripts(string html)
        {
            return Regex.Matches(html, ScriptPattern, RegexOptions.IgnoreCase);
        }

        public static IEnumerable<string> GetIfEnd(string html)
        {
            var zsLinks = new List<string>();
            foreach (Match match in Regex.Matches(html, IfEnd, RegexOptions.IgnoreCase | RegexOptions.Multiline))
            {
                var item = match.Groups[1].Value;
                item = item.Replace("\r", "").Replace("\n", "").Replace("\t", "");
                if (!string.IsNullOrEmpty(item))
                    zsLinks.Add(item.Trim());
            }
            return zsLinks;
        }

        public static string SetEncoding(HttpContext context)
        {
            bool gzip, deflate;
            string encoding = "none";
            if (!string.IsNullOrEmpty(context.Request.ServerVariables["HTTP_ACCEPT_ENCODING"]))
            {
                string acceptedTypes = context.Request.ServerVariables["HTTP_ACCEPT_ENCODING"].ToLower();
                gzip = acceptedTypes.Contains("gzip") || acceptedTypes.Contains("x-gzip") || acceptedTypes.Contains("*");
                deflate = acceptedTypes.Contains("deflate");
            }
            else
                gzip = deflate = false;

            encoding = gzip ? "gzip" : (deflate ? "deflate" : "none");

            if (context.Request.Browser.Browser == "IE")
            {
                if (context.Request.Browser.MajorVersion < 6)
                    encoding = "none";
                else if (context.Request.Browser.MajorVersion == 6 &&
                    !string.IsNullOrEmpty(context.Request.ServerVariables["HTTP_USER_AGENT"]) &&
                    context.Request.ServerVariables["HTTP_USER_AGENT"].Contains("EV1"))
                    encoding = "none";
            }
            return encoding;
        }

        public static bool IsCachedOnBrowser(HttpContext context, string hash, string contentType)
        {
            if (!string.IsNullOrEmpty(context.Request.ServerVariables["HTTP_IF_NONE_MATCH"]) &&
                context.Request.ServerVariables["HTTP_IF_NONE_MATCH"].Equals(hash))
            {
                context.Response.ClearHeaders();
                context.Response.Status = "304 Not Modified";
                context.Response.AppendHeader("Content-Length", "0");
                return true;
            }
            return false;
        }

        public static string GetLocalFile(Uri uri, HttpContext context, List<string> fileNames)
        {

            string html = "";
            try
            {
                string path2 = context.Server.MapPath(uri.AbsolutePath);
                html = File.ReadAllText(path2);
                fileNames.Add(path2);
            }
            catch
            {
                html = GetRemoteFile(uri);
            }
            return html;
        }

        public static string GetRemoteFile(Uri uri)
        {
            var html = new StringBuilder();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                using (var resp = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream recDataStream = resp.GetResponseStream())
                    {
                        byte[] buffer = new byte[1024];
                        int read = 0;

                        do
                        {
                            read = recDataStream.Read(buffer, 0, buffer.Length);
                            html.Append(UTF8Encoding.UTF8.GetString(buffer, 0, read));
                        }
                        while (read > 0);
                    }
                }
            }
            catch
            {
                // The remote site is currently down. Try again next time.
            }
            return html.ToString();
        }


        public static string GetMd5Sum(string str)
        {
            Encoder enc = System.Text.Encoding.Unicode.GetEncoder();

            byte[] unicodeText = new byte[str.Length * 2];
            enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(unicodeText);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 检测配置文件
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Configuration CheckConfig(HttpContext context)
        {
            if (context == null) return null;
            Configuration settings = null;
            if (context.Request["HTTP_X_MICROSOFTAJAX"] != null)
                return settings;
            if (context.Cache["HangeCompressModuleConfig"] == null)
            {
                settings = (Configuration)ConfigurationManager.GetSection("HangeWeb/HttpCompress");
                context.Cache.Insert("HangeCompressModuleConfig", settings,
                                     new CacheDependency(context.Server.MapPath("~/web.config")));
                //context.Cache["HangeCompressModuleConfig"] = settings;
            }
            else
            {
                settings = (Configuration) context.Cache.Get("HangeCompressModuleConfig");
            }
            if (settings == null || settings.CompressionType == CompressionType.None)
                return null;
            var realPath = context.Request.Path;
            //var appPath = context.Request.ApplicationPath;
            //if (!string.IsNullOrEmpty(appPath))
            //    realPath = realPath.Remove(0, appPath.Length);
            //realPath = realPath.TrimStart('/');
            realPath = Path.GetFileName(realPath);

            bool isIncludedPath = (settings.IncludedPaths.Contains(realPath) ||
                                   settings.IncludedPaths.Contains("~/" + realPath));
            bool isIncludedMime = (settings.IncludedMimeTypes.Contains(context.Response.ContentType));

            if (!isIncludedPath && !isIncludedMime)
                return null;

            if (settings.ExcludedPaths.Contains(realPath) || settings.ExcludedPaths.Contains("~/" + realPath))
                return null;

            if (settings.ExcludedMimeTypes.Contains(context.Response.ContentType))
                return null;
            return settings;
        }

        public static bool DeelImage(HttpContext context)
        {
            var type = context.Response.ContentType;
            var ext = Path.GetExtension(context.Request.Path);
            if (!string.IsNullOrEmpty(ext) &&
                (type.StartsWith("image/") || new[] {".jpg", ".jpeg", ".gif", ".png",".ico"}.Contains(ext)))
            {
                try
                {
                    string cache = context.Request.Url.AbsoluteUri;
                    string file = context.Server.MapPath(context.Request.Path);
                    ext = ext.ToLower().Remove(0, 1);

                    var encoding = SetEncoding(context);
                    var hash = GetMd5Sum(cache);

                    if (IsCachedOnBrowser(context, hash, "image/" + ext))
                        return true;

                    context.Response.AppendHeader("Vary", "Accept-Encoding");
                    context.Response.AppendHeader("Cache-Control", "max-age=604800");
                    context.Response.AppendHeader("Expires", DateTime.Now.AddYears(1).ToString("R"));
                    context.Response.AppendHeader("ETag", hash);
                    context.Response.WriteFile(file);
                    context.Response.ContentType = "image/" + ext;
                    context.Response.Charset = "utf-8";
                }
                catch{}
                return true;
            }
            return false;
        }

        public static string GetContentType(string type, string ext)
        {
            switch (ext)
            {
                case ".js":
                    return "application/x-javascript";
                case ".css":
                    return "text/css";
                default:
                    return type;
            }
        }
    }
}
