using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Shoy.Utility.Config;

namespace Shoy.MemoryDb.Configs
{
    [Serializable]
    [XmlRoot("root")]
    [FileName("redis.config")]
    public class RedisConfig : ConfigBase
    {
        [XmlElement("writePoolSize")]
        public int WritePoolSize { get; set; }

        [XmlElement("readPoolSize")]
        public int ReadPoolSize { get; set; }

        [XmlElement("autoStart")]
        public bool AutoStart { get; set; }

        [XmlElement("defaultDb")]
        public long? DefaultDb { get; set; }

        /// <summary> 可读写服务器 </summary>
        [XmlArray("readWrite"), XmlArrayItem("item")]
        public List<RedisDetail> ReadAndWriteServers { get; set; }

        /// <summary> 只读服务器 </summary>
        [XmlArray("readOnly"), XmlArrayItem("item")]
        public List<RedisDetail> ReadOnlyServers { get; set; }
    }

    [Serializable]
    public class RedisDetail
    {
        /// <summary> 服务器IP </summary>
        [XmlAttribute("ip")]
        public string Ip { get; set; }

        /// <summary> 服务器端口 </summary>
        [XmlAttribute("port")]
        public int Port { get; set; }

        /// <summary> 密码认证 </summary>
        [XmlAttribute("auth")]
        public string Auth { get; set; }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Auth)
                ? string.Format("{0}:{1}", Ip, Port)
                : string.Format("{2}@{0}:{1}", Ip, Port, Auth);
        }
    }
}
