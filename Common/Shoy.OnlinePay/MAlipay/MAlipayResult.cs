
namespace Shoy.OnlinePay.MAlipay
{
    public class MAlipayResult:BaseResult
    {
        public string TradeStatus { get; set; }
        public string BuyerEmail { get; set; }
        /// <summary>
        /// 支付宝交易号
        /// </summary>
        public string TradeNo { get; set; }
    }
}
