using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;

namespace Shoy.Utility
{
    /// <summary>
    /// http请求类~
    /// create by shy --2012-08-17
    /// </summary>
    public class HttpHelper : IDisposable
    {
        private string _url;//
        private readonly string _method = "GET";
        private string _referer;
        private readonly string _paras;
        private readonly Encoding _encoding = Encoding.Default;//编码
        private string _cookie;
        private string _contentType;
        private Dictionary<string, Stream> _fileList;
        private MemoryStream _postStream;

        private HttpWebRequest _req;
        private HttpWebResponse _rep;

        private static readonly string Boundary = "-------------" + DateTime.Now.Ticks.ToString("x");
        private static readonly string NewLine = Environment.NewLine;

        #region 构造函数

        /// <summary>
        /// HttpHelper构造函数
        /// </summary>
        /// <param name="url"></param>
        public HttpHelper(string url)
            : this(url, "", Encoding.Default, "", "", "")
        {
        }

        /// <summary>
        /// HttpHelper构造函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        public HttpHelper(string url, Encoding encoding)
            : this(url, "", encoding, "", "", "")
        {
        }

        /// <summary>
        /// HttpHelper构造函数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="encoding"></param>
        /// <param name="paras"></param>
        public HttpHelper(string url, string method, Encoding encoding, string paras)
            : this(url, method, encoding, "", "", paras)
        {
        }

        /// <summary>
        /// HttpHelper构造函数
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="method">请求方法</param>
        /// <param name="encoding">请求编码</param>
        /// <param name="cookie">请求Cookie</param>
        /// <param name="referer">"base"为当前url域名</param>
        /// <param name="paras"></param>
        public HttpHelper(string url, string method, Encoding encoding, string cookie, string referer, string paras)
        {
            _url = url;
            if (!string.IsNullOrEmpty(method))
                _method = method;
            if (!string.IsNullOrEmpty(cookie))
                _cookie = cookie;
            if (!string.IsNullOrEmpty(referer))
                _referer = referer;
            if (!string.IsNullOrEmpty(paras))
                _paras = paras;
            _encoding = encoding;
        }

        #endregion

        /// <summary>
        /// 创建httpwebrequest 实例
        /// </summary>
        private void CreateHttpRequest()
        {
            if (string.IsNullOrEmpty(_url))
                return;
            if (!_url.StartsWith("http://") && !_url.StartsWith("https://"))
                _url = "http://" + _url;
            _req = (HttpWebRequest) WebRequest.Create(_url);

            _req.AllowAutoRedirect = true;

            _req.Method = _method;

            _req.Timeout = 15*1000;

            _req.ServicePoint.ConnectionLimit = 1024;
            if (_fileList != null && _fileList.Any())
                _req.ContentType = string.Format("multipart/form-data; boundary={0}", Boundary);
            else
                _req.ContentType = (string.IsNullOrWhiteSpace(_contentType)
                    ? "application/x-www-form-urlencoded"
                    : _contentType);
            _req.Headers.Add("Accept-language", "zh-cn,zh;q=0.5");
            _req.Headers.Add("Accept-Charset", "GB2312,utf-8;q=0.7,*;q=0.7");
            //_req.UserAgent =
            //    "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2; .NET4.0C; .NET4.0E)";
            _req.Headers.Add("Accept-Encoding", "gzip, deflate");
            _req.Headers.Add("Keep-Alive", "350");
            _req.Headers.Add("x-requested-with", "XMLHttpRequest");
            //仿百度蜘蛛
            _req.UserAgent = "Mozilla/5.0+(compatible;+Baiduspider/2.0;++http://www.baidu.com/search/spider.html)";
            //添加Cookie
            if (!string.IsNullOrEmpty(_cookie))
                _req.Headers.Add("Cookie", _cookie);
            if (!string.IsNullOrEmpty(_referer))
            {
                if (_referer == "base")
                {
                    var baseUrl = Utils.GetRegStr(_url, "http(s)?://([^/]+?)/", 2);
                    _req.Referer = baseUrl;
                }
                else
                {
                    _req.Referer = _referer;
                }
            }

            if (_method.ToUpper() == "POST")
            {
                WriteParams(_paras);
                //传文件
                if (_fileList != null && _fileList.Any())
                {
                    _req.AllowWriteStreamBuffering = false;
                    _req.Timeout = 300*1000;
                    _req.KeepAlive = true;
                    foreach (var file in _fileList)
                    {
                        WriteFileStream(file.Key, file.Value);
                    }
                    var strBoundary = string.Format("{1}--{0}--{1}", Boundary, NewLine);
                    WriteParams(strBoundary);
                }
                if (_postStream != null)
                {
                    _req.ContentLength = _postStream.Length;
                    var postStream = _req.GetRequestStream();
                    var buffer = new Byte[checked((uint) Math.Min(4096, (int) _postStream.Length))];
                    int bytesRead;
                    _postStream.Seek(0, SeekOrigin.Begin);
                    while ((bytesRead = _postStream.Read(buffer, 0, buffer.Length)) != 0)
                        postStream.Write(buffer, 0, bytesRead);
                    _postStream.Close();
                    _postStream.Dispose();
                }
            }
        }

