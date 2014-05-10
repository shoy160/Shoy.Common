using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Shoy.HttpCompress
{
    public class CompressionHandler : IHttpHandler
    {
        private string _encoding, _hash;
        private HttpContext _context;

        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }

        void IHttpHandler.ProcessRequest(HttpContext httpContext)
        {
            _context = httpContext;
            string cache = _context.Request.Url.AbsoluteUri;
            _hash = Util.GetMd5Sum(cache);
            var ext = Path.GetExtension(_context.Request.Path);
            string contentType = "";
            if (ext == ".axd")
            {
                contentType = DeelAxdPath(cache);
                if (string.IsNullOrEmpty(contentType))
                    return;
            }
            else
            {
                var settings = Util.CheckConfig(_context);
                if (settings == null)
                    return;
                contentType = Util.GetContentType(_context.Response.ContentType, ext);
                if (string.IsNullOrEmpty(contentType))
                    return;
                if (_context.Cache[cache] == null)
                {
                    var baseUri = new Uri(httpContext.Request.Url.AbsoluteUri);
                    var html = Util.GetLocalFile(baseUri, _context, new List<string>());
                    httpContext.Cache.Insert(cache, html, null, Cache.NoAbsoluteExpiration, new TimeSpan(3, 0, 0, 0));
                }
                else if (Util.IsCachedOnBrowser(httpContext, _hash, contentType))
                    return;
            }
            _encoding = Util.SetEncoding(_context);
            _context.Response.ClearHeaders();
            _context.Response.AppendHeader("Vary", "Accept-Encoding");
            _context.Response.AppendHeader("Cache-Control", "max-age=604800");
            _context.Response.AppendHeader("Expires", DateTime.Now.AddYears(1).ToString("R"));
            _context.Response.AppendHeader("ETag", _hash);
            _context.Response.Write(_context.Cache[cache]);
            _context.Response.AppendHeader("Content-Type", contentType);

            if (!string.IsNullOrEmpty(_encoding) && _encoding != "none")
            {
                if (_encoding == "gzip")
                    _context.Response.Filter = new GZipStream(_context.Response.Filter, CompressionMode.Compress);
                else
                    _context.Response.Filter = new DeflateStream(_context.Response.Filter, CompressionMode.Compress);
                _context.Response.AppendHeader("Content-Encoding", _encoding);
            }
        }

        private string DeelAxdPath(string cache)
        {
            var sb = new StringBuilder();
            string path = Path.GetFileNameWithoutExtension(_context.Request.Path);
            string contentType = (path == "js" || path == "javascript"
                                      ? "text/javascript"
                                      : (path == "css" ? "text/css" : ""));

            _hash = Util.GetMd5Sum(cache);
            var baseUri = new Uri(_context.Request.Url.AbsoluteUri);

            if (_context.Cache[cache] == null)
            {
                string dir = _context.Request["dir"],
                       files = _context.Request["files"];
                if (string.IsNullOrEmpty(files))
                {
                    files = _context.Request["paths"];
                    if (string.IsNullOrEmpty(files))
                        return contentType;
                }
                if (string.IsNullOrEmpty(dir))
                    dir = "js/";
                if (!dir.EndsWith("/"))
                    dir += "/";
                string[] tempFiles = files.Split(',');
                var fileNames = new List<string>();
                foreach (string file in tempFiles)
                {
                    string temp = Util.GetLocalFile(new Uri(baseUri, dir + file), _context, fileNames);
                    if (contentType == "text/javascript")
                        sb.AppendLine(MyMin.parse(temp));
                    else if (contentType == "text/css")
                        sb.AppendLine(MyMin.parse(temp, true, true));
                    else
                        sb.AppendLine(temp);
                }

                if (fileNames.Count > 0)
                    _context.Cache.Insert(cache, sb.ToString(), new CacheDependency(fileNames.ToArray()));
                else
                    _context.Cache.Insert(cache, sb.ToString(), null, Cache.NoAbsoluteExpiration,
                                         new TimeSpan(3, 0, 0, 0));
            }
            else if (Util.IsCachedOnBrowser(_context, _hash, contentType))
                return "";
            return contentType;
        }
    }
}
