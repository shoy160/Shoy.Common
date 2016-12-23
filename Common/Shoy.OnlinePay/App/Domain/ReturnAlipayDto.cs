using System;

namespace Shoy.OnlinePay.App.Domain
{
    /// <summary> 支付宝返回数据格式 </summary>
    [Serializable]
    public class ReturnAlipayDto
    {
        public AlipayResult alipay_trade_pay_response { get; set; }
    }

    public class AlipayResult
    {
        public string code { get; set; }
        public string msg { get; set; }
        public string sub_code { get; set; }
        public string sub_msg { get; set; }
    }
}
