using System;
using SolrNet;

namespace Shoy.Assistant.Solr
{
    /// <summary> Solr属性基类 </summary>
    public class SolrAttribute : Attribute
    { }

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
    public class DSolrField : SolrAttribute
    {
        public DSolrField(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; set; }
    }

    /// <summary>
    /// Solr关键字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DSolrKey : DSolrField
    {
        public DSolrKey(string fieldName)
            : base(fieldName)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class DSolrOrder : SolrAttribute
    {
        public string FieldName;

        public Order OrderType { get; set; }

        public DSolrOrder(string field, Order type = Order.ASC)
        {
            FieldName = field;
            OrderType = type;
        }
    }
}
