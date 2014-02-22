using System.Configuration;

namespace Shoy.Solr
{
    public class SolrConfig : ConfigurationSection
    {
        [ConfigurationProperty("solr", IsDefaultCollection = false)]
        internal SolrBase Solr
        {
            get { return (SolrBase) base["solr"]; }
            set { base["solr"] = value; }
        }
    }

    /// <summary>
    /// Solr基础配置
    /// </summary>
    internal class SolrBase : ConfigurationElement
    {
        public SolrBase() { }

        public SolrBase(string baseUrl, string dataType = "json")
        {
            BaseUrl = baseUrl;
        }

        /// <summary>
        /// solr服务器地址
        /// </summary>
        [ConfigurationProperty("baseUrl", IsRequired = true)]
        public string BaseUrl
        {
            get { return (string)base["baseUrl"]; }
            set { base["baseUrl"] = value; }
        }

        /// <summary>
        /// core名称
        /// </summary>
        [ConfigurationProperty("coreName", IsRequired = true)]
        public string CoreName
        {
            get { return (string)base["coreName"]; }
            set { base["coreName"] = value; }
        }

        /// <summary>
        /// 数据格式
        /// </summary>
        [ConfigurationProperty("dataType", DefaultValue = "json")]
        public string DataType
        {
            get { return (string)base["dataType"]; }
            set { base["dataType"] = value; }
        }
    }
}
