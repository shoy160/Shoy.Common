using System.IO;
using System.Web.Mvc;
using Shoy.Utility.Helper;
using Shoy.Utility.Logging;

namespace Shoy.Web.Filters
{
    /// <summary> 静态化页面过滤器 </summary>
    public class StaticFileAttribute : FilterAttribute, IResultFilter
    {
        private readonly bool _overwrite;
        private readonly string _staticPath;

        /// <summary> 构造函数 </summary>
        /// <param name="staticPath">支持绝对路径，相对路径以及appsetting配置：[key]</param>
        /// <param name="overwrite"></param>
        public StaticFileAttribute(string staticPath = null, bool overwrite = false)
        {
            _overwrite = overwrite;
            _staticPath = staticPath;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Filter =
                new StaticFileFilterWrapper(filterContext.HttpContext.Response.Filter, filterContext,
                    _staticPath, _overwrite);
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        class StaticFileFilterWrapper : Stream
        {
            private readonly Stream _inner;
            private readonly string _filePath;
            private readonly ILogger _logger = LogManager.Logger<StaticFileFilterWrapper>();

            public StaticFileFilterWrapper(Stream stream, ControllerContext context, string path = null,
                bool overwrite = false)
            {
                _inner = stream;
                if (context.HttpContext.Request.QueryString["preview"] == "true")
                    return;
                if (string.IsNullOrWhiteSpace(path))
                    path = context.HttpContext.Request.Path;
                if (path.StartsWith("[") && path.EndsWith("]"))
                    path = ConfigHelper.GetAppSetting(null, string.Empty, supressKey: path.Trim('[', ']'));

                if (!Path.HasExtension(path))
                    return;

                if (!Path.IsPathRooted(path))
                    path = context.HttpContext.Server.MapPath(path);

                if (File.Exists(path))
                {
                    if (overwrite)
                        File.Delete(path);
                    else
                        return;
                }
                var dir = Path.GetDirectoryName(path);
                if (string.IsNullOrWhiteSpace(dir))
                    return;
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                _filePath = path;
            }

            public override bool CanRead
            {
                get { return _inner.CanRead; }
            }

            public override bool CanSeek
            {
                get { return _inner.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return _inner.CanWrite; }
            }

            public override void Flush()
            {
                _inner.Flush();
            }

            public override long Length
            {
                get { return _inner.Length; }
            }

            public override long Position
            {
                get
                {
                    return _inner.Position;
                }
                set
                {
                    _inner.Position = value;
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _inner.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _inner.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _inner.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _inner.Write(buffer, offset, count);
                if (!string.IsNullOrWhiteSpace(_filePath))
                    File.AppendAllText(_filePath, System.Text.Encoding.UTF8.GetString(buffer));
            }
        }
    }
}
