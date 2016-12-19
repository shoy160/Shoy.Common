using System;
using System.Xml.Serialization;

namespace DayEasy.God.Services.OnlinePay.Domain
{
    /// <summary> 微信支付返回接口 </summary>
    [Serializable]
    [XmlRoot("xml")]
    public class ReturnWeixinDto
    {
        [XmlElement]
        public string return_code { get; set; }
        [XmlElement]
        public string return_msg { get; set; }
        [XmlElement]
        public string result_code { get; set; }
        [XmlElement]
        public string trade_type { get; set; }
        [XmlElement]
        public string prepay_id { get; set; }
    }
}
