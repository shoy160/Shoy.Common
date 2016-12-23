using Shoy.OnlinePay.App.Domain;
using Shoy.Utility;
using Shoy.Utility.Config;
using Shoy.Utility.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Shoy.OnlinePay.App.Factory
{
    /// <summary> 在线支付基类 </summary>
    public abstract class DPay
    {
        protected ILogger Logger = LogManager.Logger("online-pay");
        protected PlatConfig Config;

        protected PlatConfig Get(PaidType type)
        {
            var config = ConfigUtils<OnlinePayConfig>.Instance.Get();
            return config?.Platforms?.FirstOrDefault(t => t.Type == type);
        }

        /// <summary> 支付通知 </summary>
        /// <returns></returns>
        public abstract DResult<VerifyDto> Verify();

        /// <summary> 构造支付参数 & 签名 </summary>
        /// <param name="tradeNo">订单号</param>
        /// <param name="price">价格</param>
        /// <param name="subject">标题</param>
        /// <returns></returns>
        public abstract DResult<Dictionary<string, string>> Request(string tradeNo, decimal price, string subject);
    }
}
