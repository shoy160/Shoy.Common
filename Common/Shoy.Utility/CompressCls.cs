using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace Shoy.Utility
{
    /// <summary>
    /// 文件压缩解压类
    /// </summary>
    public class CompressCls
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
    }
}
