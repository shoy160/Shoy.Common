using DayEasy.God.Contracts.Dtos;
using DayEasy.God.Services.OnlinePay.Domain;
using DayEasy.Utility.Config;
using DayEasy.Utility.Logging;
using System.Collections.Generic;
using System.Linq;

namespace DayEasy.God.Services.OnlinePay.Factory
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
        /// <summary> 支付请求 </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="price">价格</param>
        /// <param name="subject">标题</param>
        /// <returns></returns>
        public abstract DApiResult<string> Pay(long orderNo, decimal price, string subject);

        /// <summary> 支付通知 </summary>
        /// <returns></returns>
        public abstract DApiResult<VerifyDto> Verify();

        public abstract IDictionary<string, object> Request(string prepayId = null);
    }
}