        private void WriteRequestStream(byte[] buffer, int count)
        {
            if (_postStream == null)
                _postStream = new MemoryStream();
            _postStream.Write(buffer, 0, count);
        }

        /// <summary>
        /// 写post参数
        /// </summary>
        /// <param name="paras"></param>
        private void WriteParams(string paras)
        {
            if (string.IsNullOrWhiteSpace(paras)) return;
            var buffer = _encoding.GetBytes(paras);
            WriteRequestStream(buffer, buffer.Length);
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        private void WriteFileStream(string name,Stream file)
        {
            var fileField = new StringBuilder();
            fileField.Append(string.Format("{1}--{0}{1}", Boundary, NewLine));
            fileField.Append(string.Format(
                "Content-Disposition: form-data; name=\"file_{0}\"; filename=\"{1}\"{2}",
                (_fileList.Keys.ToList().IndexOf(name) + 1), Path.GetFileName(name), NewLine));
            //文件类型
            fileField.Append(string.Format("Content-Type: {0}{1}{1}", GetContentType(name), NewLine));
            WriteParams(fileField.ToString());

            var buffer = new Byte[checked((uint) Math.Min(4096, (int) file.Length))];
            int bytesRead;
            while ((bytesRead = file.Read(buffer, 0, buffer.Length)) != 0)
                WriteRequestStream(buffer, bytesRead);
            WriteParams(NewLine);
        }

        private string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(ext) || !Consts.ContentTypes.ContainsKey(ext))
                return Consts.ContentTypes["*"];
            return Consts.ContentTypes[ext];
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookie"></param>
        /// <param name="referer"></param>
        public void SetHttpInfo(string url,string cookie,string referer)
        {
            _url = url;
            _cookie = cookie;
            _referer = referer;
        }

        /// <summary>
        /// 设置url
        /// </summary>
        /// <param name="url"></param>
        public void SetUrl(string url)
        {
            _url = url;
        }

