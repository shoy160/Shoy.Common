using System;

namespace Shoy.OnlinePay
{
    public class YeePayParasInfo : ParameterInfo
    {
        /// <summary>
        /// 币种
        /// </summary>
        public string Cur { get { return "CNY"; } }

        /// <summary>
        /// 是否需要保存送货地址
        /// </summary>
        public string Saf { get { return "0"; } }

        public string ProCate { get; set; }

        public string ProDesc { get; set; }

        /// <summary>
        /// 支付通道
        /// </summary>
        public string FrpId { get; set; }

        /// <summary>
        /// 是否需要应答
        /// </summary>
        public bool NeedResponse { get; set; }

        /// <summary>
        /// 易宝支付
        /// </summary>
        /// <param name="orderNum">支付订单号</param>
        /// <param name="amount">金额</param>
        /// <param name="name">支付标题</param>
        /// <param name="extent">扩展信息</param>
        /// <param name="returnUrl">返回链接(url编码)</param>
        /// <param name="frpId">银行编码</param>
        /// <param name="needResponse">是否需要响应</param>
        public YeePayParasInfo(string orderNum, decimal amount, string name, string extent, string returnUrl, string frpId, bool needResponse)
        {
            OrderNum = orderNum;
            Amount = amount;
            ProName = name;
            ExtentInfo = extent;
            ReturnUrl = returnUrl;
            FrpId = frpId;
            NeedResponse = needResponse;
        }

        /// <summary>
        /// 易宝支付
        /// </summary>
        /// <param name="amount">金额</param>
        /// <param name="name">支付标题</param>
        /// <param name="returnUrl">返回链接(url编码)</param>
        /// <param name="frpId">银行编码</param>
        public YeePayParasInfo(decimal amount, string name, string returnUrl, string frpId)
        {
            OrderNum = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            Amount = amount;
            ProName = name;
            ExtentInfo = "";
            ReturnUrl = returnUrl;
            FrpId = frpId;
            NeedResponse = true;
        }
    }
}