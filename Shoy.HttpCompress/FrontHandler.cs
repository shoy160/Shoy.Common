using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.IO.Compression;
using System.Web.Caching;

namespace Shoy.HttpCompress
{
    public class FrontHandler:IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        private DateTime _lastModify = DateTime.MinValue;
        private HttpContext _context;

        public void ProcessRequest(HttpContext context)
        {
            _context = context;
            string ext = Path.GetExtension(context.Request.Path),
                   fileName = Path.GetFileName(context.Request.Path);
            var files = new List<string>();
            switch (ext)
            {
                case ".axd":
                    var temp = context.Request["files"];
                    if(string.IsNullOrEmpty(temp))
                        return;
                    files.AddRange(temp.Split(','));
                    break;
                case ".js":
                case ".css":
                    files.Add(fileName);
                    break;
            }
            var contentType = "text/css";
            if (ext == ".js" || fileName == "js.axd")
                contentType = "application/x-javascript";
            var html = GetFilePath(files);
            if (IsCache()) return;
            var encoding = Util.SetEncoding(context);
            context.Response.ClearHeaders();
            context.Response.AppendHeader("Vary", "Accept-Encoding");
            context.Response.AppendHeader("Last-Modified", _lastModify.ToUniversalTime().ToString("R"));
            context.Response.AppendHeader("Content-Type", contentType);
            context.Response.Write(html);

            if (!string.IsNullOrEmpty(encoding) && encoding != "none")
            {
                if (encoding == "gzip")
                    context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
                else
                    context.Response.Filter = new DeflateStream(context.Response.Filter, CompressionMode.Compress);
                context.Response.AppendHeader("Content-Encoding", encoding);
            }

        }

        private bool IsCache()
        {
            var modify = _context.Request.Headers["If-Modified-Since"];
            if(!string.IsNullOrEmpty(modify))
            {
                var date = Convert.ToDateTime(modify);
                if (Math.Abs((_lastModify - date).TotalSeconds) < 1)
                {
                    _context.Response.StatusCode = 304;
                    _context.Response.End();
                    return true;
                }
            }
            return false;
        }

        private string GetFilePath(IEnumerable<string> files)
        {
            var key = _context.Request.Url.PathAndQuery;
            var html = (_context.Cache.Get(key) ?? "").ToString();
            if (!string.IsNullOrEmpty(html))
                return html;
            var uri = new Uri(_context.Request.Url.AbsoluteUri);
            var sb = new StringBuilder();
            var paths = new List<string>();
            foreach (string file in files)
            {
                try
                {
                    var fileUri = new Uri(uri, file);
                    var path = _context.Server.MapPath(fileUri.AbsolutePath);
                    paths.Add(path);
                    var info = new FileInfo(path);
                    if (_lastModify == DateTime.MinValue || info.LastWriteTime > _lastModify)
                        _lastModify = info.LastWriteTime;
                    sb.Append(File.ReadAllText(path));
                    sb.Append(Environment.NewLine);
                }
                catch
                {
                    continue;
                }
            }
            _context.Cache.Insert(key, sb, new CacheDependency(paths.ToArray()));
            return sb.ToString();
        }
    }
}