        /// <summary>
        /// 设置内容类型
        /// </summary>
        /// <param name="contentType"></param>
        public void SetContentType(string contentType)
        {
            _contentType = contentType;
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="fileList"></param>
        public void AddFiles(Dictionary<string, Stream> fileList)
        {
            if (_fileList == null)
                _fileList = new Dictionary<string, Stream>();
            foreach (var key in fileList.Keys)
            {
                if (!_fileList.ContainsKey(key))
                    _fileList.Add(key, fileList[key]);
            }
        }

        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="pathList"></param>
        public void AddFiles(List<string> pathList)
        {
            if (_fileList == null)
                _fileList = new Dictionary<string, Stream>();
            var list =
                pathList.Select(path => new FileStream(path, FileMode.Open, FileAccess.Read)).ToList();
            foreach (var fileStream in list)
            {
                if (!_fileList.ContainsKey(fileStream.Name))
                    _fileList.Add(fileStream.Name, fileStream);
            }
        }

        /// <summary>
        /// 获取请求的url
        /// </summary>
        /// <returns></returns>
        public string GetRequestUrl()
        {
            if (_req == null)
                return "";
            return _req.Address.ToString();
        }

        /// <summary>
        /// 设置有帐号的代理
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPwd"></param>
        /// <param name="ip"></param>
        public void SetWebProxy(string userName,string userPwd,string ip)
        {
            if (_req != null)
            {
                //设置代理服务器
                var myProxy = new WebProxy(ip, false)
                                  {
                                      //建立连接
                                      Credentials = new NetworkCredential(userName, userPwd)
                                  };
                //给当前请求对象
                _req.Proxy = myProxy;
                //设置安全凭证
                _req.Credentials = CredentialCache.DefaultNetworkCredentials;
            }
        }

        /// <summary>
        /// 设置免费代理
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void SetWebProxy(string ip, int port)
        {
            if (_req != null)
            {
                //设置代理服务器
                var myProxy = new WebProxy(ip, port);

                //给当前请求对象
                _req.Proxy = myProxy;
                //设置安全凭证
                _req.Credentials = CredentialCache.DefaultNetworkCredentials;
            }
        }

        /// <summary>
        /// 获取返回流
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
            CreateHttpRequest();
            if (_req == null)
                return null;
            Stream stream = null;
            try
            {
                _rep = (HttpWebResponse) _req.GetResponse();
                //try
                //{
                //    _cookie = _rep.Headers["set-cookie"];
                //}
                //catch
                //{
                //    _cookie = "";
                //}
                stream = (_rep.ContentEncoding == "gzip"
                              ? new GZipStream(_rep.GetResponseStream(), CompressionMode.Decompress)
                              : _rep.GetResponseStream());
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
            }
            return stream;
        }

        /// <summary>
        /// 获取cookie
        /// </summary>
        /// <returns></returns>
        public string GetCookie()
        {
            CreateHttpRequest();
            if (_req == null)
                return "";
            try
            {
                _rep = (HttpWebResponse) _req.GetResponse();
                _cookie = _rep.Headers["set-cookie"];
                return _cookie;
            }
            catch (Exception ex)
            {
                Utils.WriteException(ex);
                _cookie = "";
                return "";
            }
        }

        /// <summary>
        /// 获取html代码
        /// </summary>
        /// <returns></returns>
        public string GetHtml()
        {
            var str = "";
            var stream = GetStream();
            if (stream != null)
            {
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader(stream, _encoding);
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                finally
                {
                    if (sr != null)
                        sr.Close();
                }
            }
            return str;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool SaveFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            else
            {
                return false;
            }
            var resStream = GetStream();
            if (resStream == null)
                return false;
            try
            {
                using (Stream fileStream = new FileStream(path, FileMode.Create))
                {
                    var by = new byte[1024];
                    int osize = resStream.Read(by, 0, by.Length);
                    while (osize > 0)
                    {
                        fileStream.Write(by, 0, osize);
                        osize = resStream.Read(by, 0, by.Length);
                    }
                    resStream.Close();
                    fileStream.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        //[DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //static extern bool InternetGetCookieEx(string pchUrl, string pchCookieName, StringBuilder pchCookieData, ref int pcchCookieData, int dwFlags, object lpReserved);
        ////WebBrowser取出Cookie，当登录后才能取    
        //public string GetCookieString()
        //{
        //    // Determine the size of the cookie      
        //    int datasize = 256;
        //    var cookieData = new StringBuilder(datasize);
        //    if (!InternetGetCookieEx(_url, null, cookieData, ref datasize, 0x00002000, null))
        //    {
        //        if (datasize < 0)
        //            return null;
        //        // Allocate stringbuilder large enough to hold the cookie    
        //        cookieData = new StringBuilder(datasize);
        //        if (!InternetGetCookieEx(_url, null, cookieData, ref datasize, 0x00002000, null))
        //            return null;
        //    }
        //    return cookieData.ToString();
        //}

        #region IDisposable 成员

        void IDisposable.Dispose()
        {
            if (_fileList != null && _fileList.Any())
            {
                foreach (var fileStream in _fileList)
                {
                    fileStream.Value.Close();
                    fileStream.Value.Dispose();
                }
            }
            if (_postStream != null)
            {
                _postStream.Close();
                _postStream.Dispose();
            }
            if (_rep != null)
                _rep.Close();
            if (_req != null)
                _req.Abort();
        }

        #endregion
    }
}
