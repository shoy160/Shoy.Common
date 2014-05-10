using System;

namespace Shoy.OnlinePay
{
    public class AlipayParasInfo : ParameterInfo
    {
        /// <summary>
        /// 通知链接
        /// </summary>
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 展示链接
        /// </summary>
        public string ShowUrl { get; set; }

        public string Account { get; set; }

        /// <summary>
        /// 支付宝
        /// </summary>
        /// <param name="orderNum">支付订单号</param>
        /// <param name="amount">金额</param>
        /// <param name="name">支付标题</param>
        /// <param name="extent">扩展信息</param>
        /// <param name="returnUrl">返回链接</param>
        /// <param name="notifyUrl">通知链接</param>
        /// <param name="showUrl">展示链接</param>
        /// <param name="account">支付宝帐号</param>
        public AlipayParasInfo(string orderNum,decimal amount,string name,string extent,string returnUrl,string notifyUrl,string showUrl,string account)
        {
            OrderNum = orderNum;
            Amount = amount;
            ProName = name;
            ExtentInfo = extent;
            ReturnUrl = returnUrl;
            NotifyUrl = notifyUrl;
            ShowUrl = showUrl;
            Account = account;
        }

        public AlipayParasInfo(decimal amount, string name, string returnUrl, string notifyUrl, string showUrl, string account)
        {
            OrderNum = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            Amount = amount;
            ProName = name;
            ExtentInfo = "";
            ReturnUrl = returnUrl;
            NotifyUrl = notifyUrl;
            ShowUrl = (showUrl ?? "http://www.100hg.com");
            Account = account;
        }
    }
}