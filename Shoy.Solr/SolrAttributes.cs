using System;

namespace Shoy.Solr
{
    /// <summary>
    /// Solr属性基类
    /// </summary>
    public class SolrAttribute:Attribute
    {}

    /// <summary>
    /// Solr核心
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SolrCore : SolrAttribute
    {
        public SolrCore(string coreName)
        {
            CoreName = coreName;
        }

        public string CoreName { get; set; }
    }

    /// <summary>
    /// Solr字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SolrField : SolrAttribute
    {
        public SolrField(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; set; }
    }

    /// <summary>
    /// Solr关键字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SolrKey : SolrField
    {
        public SolrKey(string fieldName)
            : base(fieldName)
        {
        }
    }
}
