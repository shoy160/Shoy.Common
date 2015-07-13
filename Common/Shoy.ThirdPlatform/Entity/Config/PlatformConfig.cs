using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Shoy.Utility.Config;

namespace Shoy.ThirdPlatform.Entity.Config
{
    /// <summary> 平台配置文件 </summary>
    [Serializable]
    [FileName("platform.config")]
    [XmlRoot("root")]
    public class PlatformConfig : ConfigBase
    {
        [XmlArray("platforms"), XmlArrayItem("item")]
        public List<Platform> Platforms { get; set; }
    }

    [Serializable]
    public class Platform
    {
        private string _charset = "utf-8";
        private string _signType = "MD5";

        [XmlAttribute("type")]
        public int PlatType { get; set; }

        [XmlAttribute("partner")]
        public string Partner { get; set; }

        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlAttribute("charset")]
        public string Charset
        {
            get { return _charset; }
            set { _charset = value; }
        }

        [XmlAttribute("sign-type")]
        public string SignType
        {
            get { return _signType; }
            set { _signType = value; }
        }

        [XmlElement("tokenUrl")]
        public string TokenUrl { get; set; }

        [XmlElement("authorizeUrl")]
        public string AuthorizeUrl { get; set; }

        [XmlElement("userUrl")]
        public string UserUrl { get; set; }

        [XmlElement("openIdUrl")]
        public string OpenIdUrl { get; set; }
    }
}
