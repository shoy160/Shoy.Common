namespace Shoy.OnlinePay.App.Domain
{
    public class VerifyDto
    {
        /// <summary> 商户订单号 </summary>
        public string Id { get; set; }
        /// <summary> 支付订单号 </summary>
        public string TradeNo { get; set; }
        public decimal Amount { get; set; }
        /// <summary> 支付状态 </summary>
        public string TradeStatus { get; set; }
        public string BuyerId { get; set; }
        public string BuyerAccount { get; set; }
    }
}
