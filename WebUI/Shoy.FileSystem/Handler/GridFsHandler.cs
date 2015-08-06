using System;
using System.IO;
using System.Linq;
using System.Web;
using Shoy.Utility.Extend;

namespace Shoy.FileSystem.Handler
{
    public class GridFsHandler : IHttpHandler
    {
        private HttpContext _context;

        /// <summary> 其他请求是否可使用此服务 </summary>
        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }

        void IHttpHandler.ProcessRequest(HttpContext httpContext)
        {
            _context = httpContext;
            var path = _context.Request.Url.AbsolutePath;
            var ext = (Path.GetExtension(path) ?? string.Empty).ToLower();

            if (Contains.Config.Image.Exts.Contains(ext))
            {
                Util.ResponseImage(_context);
                return;
            }
            var fileName = Path.GetFileNameWithoutExtension(path);
            var db = path.Split('/')[2];
            var stream = GridFsManager.Instance(db).Read(fileName);
            var hash = _context.Request.Url.AbsoluteUri.Md5();
            if (Util.IsCachedOnBrowser(_context, hash))
                return;
            var resp = _context.Response;
            resp.AppendHeader("Vary", "Accept-Encoding");
            resp.AppendHeader("Cache-Control", "max-age=604800");
            resp.AppendHeader("Expires", DateTime.Now.AddYears(1).ToString("R"));
            resp.AppendHeader("ETag", hash);
            resp.ContentType = Contains.MiniType[ext];
            resp.Charset = "utf-8";

            stream.CopyTo(resp.OutputStream);
            stream.Flush();
            stream.Close();

            resp.End();
        }
    }
}