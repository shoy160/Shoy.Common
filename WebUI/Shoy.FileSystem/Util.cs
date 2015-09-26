using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ICSharpCode.SharpZipLib.Zip;
using Shoy.FileSystem.Domain;
using Shoy.Utility;
using Shoy.Utility.Config;
using Shoy.Utility.Extend;
using Shoy.Utility.Helper;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Shoy.FileSystem
{
    internal static class Util
    {
        /// <summary>
        /// 设置编码格式
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string SetEncoding(HttpContext context)
        {
            bool gzip, deflate;
            if (!string.IsNullOrEmpty(context.Request.ServerVariables["HTTP_ACCEPT_ENCODING"]))
            {
                string acceptedTypes = context.Request.ServerVariables["HTTP_ACCEPT_ENCODING"].ToLower();
                gzip = acceptedTypes.Contains("gzip") || acceptedTypes.Contains("x-gzip") || acceptedTypes.Contains("*");
                deflate = acceptedTypes.Contains("deflate");
            }
            else
                gzip = deflate = false;

            string encoding = (gzip ? "gzip" : (deflate ? "deflate" : "none"));

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

        /// <summary>
        /// 判断是否缓存在浏览器中
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool IsCachedOnBrowser(HttpContext context, string hash)
        {
            if (!string.IsNullOrEmpty(context.Request.ServerVariables["HTTP_IF_NONE_MATCH"]) &&
                context.Request.ServerVariables["HTTP_IF_NONE_MATCH"].Equals(hash))
            {
                context.Response.ClearHeaders();
                context.Response.StatusCode = 304;
                context.Response.Status = "304 Not Modified";
                context.Response.AppendHeader("Content-Length", "0");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取本地文件内容
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="context"></param>
        /// <param name="fileNames"></param>
        /// <returns></returns>
        public static string GetLocalFile(Uri uri, HttpContext context, List<string> fileNames)
        {

            string html;
            try
            {
                string path2 = context.Server.MapPath(uri.AbsolutePath);
                html = System.IO.File.ReadAllText(path2);
                fileNames.Add(path2);
            }
            catch
            {
                html = GetRemoteFile(uri);
            }
            return html;
        }

        /// <summary>
        /// 获取远程文件
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static string GetRemoteFile(Uri uri)
        {
            var html = new StringBuilder();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                using (var resp = request.GetResponse() as HttpWebResponse)
                {
                    if (resp == null) return string.Empty;
                    using (Stream recDataStream = resp.GetResponseStream())
                    {
                        if (recDataStream == null) return string.Empty;
                        var buffer = new byte[1024];
                        int read;
                        do
                        {
                            read = recDataStream.Read(buffer, 0, buffer.Length);
                            html.Append(Encoding.UTF8.GetString(buffer, 0, read));
                        }
                        while (read > 0);
                    }
                }
            }
            catch (Exception)
            {
                return html.ToString();
            }
            return html.ToString();
        }

        /// <summary>
        /// 检测扩展名
        /// </summary>
        /// <param name="path">uri或路径</param>
        /// <param name="contentLength"></param>
        /// <param name="fileType">文件类型</param>
        /// <returns></returns>
        internal static DResult CheckFile(string path, int contentLength, FileType fileType)
        {
            if (string.IsNullOrWhiteSpace(path) || contentLength == 0)
                return DResult.Error("文件路径为空或文件丢失！");
            if (fileType == FileType.ZipFile || fileType == FileType.All)
                return DResult.Success;
            var ext = Path.GetExtension(path);
            if (string.IsNullOrWhiteSpace(ext))
                return DResult.Error("文件无扩展名！");
            FileTypeLimit limit;
            var config = ConfigUtils<FileServerConfig>.Instance.Get();
            switch (fileType)
            {
                case FileType.Image:
                    limit = config.Image;
                    break;
                case FileType.Video:
                    limit = config.Video;
                    break;
                case FileType.Audio:
                    limit = config.Audio;
                    break;
                default:
                    limit = config.Attach;
                    break;
            }
            if (!limit.Exts.Contains(ext))
                return DResult.Error("扩展名{0}不被支持！".FormatWith(ext));
            if (limit.MaxSize * 1024 < contentLength)
                return DResult.Error("文件大小超出了限制范围：不大于{0}k！！".FormatWith(limit.MaxSize));
            return DResult.Success;
        }

        /// <summary>
        /// 获取新文件名
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string GenerateFileName(string ext = null)
        {
            var name = IdHelper.Instance.Guid32;
            return string.Concat(name, ext ?? string.Empty);
        }

        public static void ResponseJson(string str, HttpContext context = null)
        {
            context = context ?? HttpContext.Current;
            context.Response.Clear();
            context.Response.Expires = -1;
            context.Response.ContentType = "application/json";
            context.Response.Write(str);
            context.Response.Flush();
            context.Response.End();
        }

        /// <summary> 返回图片格式 </summary>
        /// <param name="context"></param>
        /// <param name="defaultFile"></param>
        internal static void ResponseImage(HttpContext context = null, string defaultFile = null)
        {
            context = context ?? HttpContext.Current;
            if (context == null)
                return;
            var path = (string.IsNullOrWhiteSpace(defaultFile) ? context.Request.Url.AbsolutePath : defaultFile);
            var ext = Path.GetExtension(path);
            if (string.IsNullOrWhiteSpace(ext))
                return;
            var reg = new Regex(Contains.ImageUrlRegex, RegexOptions.IgnoreCase);
            var uri = context.Request.Url.AbsoluteUri;
            var hash = uri.Md5();
            if (IsCachedOnBrowser(context, hash))
                return;
            int width = -1, height = -1;
            if (reg.IsMatch(uri))
            {
                var m = reg.Match(uri);
                width = ConvertHelper.StrToInt(m.Groups["w"].Value, -1);
                height = ConvertHelper.StrToInt(m.Groups["h"].Value, -2);
                if (width == -1 && height < -1) height = -1;
                if (path != null)
                    path = path.Replace(m.Groups[1].Value, string.Empty);
            }
            var resp = context.Response;
            resp.AppendHeader("Vary", "Accept-Encoding");
            resp.AppendHeader("Cache-Control", "max-age=604800");
            resp.AppendHeader("Expires", DateTime.Now.AddYears(1).ToString("R"));
            resp.AppendHeader("ETag", hash);
            resp.ContentType = Contains.MiniType[ext];
            resp.Charset = "utf-8";
            var fileStream = Stream.Null;
            if (path != null && path.StartsWith("/mongo/"))
            {
                var name = Path.GetFileNameWithoutExtension(path);
                var db = path.Split('/')[2];
                fileStream = GridFsManager.Instance(db).Read(name);
                if (fileStream == Stream.Null)
                    path = context.Server.MapPath(Contains.DefaultImage);
            }
            else
            {
                path = context.Server.MapPath(path);
            }

            using (var img = fileStream == Stream.Null ? new ImageCls(path) : new ImageCls(fileStream))
            {
                var bit = img.ResizeImg(width, height);
                var encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 88L);
                var encoder = GetEncoderInfo(ext);
                bit.Save(resp.OutputStream, encoder, encoderParameters);
            }
        }

        /// <summary> 解压文件 </summary>
        /// <param name="stream">文件流</param>
        /// <param name="savePath">保存路径</param>
        public static void UnZipFile(Stream stream, string savePath)
        {
            using (var s = new ZipInputStream(stream))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    var fileName = Path.GetFileName(theEntry.Name);
                    if (string.IsNullOrWhiteSpace(fileName))
                        continue;
                    // create directory
                    var directoryName = Path.GetDirectoryName(theEntry.Name);
                    if (!string.IsNullOrWhiteSpace(directoryName))
                        savePath = Path.Combine(savePath, directoryName);
                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);
                    var filePath = Path.Combine(savePath, fileName);
                    using (var streamWriter = System.IO.File.Create(filePath))
                    {
                        var size = 2048;
                        var data = new byte[size];
                        while ((size = s.Read(data, 0, data.Length)) > 0)
                        {
                            streamWriter.Write(data, 0, size);
                        }
                    }
                }
            }
        }

        internal static void InitConfig()
        {
            var config = new FileServerConfig
            {
                Site = "http://file.dayeasy.dev/",
                DefaultImage = "/image/dayeasy.gif",
                MaxFileCount = 4000,
                Image = new FileTypeLimit
                {
                    Exts = new[] { ".jpg", ".jpeg", ".gif", ".png" },
                    MaxSize = 1024
                },
                Video = new FileTypeLimit
                {
                    Exts = new[] { ".flv", ".mp4", ".swf", ".rm" },
                    MaxSize = 500 * 1024
                },
                Audio = new FileTypeLimit
                {
                    Exts = new[] { ".ogg", ".mp3" },
                    MaxSize = 50 * 1024
                },
                Attach = new FileTypeLimit
                {
                    Exts = new[] { ".rar", ".zip", ".7z", ".doc", ".docx" },
                    MaxSize = 100 * 1024
                },
                Mongo = new MongoConfig
                {
                    Servers = new List<MongoServer>
                    {
                        new MongoServer {Host = "192.168.10.20", Port = 27015}
                    },
                    Credentials = new List<MongoCredential>
                    {
                        new MongoCredential {DataBase = "file_picture", User = "file_user", Pwd = "dayeasy@123"},
                        new MongoCredential {DataBase = "file_attach", User = "file_user", Pwd = "dayeasy@123"}
                    }
                }
            };
            ConfigUtils<FileServerConfig>.Instance.Set(config);
        }

        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            //根据 mime 类型，返回编码器
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            mimeType = "image/" + mimeType.TrimStart('.').ToLower();
            mimeType = mimeType.Replace("jpg", "jpeg");
            return encoders.FirstOrDefault(t => t.MimeType == mimeType);
        }
    }
}
