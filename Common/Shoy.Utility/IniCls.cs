using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Shoy.Utility
{
    /// <summary>
    /// 配置文件类
    /// </summary>
    public class IniCls
    {
        private readonly string _inipath;
        private string _sectionName;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary> 
        /// 构造方法 
        /// </summary> 
        /// <param name="iniPath">文件路径</param> 
        public IniCls(string iniPath)
        {
            _inipath = iniPath;
        }

        /// <summary>
        /// 设置Section
        /// </summary>
        /// <param name="sectionName"></param>
        public void SetSection(string sectionName)
        {
            _sectionName = sectionName;
        }

        /// <summary> 
        /// 写入INI文件 
        /// </summary> 
        /// <param name="section">项目名称(如 [TypeName] )</param> 
        /// <param name="key">键</param> 
        /// <param name="value">值</param> 
        public void Write(string key, string value, string section = null)
        {
            if (string.IsNullOrWhiteSpace(section))
                section = _sectionName;
            WritePrivateProfileString(section, key, value, _inipath);
        }

        /// <summary> 
        /// 读出INI文件 
        /// </summary> 
        /// <param name="section">项目名称(如 [TypeName] )</param> 
        /// <param name="key">键</param> 
        public string Read(string key, string section = null)
        {
            if (string.IsNullOrWhiteSpace(section))
                section = _sectionName;
            var temp = new StringBuilder(500);
            int i = GetPrivateProfileString(section, key, string.Empty, temp, 500, _inipath);
            return temp.ToString();
        }

        /// <summary> 
        /// 验证文件是否存在 
        /// </summary> 
        /// <returns>布尔值</returns> 
        public bool IsExist()
        {
            return File.Exists(_inipath);
        }
    }
}