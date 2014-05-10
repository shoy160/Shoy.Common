using System;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

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

        private HttpWebRequest _req;
        private HttpWebResponse _rep;

        #region 构造函数

        public HttpHelper(string url)
            : this(url, "", Encoding.Default, "", "", "")
        {
        }

        public HttpHelper(string url, Encoding encoding)
            : this(url, "", encoding, "", "", "")
        {
        }

        public HttpHelper(string url, string method, Encoding encoding, string paras)
            : this(url, method, encoding, "", "", paras)
        {
        }

        /// <summary>
        /// HttpHelper构造
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
            _req.ContentType = "application/x-www-form-urlencoded";
            _req.Headers.Add("Accept-language", "zh-cn,zh;q=0.5");
            _req.Headers.Add("Accept-Charset", "GB2312,utf-8;q=0.7,*;q=0.7");
            //_req.UserAgent =
            //    "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2; .NET4.0C; .NET4.0E)";
            _req.Headers.Add("Accept-Encoding", "gzip, deflate");
            _req.Headers.Add("Keep-Alive", "350");
            _req.Headers.Add("x-requested-with", "XMLHttpRequest");
            //仿百度蜘蛛
            _req.UserAgent = "Mozilla/5.0+(compatible;+Baiduspider/2.0;++http://www.baidu.com/search/spider.html)";
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

            if (_method.ToUpper() == "POST" && !string.IsNullOrEmpty(_paras))
            {
                byte[] buffer = _encoding.GetBytes(_paras);
                _req.ContentLength = buffer.Length;
                _req.GetRequestStream().Write(buffer, 0, buffer.Length);
            }
        }

        public void SetHttpInfo(string url,string cookie,string referer)
        {
            _url = url;
            _cookie = cookie;
            _referer = referer;
        }

        public void SetUrl(string url)
        {
            _url = url;
        }

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
            if (_rep != null)
                _rep.Close();
            if (_req != null)
                _req.Abort();
        }

        #endregion
    }
}
