namespace Shoy.OnlinePay
{
    public class ParameterInfo
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNum { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        public string ProName { get; set; }

        /// <summary>
        /// 返回链接
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 商家扩展信息
        /// </summary>
        public string ExtentInfo { get; set; }
    }
}
