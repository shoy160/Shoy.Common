using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Shoy.Utility.Config;

namespace Shoy.Assistant.Config
{
    /// <summary>
    /// Solr配置文件
    /// </summary>
    [Serializable]
    [XmlRoot("root")]
    [FileName("solrs.config")]
    public class SolrConfig : ConfigBase
    {
        [XmlArray("solrs")]
        [XmlArrayItem("item")]
        public List<SolrItem> SolrList { get; set; }
    }

    public class SolrItem
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText]
        public string CoreUrl { get; set; }

        [XmlAttribute("coreName")]
        public string CoreName { get; set; }

        [XmlAttribute("dataType")]
        public string DataType { get; set; }
    }
}
