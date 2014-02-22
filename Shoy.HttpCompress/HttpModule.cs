using System;
using System.IO;
using System.Web;

namespace Shoy.HttpCompress
{
    public class HttpModule : IHttpModule
    {
        #region IHttpModule Members

        void IHttpModule.Dispose()
        {

        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.PostReleaseRequestState += context_PostReleaseRequestState;
        }

        void context_PostReleaseRequestState(object sender, EventArgs e)
        {
            var app = (HttpApplication) sender;
            var context = app.Context;
            var settings = Util.CheckConfig(context);
            if (settings == null)
                return;
            context.Response.Cache.VaryByHeaders["Accept-Encoding"] = true;
            string acceptedTypes = app.Request.Headers["Accept-Encoding"];

            if (Util.DeelImage(context))
                return;
            var ext = Path.GetExtension(app.Request.Path);
            //ÐÞ¸ÄÎªaspx²»»º´æ
            if (ext != ".aspx")
            {
                string cache = app.Context.Request.Url.AbsoluteUri;
                cache = Util.GetMd5Sum(cache);
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetMaxAge(new TimeSpan(7, 0, 0, 0));
                context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
                try
                {
                    context.Response.Cache.SetETag(cache);
                }
                catch (InvalidOperationException)
                {
                    context.Response.AppendHeader("ETag", cache);
                }
            }
            context.Response.Charset = "utf-8";
            if (acceptedTypes == null)
                return;

            var filter = new CompressionPageFilter(app.Response.Filter) {App = app, Setting = settings};
            context.Response.Filter = filter;

            if (context.Request.Browser.Browser == "IE")
            {
                if (context.Request.Browser.MajorVersion < 6)
                    return;
                if (context.Request.Browser.MajorVersion == 6 &&
                    !string.IsNullOrEmpty(context.Request.ServerVariables["HTTP_USER_AGENT"]) &&
                    context.Request.ServerVariables["HTTP_USER_AGENT"].Contains("EV1"))
                    return;
            }
            acceptedTypes = acceptedTypes.ToLower();
            if ((acceptedTypes.Contains("gzip") || acceptedTypes.Contains("x-gzip") || acceptedTypes.Contains("*")) &&
                (settings.CompressionType != CompressionType.Deflate))
                filter.Compress = "gzip";
            else if (acceptedTypes.Contains("deflate"))
                filter.Compress = "deflate";
            try
            {
                if (filter.Compress != "none")
                {
                    //context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
                    context.Response.AppendHeader("Content-Encoding", filter.Compress);
                }
            }
            catch{}
        }

        #endregion
    }
}
