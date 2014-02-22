using System;
using System.IO;
using System.Web;

namespace Shoy.HttpCompress
{
    public class ImageHandler : IHttpHandler
    {
        private HttpContext context;
        private string encoding, hash;

        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }

        void IHttpHandler.ProcessRequest(HttpContext httpContext)
        {
            context = httpContext;
            string cache = context.Request.Url.AbsoluteUri;
            string file = context.Server.MapPath(context.Request.Path);
            string extension = Path.GetExtension(context.Request.PhysicalPath);
            if (string.IsNullOrEmpty(extension))
                return;
            extension = extension.ToLower().Remove(0, 1);

            encoding = Util.SetEncoding(context);
            hash = Util.GetMd5Sum(cache);

            if (Util.IsCachedOnBrowser(context, hash, "image/" + extension))
                return;

            context.Response.AppendHeader("Vary", "Accept-Encoding");
            context.Response.AppendHeader("Cache-Control", "max-age=604800");
            context.Response.AppendHeader("Expires", DateTime.Now.AddYears(1).ToString("R"));
            context.Response.AppendHeader("ETag", hash);
            context.Response.WriteFile(file);
            context.Response.ContentType = "image/" + extension;
            context.Response.Charset = "utf-8";
        }
    }
}
