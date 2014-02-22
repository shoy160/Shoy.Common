using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Shoy.HttpCompress
{
    internal class CompressionPageFilter : Stream
    {
        private HttpApplication _app;
        public HttpApplication App
        {
            get { return _app; }
            set { _app = value; }
        }

        public Configuration Setting { get; set; }

        private string _compress = "none";
        public string Compress
        {
            get { return _compress; }
            set { _compress = value; }
        }

        StringBuilder responseHtml;

        public CompressionPageFilter(Stream sink)
        {
            _sink = sink;
            responseHtml = new StringBuilder();
        }

        private Stream _sink;

        #region Properites

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            _sink.Flush();
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position { get; set; }

        #endregion

        #region Methods

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _sink.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _sink.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _sink.SetLength(value);
        }

        public override void Close()
        {
            _sink.Close();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            string strBuffer = Encoding.UTF8.GetString(buffer, offset, count);

            var eof = new Regex("</html>", RegexOptions.IgnoreCase);

            responseHtml.Append(strBuffer);

            if (eof.IsMatch(strBuffer))
            {
                responseHtml.Append(Environment.NewLine + Environment.NewLine + Environment.NewLine +
                                    Environment.NewLine + Environment.NewLine + Environment.NewLine +
                                    Environment.NewLine + Environment.NewLine);
                string html = responseHtml.ToString();

                var builder = FilterBuilder.GetInstance(_app.Context);

                html = builder.GetHtml(html, Setting.AutoCompress.Css, Setting.AutoCompress.Js);

                //if (Setting.AutoCompress.Css)
                //    html = ReplaceCss(html);
                //if (Setting.AutoCompress.Js)
                //    html = ReplaceJs(html);

                byte[] data = Encoding.UTF8.GetBytes(html);

                if (_compress == "gzip")
                {
                    var gzip = new GZipStream(_sink, CompressionMode.Compress);
                    gzip.Write(data, 0, data.Length);
                }
                else if (_compress == "deflate")
                {
                    var deflate = new DeflateStream(_sink, CompressionMode.Compress);
                    deflate.Write(data, 0, data.Length);
                }
                else
                    _sink.Write(data, 0, data.Length);
            }
        }

        
        #endregion

    }
}
