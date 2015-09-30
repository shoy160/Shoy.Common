using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Shoy.Utility.Config;

namespace Shoy.Assistant.Config
{
    /// <summary>
    /// 消息队列配置
    /// </summary>
    [Serializable]
    [XmlRoot("rabbitMq")]
    [FileName("rabbitMq.config")]
    public class RabbitMqConfig : ConfigBase
    {
        /// <summary>
        /// 队列列表
        /// </summary>
        [XmlArray("rabbits")]
        [XmlArrayItem("item")]
        public List<RabbitMqItem> RabbitMqList { get; set; }

        public RabbitMqItem Default
        {
            get
            {
                var item = (RabbitMqList.FirstOrDefault(t => t.IsDefault) ?? RabbitMqList.FirstOrDefault());
                return item;
            }
        }
    }

    [Serializable]
    public class RabbitMqItem
    {
        /// <summary>
        /// 队列URL
        /// </summary>
        [XmlText]
        public string Url { get; set; }

        /// <summary>
        /// VHost
        /// </summary>
        [XmlAttribute("vhost")]
        public string VHost { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [XmlAttribute("user")]
        public string UserName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [XmlAttribute("pwd")]
        public string UserPwd { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        [XmlAttribute("isDefault")]
        public bool IsDefault { get; set; }
    }
}