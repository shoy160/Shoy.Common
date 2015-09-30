using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Shoy.Utility.Config;

namespace Shoy.MongoDb.Configs
{
    /// <summary>
    /// MongoDB配置文件
    /// </summary>
    [Serializable]
    [FileName("mongo.config")]
    [XmlRoot("root")]
    public class MongoConfig : ConfigBase
    {
        [XmlArray("servers"), XmlArrayItem("item")]
        public List<DServer> Servers { get; set; }

        [XmlArray("credentials"), XmlArrayItem("item")]
        public List<DCredential> Credentials { get; set; }
    }

    [Serializable]
    public class DServer
    {
        /// <summary> Ip </summary>
        [XmlAttribute("host")]
        public string Host { get; set; }

        /// <summary> 端口号 </summary>
        [XmlAttribute("port")]
        public int Port { get; set; }
    }

    /// <summary> 身份认证 </summary>
    [Serializable]
    public class DCredential
    {
        [XmlAttribute("database")]
        public string DataBase { get; set; }

        [XmlAttribute("user")]
        public string User { get; set; }

        [XmlAttribute("pwd")]
        public string Pwd { get; set; }
    }
}
