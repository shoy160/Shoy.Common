using System;
using System.Linq;
using System.Xml.Serialization;

namespace Shoy.Utility.Config
{
    [Serializable]
    public class ConfigUtils<T>
        where T : ConfigBase
    {
        private string _fileName;

        public static ConfigUtils<T> Instance()
        {
            return new ConfigUtils<T>();
        }

        [XmlIgnore]
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                {
                    var attrs = typeof (T).GetCustomAttributes(typeof (FileNameAttribute), true);
                    if (attrs.Any())
                    {
                        var attr = attrs.FirstOrDefault() as FileNameAttribute;
                        if (attr != null) _fileName = attr.Name;
                    }
                }
                return _fileName;
            }
        }

        public T Get(string fileName = "")
        {
            if (string.IsNullOrEmpty(fileName)) fileName = FileName;
            return ConfigManager.GetConfig<T>(fileName);
        }

        public void Set(T config, string fileName = "")
        {
            if (string.IsNullOrEmpty(fileName)) fileName = FileName;
            ConfigManager.SetConfig(fileName, config);
        }
    }
}
