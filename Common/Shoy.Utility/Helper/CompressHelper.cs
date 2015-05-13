using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.Win32;
using Shoy.Utility.Extend;

namespace Shoy.Utility.Helper
{
    /// <summary>
    /// 压缩解压类
    /// </summary>
    public static class CompressHelper
    {
        /// <summary>
        /// 是否安装了Winrar
        /// </summary>
        /// <returns></returns>
        public static bool Exists()
        {
            RegistryKey theReg = Registry.LocalMachine.OpenSubKey(Consts.WinRarPath);
            return theReg != null && !string.IsNullOrEmpty(theReg.GetValue(string.Empty).ToString());
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="sourcePath">源文件</param>
        /// <param name="rarPath">rar文件路径</param>
        public static bool CompressRar(string sourcePath, string rarPath)
        {
            string rarName = Path.GetFileName(rarPath);
            if (string.IsNullOrEmpty(rarName))
                return false;
            string rarDirectory = rarPath.Replace(rarName, string.Empty);
            try
            {
                RegistryKey theReg = Registry.LocalMachine.OpenSubKey(Consts.WinRarPath);
                if (theReg == null)
                {
                    return false;
                }
                object theObj = theReg.GetValue(string.Empty);
                string theRar = theObj.ToString();
                theReg.Close();
                //theRar = theRar.Substring(1, theRar.Length - 7);
                if (!Directory.Exists(rarDirectory))
                    Directory.CreateDirectory(rarDirectory);

                //命令参数
                //the_Info = " a  (-p"123") " + rarName + " " + @"C:Test?70821.txt"; //文件压缩
                string theInfo = string.Format(Consts.CompressCommand, rarName, sourcePath);
                var theStartInfo = new ProcessStartInfo
                                       {
                                           FileName = theRar,
                                           Arguments = theInfo,
                                           WindowStyle = ProcessWindowStyle.Hidden,
                                           WorkingDirectory = rarDirectory
                                       };
                //打包文件存放目录
                var theProcess = new Process { StartInfo = theStartInfo };
                theProcess.Start();
                theProcess.WaitForExit();
                theProcess.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="rarPath">压缩文件路径</param>
        /// <param name="newPath">解压到文件路径</param>
        /// <returns></returns>
        public static bool DecompressionRar(string rarPath, string newPath)
        {
            string rarName = Path.GetFileName(rarPath);
            if (string.IsNullOrEmpty(rarName))
                return false;
            string rarDirectory = rarPath.Replace(rarName, string.Empty);
            try
            {
                RegistryKey theReg = Registry.LocalMachine.OpenSubKey(Consts.WinRarPath);
                if (theReg == null)
                    return false;
                object theObj = theReg.GetValue(string.Empty);
                string theRar = theObj.ToString();
                theReg.Close();
                //the_rar = the_rar.Substring(1, the_rar.Length - 7);

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                //命令参数 x (-p"123") rar.rar d:\rar -y
                string theInfo = string.Format(Consts.UnzipCommand, rarName, newPath);

                var theStartInfo = new ProcessStartInfo
                                       {
                                           FileName = theRar,
                                           Arguments = theInfo,
                                           WindowStyle = ProcessWindowStyle.Hidden,
                                           WorkingDirectory = rarDirectory
                                       };

                var theProcess = new Process { StartInfo = theStartInfo };
                theProcess.Start();
                theProcess.WaitForExit();
                theProcess.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 对byte数组进行压缩
        /// </summary>
        /// <param name="data">待压缩的byte数组</param>
        /// <returns>压缩后的byte数组</returns>
        public static byte[] Compress(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                var zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(data, 0, data.Length);
                zip.Close();
                var buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        /// <summary>
        /// 对byte[]数组进行解压
        /// </summary>
        /// <param name="data">待解压的byte数组</param>
        /// <returns>解压后的byte数组</returns>
        public static byte[] Decompress(byte[] data)
        {
            using (var tmpMs = new MemoryStream())
            {
                using (var ms = new MemoryStream(data))
                {
                    var zip = new GZipStream(ms, CompressionMode.Decompress, true);
                    zip.CopyTo(tmpMs);
                    zip.Close();
                }
                return tmpMs.ToArray();
            }
        }

        /// <summary>
        /// 对字符串进行压缩
        /// </summary>
        /// <param name="value">待压缩的字符串</param>
        /// <returns>压缩后的字符串</returns>
        public static string Compress(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            bytes = Compress(bytes);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 对字符串进行解压
        /// </summary>
        /// <param name="value">待解压的字符串</param>
        /// <returns>解压后的字符串</returns>
        public static string Decompress(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            byte[] bytes = Convert.FromBase64String(value);
            bytes = Decompress(bytes);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
