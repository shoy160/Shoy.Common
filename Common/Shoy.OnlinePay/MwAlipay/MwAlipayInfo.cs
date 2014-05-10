using System;
namespace Shoy.OnlinePay.MwAlipay
{
    public class MwAlipayInfo : ParameterInfo
    {
        public string NotifyUrl { get; set; }

        public MwAlipayInfo(string orderNum, decimal amount, string returnUrl, string notifyUrl)
        {
            OrderNum = orderNum;
            Amount = amount;
            ReturnUrl = returnUrl;
            NotifyUrl = notifyUrl;
            ProName = "百货购手机在线支付";
        }
        public MwAlipayInfo(decimal amount, string returnUrl, string notifyUrl)
        {
            var orderNum = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            OrderNum = orderNum;
            Amount = amount;
            ReturnUrl = returnUrl;
            NotifyUrl = notifyUrl;
            ProName = "百货购手机在线支付";
        }
    }
}
