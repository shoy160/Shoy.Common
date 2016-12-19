using DayEasy.Utility.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DayEasy.God.Services.OnlinePay
{
    public enum PaidType
    {
        [Description("支付宝")]
        Alipay = 0,
        [Description("微信")]
        Weixin = 1
    }

    [Serializable]
    [XmlRoot("root")]
    [FileName("online_pay.config")]
    public class OnlinePayConfig : ConfigBase
    {
        [XmlArray("platforms")]
        [XmlArrayItem("item")]
        public List<PlatConfig> Platforms { get; set; }
    }

    public class PlatConfig
    {
        [XmlAttribute("type")]
        public PaidType Type { get; set; }
        [XmlAttribute("appid")]
        public string AppId { get; set; }
        [XmlAttribute("format")]
        public string Format { get; set; }
        [XmlAttribute("charset")]
        public string Charset { get; set; }
        [XmlElement("gateway")]
        public string Gateway { get; set; }
        [XmlElement("notify")]
        public string Notify { get; set; }
        [XmlElement("privateKey")]
        public string PrivateKey { get; set; }
    }
}
