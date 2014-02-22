using System;
namespace Shoy.OnlinePay.UnionPay
{
    public class UnionPayParasInfo : ParameterInfo
    {
        private string _productUrl = "http://www.100hg.com";
        public string ProductUrl { get { return _productUrl; } set { _productUrl = value; } }

        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Cur { get { return "156"; } }

        /// <summary>
        /// 通知url
        /// </summary>
        public string NotifyUrl { get; set; }

        public UnionPayParasInfo(string orderNum,decimal amount,string returnUrl,string notifyUrl,string showUrl)
        {
            if (string.IsNullOrEmpty(orderNum))
                orderNum = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            OrderNum = orderNum;
            Amount = amount;
            ReturnUrl = returnUrl;
            NotifyUrl = notifyUrl;
            if (!string.IsNullOrEmpty(showUrl))
                ProductUrl = showUrl;
        }
    }
}
