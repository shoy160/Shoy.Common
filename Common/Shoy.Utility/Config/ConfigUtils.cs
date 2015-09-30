using System;
using System.Linq;
using System.Xml.Serialization;

namespace Shoy.Utility.Config
{
    [Serializable]
    public class ConfigUtils<T>
        where T : ConfigBase
    {
        private static ConfigUtils<T> _instance;
        private string _fileName;
        private ConfigUtils() { }

        public static ConfigUtils<T> Instance
        {
            get { return _instance ?? (_instance = new ConfigUtils<T>()); }
        }

        public static T Config
        {
            get { return Instance.Get(); }
        }

        [XmlIgnore]
        public string FileName
        {
            get
            {
                if (!string.IsNullOrEmpty(_fileName))
                    return _fileName;
                var attrs = typeof(T).GetCustomAttributes(typeof(FileNameAttribute), true);
                if (!attrs.Any())
                    return _fileName;
                var attr = attrs.FirstOrDefault() as FileNameAttribute;
                if (attr != null) _fileName = attr.Name;
                return _fileName;
            }
        }

        public T Get(string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName)) fileName = FileName;
            return ConfigManager.GetConfig<T>(fileName);
        }

        public void Set(T config, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName)) fileName = FileName;
            ConfigManager.SetConfig(fileName, config);
        }
    }
}
