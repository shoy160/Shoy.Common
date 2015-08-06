using System.IO;
using System.Web;
using Shoy.Utility.Extend;

namespace Shoy.FileSystem.Handler
{
    /// <summary> 文件服务 </summary>
    public class FileHandler : IHttpHandler
    {
        private HttpContext _context;
        private string _hash;

        /// <summary> 其他请求是否可使用此服务 </summary>
        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }

        void IHttpHandler.ProcessRequest(HttpContext httpContext)
        {
            _context = httpContext;
            var cache = _context.Request.Url.AbsoluteUri;
            var path = _context.Request.Path.Substring(5);
            var file = _context.Server.MapPath(path);
            var extension = Path.GetExtension(file);
            if (string.IsNullOrEmpty(extension))
                return;
            extension = extension.ToLower();

            Util.SetEncoding(_context);
            _hash = cache.Md5();

            if (Util.IsCachedOnBrowser(_context, _hash))
                return;
            _context.Response.WriteFile(file);
            _context.Response.ContentType = Contains.MiniType[extension];
        }
    }
}