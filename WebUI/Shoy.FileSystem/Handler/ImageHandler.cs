using System.Web;

namespace Shoy.FileSystem.Handler
{
    /// <summary>
    /// 图片服务
    /// </summary>
    public class ImageHandler : IHttpHandler
    {
        private HttpContext _context;

        /// <summary>
        /// 其他请求是否可使用此服务
        /// </summary>
        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }

        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            _context = context;
            var uri = context.Request.Url.AbsoluteUri;
            if (!Util.CheckFile(uri, 1, FileType.Image).Status)
            {
                Util.ResponseImage(_context, Contains.DefaultImage);
                return;
            }
            Util.ResponseImage(_context);
        }
    }
}