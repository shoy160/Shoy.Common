using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Shoy.Utility.Helper
{
    /// <summary>
    /// 加密类
    /// </summary>
    public static class SecurityHelper
    {
        private const string Key64 = "shoyluyn";
        private const string Iv64 = "shoy1168";

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data">字符</param>
        /// <param name="key">Key</param>
        /// <param name="iv">IV</param>
        /// <returns></returns>
        public static string Encode(string data, string key, string iv)
        {
            try
            {
                byte[] byKey = Encoding.ASCII.GetBytes(key);
                byte[] byIv = Encoding.ASCII.GetBytes(iv);
                var dataByte = Encoding.UTF8.GetBytes(data);
                var sb = new StringBuilder();

                using (var des = new DESCryptoServiceProvider())
                {
                    using (var ms = new MemoryStream())
                    {
                        using (
                            var cst = new CryptoStream(ms, des.CreateEncryptor(byKey, byIv),
                                                       CryptoStreamMode.Write))
                        {
                            cst.Write(dataByte, 0, dataByte.Length);
                            cst.FlushFinalBlock();
                            foreach (byte b in ms.ToArray())
                            {
                                sb.AppendFormat("{0:x2}", b);
                            }
                            return sb.ToString();
                        }
                    }
                }
            }
            catch
            {
                return data;
            }
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data">待加密字符</param>
        /// <returns></returns>
        public static string Encode(string data)
        {
            return Encode(data, Key64, Iv64);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data">解密字符</param>
        /// <param name="key">Key</param>
        /// <param name="iv">IV</param>
        /// <returns></returns>
        public static string Decode(string data, string key, string iv)
        {
            try
            {
                byte[] byKey = Encoding.ASCII.GetBytes(key);
                byte[] byIv = Encoding.ASCII.GetBytes(iv);
                var len = data.Length / 2;
                var dataByte = new byte[len];
                int x, i;
                for (x = 0; x < len; x++)
                {
                    i = Convert.ToInt32(data.Substring(x * 2, 2), 16);
                    dataByte[x] = (byte)i;
                }
                using (var des = new DESCryptoServiceProvider())
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var cst = new CryptoStream(ms, des.CreateDecryptor(byKey, byIv), CryptoStreamMode.Write))
                        {
                            cst.Write(dataByte, 0, dataByte.Length);
                            cst.FlushFinalBlock();
                            return Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }
            }
            catch
            {
                return data;
            }
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data">解密字符</param>
        /// <returns></returns>
        public static string Decode(string data)
        {
            return Decode(data, Key64, Iv64);
        }

        /// <summary>
        /// Md5加密
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <param name="salt">加密盐</param>
        /// <returns></returns>
        public static string Md5(string str, string salt = null)
        {
            if (!string.IsNullOrEmpty(salt))
                str = string.Format("{0}[[{1}]]", str, salt);
            var md5 = MD5.Create();
            var bs = Encoding.UTF8.GetBytes(str);
            var hs = md5.ComputeHash(bs);
            var sb = new StringBuilder();
            foreach (var b in hs)
            {
                // 以十六进制格式格式化
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
            //string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
            //return md5;
        }
    }
}
