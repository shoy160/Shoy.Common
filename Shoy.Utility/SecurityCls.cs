using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Web.Security;

namespace Shoy.Utility
{
    /// <summary>
    /// 加密类
    /// </summary>
    public static class SecurityCls
    {
        private const string Key64 = "hugehuge";
        private const string Iv64 = "hange168";

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data">字符</param>
        /// <returns></returns>
        public static string Encode(string data)
        {
            try
            {
                byte[] byKey = Encoding.ASCII.GetBytes(Key64);
                byte[] byIv = Encoding.ASCII.GetBytes(Iv64);
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
        /// 解密
        /// </summary>
        /// <param name="data">字符</param>
        /// <returns></returns>
        public static string Decode(string data)
        {
            try
            {
                byte[] byKey = Encoding.ASCII.GetBytes(Key64);
                byte[] byIv = Encoding.ASCII.GetBytes(Iv64);
                var len = data.Length/2;
                var dataByte = new byte[len];
                int x, i;
                for (x = 0; x < len; x++)
                {
                    i = Convert.ToInt32(data.Substring(x*2, 2), 16);
                    dataByte[x] = (byte) i;
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
        /// Md5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Md5(string str)
        {
            string md5 = FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
            return md5;
        }
    }
}
